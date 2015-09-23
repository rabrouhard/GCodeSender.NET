using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Timers;
using GCodeSender.NET.Properties;

namespace GCodeSender.NET
{
	static class Connection
	{
		public static event Action Connected;
		public static event Action Disconnected;

		private static bool _isConnected = false;
		public static bool IsConnected
		{
			get
			{
				return _isConnected;
			}
			private set
			{
				if (_isConnected != value)
				{
					if (value)
					{
						Connected();
					}
					else
					{
						Disconnected();
					}
				}

				_isConnected = value;
			}
		}

		public static string PortName { get; private set; }

		static Stream ConnectionStream;

		static Timer ReceiveTimer;
		static StringBuilder ReceivedString;
		static byte[] InputBuffer;

		private static Func<bool> ConnectionStreamAvailable;

		static Connection()
		{
			ReceiveTimer = new Timer();
			ReceiveTimer.Interval = Settings.Default.ReceiveTimerInterval;
			ReceiveTimer.AutoReset = true;
			ReceiveTimer.Elapsed += ReceiveTimer_Elapsed;

			ReceivedString = new StringBuilder(Settings.Default.ReceiveBufferSize);

			InputBuffer = new byte[Settings.Default.ReceiveBufferSize];
		}

		public delegate void LineReceivedDelegate(string line);
		public static event LineReceivedDelegate LineReceived;

		private static async void ReceiveTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!ConnectionStream.CanRead)
				Disconnect();

			if (!ConnectionStreamAvailable())
				return;

			int received = await ConnectionStream.ReadAsync(InputBuffer, 0, InputBuffer.Length);

			if (received > 0)   //find line breaks (\r\n)
			{
				ReceivedString.Append(Encoding.ASCII.GetChars(InputBuffer, 0, received));

				bool lineFound = true;

				while (lineFound)
				{
					lineFound = false;

					for (int searchIndex = 0; searchIndex < ReceivedString.Length; searchIndex++)
					{
						if (ReceivedString[searchIndex] == '\n')
						{
							string line;

							if (ReceivedString[searchIndex - 1] == '\r')
								line = ReceivedString.ToString(0, searchIndex - 1);  //discard \r + \n
							else
								line = ReceivedString.ToString(0, searchIndex);     //discard \n

							LineReceived(line);

							ReceivedString.Remove(0, searchIndex + 1);

							lineFound = true;
						}
					}
				}
			}


		}

		public static void Disconnect()
		{
			ReceiveTimer.Stop();
			ReceivedString.Clear();

			if (ConnectionStream != null)
			{
				ConnectionStream.Close();
				ConnectionStream = null;
			}

			IsConnected = false;
		}

		private static void ConnectStream(Stream stream)
		{
			ConnectionStream = stream;

			IsConnected = true;

			ReceiveTimer.Start();
		}

		public static bool ConnectNetwork(string address)
		{
			Disconnect();
			PortName = address;

			try
			{
				TcpClient tcpc = new TcpClient();
				tcpc.Connect(Util.ParseIPEndPoint(address));

				NetworkStream nets = tcpc.GetStream();

				ConnectionStreamAvailable = () => { return nets.DataAvailable; };

				ConnectStream(nets);

				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool ConnectSerial(string portName)
		{
			Disconnect();
			PortName = portName;

			try
			{
				SerialPort port = new SerialPort(portName, Settings.Default.SerialBaudRate);
				port.Open();

				ConnectionStreamAvailable = () => { return port.BytesToRead > 0; };

				ConnectStream(port.BaseStream);

				

				return true;
			}
			catch
			{
				return false;
			}
		}







	}
}