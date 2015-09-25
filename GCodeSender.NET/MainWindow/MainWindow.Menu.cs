using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GCodeSender.NET
{
	partial class MainWindow
	{
		private void Menu_ListSerialports(object sender, RoutedEventArgs e)
		{
			menuItemConnect.Items.Clear();

			foreach (string port in SerialPort.GetPortNames())
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
			if (!Connection.ConnectSerial((string)((MenuItem)sender).Header))
			{
				App.Message("Serial Error");
			}
		}

		private void MenuTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (menuMain.IsKeyboardFocusWithin)
				e.Handled = true;
		}
		private void Menu_Connect_Network_Click(object sender, RoutedEventArgs e)
		{
			if (!Connection.ConnectNetwork(textBoxNetworkAddress.Text))
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

	}
}
