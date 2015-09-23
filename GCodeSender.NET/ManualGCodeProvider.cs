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

		public int AvailableLines { get { return CommandQueue.Count; } }

		public bool IsActive
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public event Action LineAdded;

		public string GetLine()
		{
			return CommandQueue.Dequeue();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}
	}
}
