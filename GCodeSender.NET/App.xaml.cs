﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GCodeSender.NET
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App : Application
	{
		public static CultureInfo CultureEN = new CultureInfo("en-US");

		public static void Message(string message)
		{
			MessageBox.Show(message);
		}
	}
}
