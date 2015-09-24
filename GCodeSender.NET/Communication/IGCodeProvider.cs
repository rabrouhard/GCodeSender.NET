using System;

namespace GCodeSender.NET
{
	interface IGCodeProvider
	{
		event Action LineAdded;
		string GetLine();
		int PeekLineLength();
		bool HasLine { get; }
		bool IsRunning { get; }
		void Stop();
	}
}
