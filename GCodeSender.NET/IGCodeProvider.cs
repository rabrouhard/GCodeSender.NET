using System;

namespace GCodeSender.NET
{
	interface IGCodeProvider
	{
		event Action LineAdded;
		event Action Completed;
		string GetLine();
		int PeekLineLength();
		bool HasLine { get; }
		bool IsRunning { get; }
		void Stop();
	}
}
