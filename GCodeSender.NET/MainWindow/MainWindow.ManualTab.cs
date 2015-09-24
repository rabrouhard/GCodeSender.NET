using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GCodeSender.NET
{
	partial class MainWindow
	{
		private void textBoxManualIncrement_Loaded(object sender, RoutedEventArgs e)
		{
			textBoxManualIncrement.Text = Properties.Settings.Default.ManualJogIncrement.ToString();
		}

		private void buttonManualMove(object sender, RoutedEventArgs e)
		{
			double increment;

			if (!double.TryParse(textBoxManualIncrement.Text, NumberStyles.Float, App.CultureEN, out increment))
			{
				App.Message("Invalid Increment");
				return;
			}

			Properties.Settings.Default.ManualJogIncrement = increment;
		}

		private void buttonManualCommand_Click(object sender, RoutedEventArgs e)
		{
			GCodeStreamer.ManualProvider.SendLine(textBoxManualCommand.Text);
		}

		private void textBoxManualCommand_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Return)
			{
				e.Handled = true;
				buttonManualCommand_Click(null, null);
			}
		}
	}
}
