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
			Console.WriteLine("Initializing Main Window");
			InitializeComponent();
			Connection.Connected += Connection_Connected;
			Connection.Connected += ConnectionStatusChanged;
			Connection.Disconnected += Connection_Disconnected;
			Connection.Disconnected += ConnectionStatusChanged;

			GCodeStreamer.GCodeProviderChanged += ConnectionStatusChanged;
			Console.WriteLine("Initialized Main Window");
		}

		private void ConnectionStatusChanged()
		{
			tabItemManualMode.IsEnabled = Connection.IsConnected && GCodeStreamer.IsManualMode;
			tabItemFileMode.IsEnabled = Connection.IsConnected && (GCodeStreamer.IsManualMode  /*|| GCodeStreamer.IsFileMode*/);

			if (!((TabItem)tabControl.SelectedItem).IsEnabled)
				tabControl.SelectedItem = tabItemStatus;
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

		private void Command_Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			
		}

		private void Command_SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			Properties.Settings.Default.Save();
		}
	}
}
