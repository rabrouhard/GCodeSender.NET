using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	static class GCodeStreamer
	{
		public static List<string> ActiveCommands { get; } = new List<string>();

		private static IGCodeProvider GCodeProvider;
		public static IGCodeProvider DefaultProvider { get; } = new ManualGCodeProvider();

		private static int CurrentGRBLBuffer = 0;

		static GCodeStreamer()
		{
			Connection.Disconnected += Reset;
		}

		/// <summary>
		/// Resets GCodeStreamer, used for cleanup or when irresponsive
		/// </summary>
		public static void Reset()
		{
			ActiveCommands.Clear();
			CurrentGRBLBuffer = 0;
		}

		public static void Stop()
		{
			if (GCodeProvider != null)
				GCodeProvider.Stop();
		}

		/// <summary>
		/// Sets the source for streaming GCode to the Connection
		/// </summary>
		/// <param name="newProvider">The new GCodeProvider</param>
		/// <returns>Success</returns>
		public static bool SetGCodeProvider(IGCodeProvider newProvider)
		{
			if (ActiveCommands.Count > 0)
				return false;

			if (GCodeProvider?.IsActive ?? false)   //is active + null check
				return false;

			GCodeProvider = newProvider;

			return true;
		}

		static void Update()
		{
			if (GCodeProvider == null || !GCodeProvider.IsActive)
				return;

		}
	}
}
