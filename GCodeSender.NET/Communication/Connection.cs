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
			get { return _isConnected; }
			private set
			{
				if (_isConnected != value)
				{
					_isConnected = value;

					if (value)
					{
						Connected();
					}
					else
					{
						Disconnected();
					}
				}
			}
		}

		public static string PortName { get; private set; }

		static Stream ConnectionStream;
		public static StreamReader ConnectionReader { get; private set; }
		public static StreamWriter ConnectionWriter { get; private set; }

		private static Func<bool> ConnectionStreamAvailable;

		static Connection()
		{
			
		}

		public static void Disconnect()
		{
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

			if (ConnectionReader != null)
				ConnectionReader.Dispose();
			ConnectionReader = new StreamReader(ConnectionStream, Encoding.ASCII);

			if (ConnectionWriter != null)
				ConnectionWriter.Dispose();
			ConnectionWriter = new StreamWriter(ConnectionStream, Encoding.ASCII) { AutoFlush = true, NewLine="\n" };

			IsConnected = true;
		}

		public static bool ConnectNetwork(string address)
		{
			Disconnect();
			PortName = address;

			try
			{
				TcpClient tcpc = new TcpClient();
				tcpc.Connect(Util.Parsers.ParseIPEndPoint(address));

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