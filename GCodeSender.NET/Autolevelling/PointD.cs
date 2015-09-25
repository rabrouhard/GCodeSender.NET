using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	class PointD
	{
		public double X { get; set; }
		public double Y { get; set; }

		public PointD(double x, double y)
		{
			X = x;
			Y = y;
		}
	}
}
