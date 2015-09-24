using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GCodeSender.NET
{
	partial class MainWindow
	{
		private void textBoxSettingBaud_Loaded(object sender, RoutedEventArgs e)
		{
			textBoxSettingBaud.Text = $"{Properties.Settings.Default.SerialBaudRate}";
		}

		private void textBoxSettingBaud_Unloaded(object sender, RoutedEventArgs e)
		{
			int newbaud;
			if (int.TryParse(textBoxSettingBaud.Text, out newbaud))
			{
				if (newbaud > 300 && newbaud < 2000000)
				{
					Properties.Settings.Default.SerialBaudRate = newbaud;
					return;
				}
			}
			App.Message("Invalid Baud Rate");
		}
	}
}
