using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

namespace GCodeSender.NET
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Connection.Connected += Connection_Connected;
			Connection.Disconnected += Connection_Disconnected;
			Connection.LineReceived += Connection_LineReceived;
		}

		private void Connection_LineReceived(string line)
		{
			Console.WriteLine(line);
		}

		private void Connection_Disconnected()
		{
			menuItemConnect.Visibility = Visibility.Visible;
			menuItemDisconnect.Visibility = Visibility.Collapsed;
		}

		private void Connection_Connected()
		{
			menuItemConnect.Visibility = Visibility.Collapsed;
			menuItemDisconnect.Header = $"Disconnect {Connection.PortName}";
			menuItemDisconnect.Visibility = Visibility.Visible;
		}

		private void Menu_ListSerialports(object sender, RoutedEventArgs e)
		{
			menuItemConnect.Items.Clear();

			foreach(string port in SerialPort.GetPortNames())
			{
				MenuItem portItem = new MenuItem();

				portItem.Header = port;

				portItem.Click += PortItem_Click;
				
				menuItemConnect.Items.Add(portItem);
			}

			menuItemConnect.Items.Add(menuItemNetwork);			
		}

		private void PortItem_Click(object sender, RoutedEventArgs e)
		{
			if(!Connection.ConnectSerial((string)((MenuItem)sender).Header))
			{
				App.Message("Serial Error");
			}
		}

		private void MenuTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (menuMain.IsKeyboardFocusWithin)
				e.Handled = true;
		}

		private void Command_Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			
		}

		private void Command_SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void Menu_Connect_Network_Click(object sender, RoutedEventArgs e)
		{
			if(!Connection.ConnectNetwork(textBoxNetworkAddress.Text))
			{
				App.Message("Network Error");
			}
		}

		private void menuItemDisconnect_Click(object sender, RoutedEventArgs e)
		{
			Connection.Disconnect();
		}

		private void textBoxNetworkAddress_Load(object sender, RoutedEventArgs e)
		{
			textBoxNetworkAddress.Text = Properties.Settings.Default.NetworkAddress;
		}

		private void textBoxNetworkAddress_Unload(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default.NetworkAddress = textBoxNetworkAddress.Text;
        }

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			Properties.Settings.Default.Save();
		}
	}
}
