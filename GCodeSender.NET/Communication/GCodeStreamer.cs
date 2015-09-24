using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	static class GCodeStreamer
	{
		public static event Action GCodeProviderChanged;

		public static Queue<string> ActiveCommands { get; } = new Queue<string>();


		public static ManualGCodeProvider ManualProvider { get; } = new ManualGCodeProvider();

		private static IGCodeProvider _gCodeProvider = ManualProvider;
		private static IGCodeProvider GCodeProvider
		{
			get { return _gCodeProvider; }
			set
			{
				if (_gCodeProvider != value)
				{
					_gCodeProvider = value;

					GCodeProviderChanged();

					if (IsManualMode)
						ManualProvider.Stop();
				}
			}
		}

		public static bool IsManualMode { get { return ManualProvider.Equals(GCodeProvider); } }

		private static int CurrentGRBLBuffer = 0;

		static GCodeStreamer()
		{
			Connection.Disconnected += Reset;
			Connection.LineReceived += Connection_LineReceived;

			ManualProvider.LineAdded += Update;
		}

		/// <summary>
		/// Resets GCodeStreamer, used for cleanup or when irresponsive
		/// </summary>
		public static void Reset()
		{
			ActiveCommands.Clear();
			CurrentGRBLBuffer = 0;

			if (GCodeProvider != null)
			{
				GCodeProvider.Stop();
				GCodeProvider = ManualProvider;
			}
		}

		public static void Stop()
		{
			GCodeProvider.Stop();
			GCodeProvider = ManualProvider;
		}

		public static bool IsActive
		{
			get
			{
				return GCodeProvider.HasLine || ActiveCommands.Count > 0 || GCodeProvider.IsRunning;
			}
		}

		/// <summary>
		/// Sets the source for streaming GCode to the Connection
		/// </summary>
		/// <param name="newProvider">The new GCodeProvider</param>
		/// <returns>Success</returns>
		public static bool SetGCodeProvider(IGCodeProvider newProvider)
		{
			if (IsActive)
				return false;

			GCodeProvider = newProvider;

			return true;
		}

		static void Update()
		{
			while (true)
			{
				if (!GCodeProvider.HasLine)
					return;

				if (CurrentGRBLBuffer + GCodeProvider.PeekLineLength() + 1 > Properties.Settings.Default.GrblBufferSize)    //account for cr + lf
					return;

				string line = GCodeProvider.GetLine();

				CurrentGRBLBuffer += line.Length + 1;

				ActiveCommands.Enqueue(line);

				Connection.WriteLine(line);
			}
		}

		private static void Connection_LineReceived(string line)
		{
			if (line.StartsWith("ok"))
			{
				if (ActiveCommands.Count > 0)
					CurrentGRBLBuffer -= ActiveCommands.Dequeue().Length + 1;
				else
					Console.Error.WriteLine("Received ok without active command");
			}
			else if (line.StartsWith("error"))
			{
				if (ActiveCommands.Count > 0)
					CurrentGRBLBuffer -= ActiveCommands.Dequeue().Length + 1;
				else
					Console.Error.WriteLine("Received ok without active command");

				if (line.StartsWith(Properties.Resources.errorInvalidGCode))
				{
					int errorNo;

					if (int.TryParse(line.Substring(Properties.Resources.errorInvalidGCode.Length), out errorNo))
					{
						if (Util.GrblErrorProvider.Errors.ContainsKey(errorNo))
						{
							App.Message($"{line}\n{Util.GrblErrorProvider.Errors[errorNo]}");
							goto skipMessage;
						}
						else
							Console.Error.WriteLine($"Unknown Error ID '{line}'\ntime to update error definitions?");
					}
					else
						Console.Error.WriteLine($"Couldn't parse Error ID in '{line}'");
				}
				App.Message(line);

			skipMessage:;
			}
			else if (line.StartsWith("Alarm"))
			{
				Stop();
				App.Message(line);
			}

			Update();
		}
	}
}
