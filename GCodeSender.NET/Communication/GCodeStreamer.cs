using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GCodeSender.NET
{
	static class GCodeStreamer
	{
		public static Queue<string> ActiveCommands { get; } = new Queue<string>();
		private static BackgroundWorker streamWorker;

		public static event Action GCodeProviderChanged;
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

		public static event Action<string> LineReceived;

		public static bool IsManualMode { get { return ManualProvider.Equals(GCodeProvider); } }

		private static int CurrentGRBLBuffer = 0;

		static GCodeStreamer()
		{
			Console.WriteLine("Initializing GCode Streamer ...");

			LineReceived += (line)=> { };


			streamWorker = new BackgroundWorker() { WorkerReportsProgress = true };
			streamWorker.DoWork += StreamWorker_DoWork;
			streamWorker.ProgressChanged += StreamWorker_ProgressChanged;
			Connection.Connected += streamWorker.RunWorkerAsync;

			Console.WriteLine("Initialized GCode Streamer");
		}

		private static void StreamWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			LineReceived((string)e.UserState);
		}

		private static void StreamWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				Task<string> receiveTask = Connection.ConnectionReader.ReadLineAsync();	//won't raise exception on disconnect, will set exception prop and IsCompleted prop

				while (!receiveTask.IsCompleted)
				{
					if (GCodeProvider.HasLine && (CurrentGRBLBuffer + GCodeProvider.PeekLineLength() + 1 < Properties.Settings.Default.GrblBufferSize))
					{
						string line = GCodeProvider.GetLine();

						CurrentGRBLBuffer += line.Length + 1;
						ActiveCommands.Enqueue(line);

						Connection.ConnectionWriter.WriteLine(line);
						Console.WriteLine($"sent {line}");
					}

					System.Threading.Thread.Sleep(25);
				}

				if (receiveTask.Exception == null)
				{
					string receivedLine = receiveTask.Result;
					Console.WriteLine($"received {receivedLine}");

					if (!string.IsNullOrWhiteSpace(receivedLine) && !HandleReceivedLine(receivedLine))
						streamWorker.ReportProgress(0, receivedLine);
				}
			}
		}

		public static bool IsActive
		{
			get
			{
				return GCodeProvider.IsRunning || ActiveCommands.Count > 0;
			}
		}

		public static bool SetGCodeProvider(IGCodeProvider newProvider)
		{
			if (IsActive)
				return false;

			GCodeProvider = newProvider;

			return true;
		}


		private static bool HandleReceivedLine(string line)
		{
			if (line.StartsWith("ok"))
			{
				if (ActiveCommands.Count > 0)
					CurrentGRBLBuffer -= ActiveCommands.Dequeue().Length + 1;
				else
					Console.Error.WriteLine("Received ok without active command");

				return true;
			}
			else if (line.StartsWith("error"))
			{
				string activeCommand;

				if (ActiveCommands.Count > 0)
				{
					activeCommand = ActiveCommands.Dequeue();
					CurrentGRBLBuffer -= activeCommand.Length + 1;
				}
				else
				{
					Console.Error.WriteLine("Received ok without active command");
					activeCommand = "no active command";
				}

				if (line.StartsWith(Properties.Resources.errorInvalidGCode))
				{
					int errorNo;

					if (int.TryParse(line.Substring(Properties.Resources.errorInvalidGCode.Length), out errorNo))
					{
						if (Util.GrblErrorProvider.Errors.ContainsKey(errorNo))
						{
							App.Message($"'{activeCommand}':\n'{line}'\n{Util.GrblErrorProvider.Errors[errorNo]}");
							goto skipMessage;
						}
						else
							Console.Error.WriteLine($"Unknown Error ID '{line}'\ntime to update error definitions?");
					}
					else
						Console.Error.WriteLine($"Couldn't parse Error ID in '{line}'");
				}
				App.Message($"'{activeCommand}':\n'{line}'");

			skipMessage:
				return true;
			}
			else if (line.StartsWith("Alarm"))
			{
				App.Message(line);
				return true;
			}

			return false;
		}
	}
}
