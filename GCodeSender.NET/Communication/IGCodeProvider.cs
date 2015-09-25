using System;

namespace GCodeSender.NET
{
	interface IGCodeProvider
	{
		string GetLine();
		int PeekLineLength();
		bool HasLine { get; }
		bool IsRunning { get; }
		void Stop();
	}
}
