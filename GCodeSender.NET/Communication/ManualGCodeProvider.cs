using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	class ManualGCodeProvider : IGCodeProvider
	{
		private Queue<string> CommandQueue = new Queue<string>();

		public bool HasLine
		{
			get
			{
				return CommandQueue.Count > 0;
			}
		}

		public bool IsRunning
		{
			get
			{
				return false;
			}
		}
		
		public int PeekLineLength()
		{
			return CommandQueue.Peek().Length;
		}

		public string GetLine()
		{
			return CommandQueue.Dequeue();
		}

		public void Stop()
		{
			CommandQueue.Clear();
		}

		public void SendLine(string line)
		{
			CommandQueue.Enqueue(line);
		}
	}
}
