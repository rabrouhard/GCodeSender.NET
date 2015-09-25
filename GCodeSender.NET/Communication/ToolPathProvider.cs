using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	class ToolPathProvider : IGCodeProvider
	{
		public ToolPath Path { get; private set; }
		int NextLine = 0;

		private bool isPaused = true;

		public ToolPathProvider(ToolPath path)
		{
			Path = path;
		}

		public void Reload()
		{
			NextLine = 0;
		}

		public bool HasLine { get { return (!isPaused) && (NextLine < Path.Count); } }

        public bool IsRunning { get { return (NextLine < Path.Count); } }

		public string GetLine()
		{
			return Path[NextLine++].ToString();
		}

		public int PeekLineLength()
		{
			return Path[NextLine].ToString().Length;
		}

		public void Stop()
		{
			NextLine = Path.Count;
		}

		public void Pause()
		{
			isPaused = true;
		}

		public void Start()
		{
			isPaused = false;
		}
	}
}
