using System;

namespace GCodeSender.NET
{
	interface IGCodeProvider
	{
		event Action LineAdded;
		string GetLine();
		int AvailableLines { get; }
		bool IsActive { get; }
		void Stop();
	}
}
