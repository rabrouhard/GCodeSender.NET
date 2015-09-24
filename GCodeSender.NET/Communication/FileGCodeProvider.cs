using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	class FileGCodeProvider : IGCodeProvider
	{
		private Queue<string> Lines = new Queue<string>();
		private bool isPaused = true;

		public FileGCodeProvider(string path)
		{
			foreach (string line in File.ReadAllLines(path))
			{
				string cleanLine = line.Remove(line.IndexOfAny(new char[] { ';', '(' }));
				cleanLine.Replace(" ", "");

				if (!string.IsNullOrWhiteSpace(cleanLine))
					Lines.Enqueue(line);
			}
		}


		public bool HasLine { get { return Lines.Count > 0 && (!isPaused); } }

		public bool IsRunning { get { return Lines.Count > 0; } }

		public event Action LineAdded;

		public string GetLine()
		{
			return Lines.Dequeue();
		}

		public int PeekLineLength()
		{
			return Lines.Peek().Length;
		}

		public void Stop()
		{
			Lines.Clear();
		}

		public void Pause()
		{
			isPaused = true;
		}

		public void Start()
		{
			isPaused = false;
			LineAdded();
		}
	}
}
