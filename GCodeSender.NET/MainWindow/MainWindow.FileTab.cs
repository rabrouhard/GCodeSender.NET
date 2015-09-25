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
		private void buttonFileStart_Click(object sender, RoutedEventArgs e)
		{
			//testing
			FileGCodeProvider x = new FileGCodeProvider(@"D:\Users\Martin\Desktop\box_logo.tap");
            GCodeStreamer.SetGCodeProvider(x);
			x.Start();
		}

		private void buttonFilePause_Click(object sender, RoutedEventArgs e)
		{

		}

		private void buttonFileStop_Click(object sender, RoutedEventArgs e)
		{

		}

		private void buttonFileReload_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
