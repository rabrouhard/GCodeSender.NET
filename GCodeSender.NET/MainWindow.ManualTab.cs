using System;
using System.Collections.Generic;
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

		private string _stringManualIncrement = "1.0";
		private double manualIncrement = 1.0;
		public string manualIncrementString {
			get { return _stringManualIncrement; }
			set
			{
				_stringManualIncrement = value;
				double.TryParse(value, out manualIncrement);
			}
		}
		private void buttonManualMove(object sender, RoutedEventArgs e)
		{
			if (manualIncrement <= 0)
				return;
		}
	}
}
