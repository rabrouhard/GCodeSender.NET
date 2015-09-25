using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	enum ArcDirection
	{
		CW,
		CCW
	}

	class Arc : Movement
	{
		public ArcDirection Direction;

		/// <summary>
		/// Absolute Position of Arc Center
		/// </summary>
		public Vector3 Center;

		public Arc(Vector3 start, Vector3 end, Vector3 center, ArcDirection direction) : base(start, end)
		{
			Center = center;
			Center.Z = 0;
			Direction = direction;
		}

		public override string GetGCode()
		{
			string code = (Direction == ArcDirection.CW) ? "G2" : "G3";

			if (End.X != Start.X)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "X{0:0.###}", End.X);
			if (End.Y != Start.Y)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "Y{0:0.###}", End.Y);
			if (End.Z != Start.Z)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "Z{0:0.###}", End.Z);

			if (Center.X != Start.X)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "I{0:0.###}", Center.X - Start.X);
			if (Center.Y != Start.Y)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "J{0:0.###}", Center.Y - Start.Y);

			if (FeedRate.HasValue)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "F{0:F0}", FeedRate);
			return code;
		}

		public double StartAngle
		{
			get
			{
				Vector3 relStart = Start - Center;
				double a = Math.Atan2(relStart.Y, relStart.X);
				return a >= 0 ? a : 2 * Math.PI + a;
			}
		}

		public double EndAngle
		{
			get
			{
				Vector3 relEnd = End - Center;
				double a = Math.Atan2(relEnd.Y, relEnd.X);
				return a >= 0 ? a : 2 * Math.PI + a;
			}
		}

		public double Radius
		{
			// get average between both radii
			get 
			{
				return (
					Math.Sqrt(Math.Pow(Start.X - Center.X, 2) + Math.Pow(Start.Y - Center.Y, 2)) + 
					Math.Sqrt(Math.Pow(End.X - Center.X, 2) + Math.Pow(End.Y - Center.Y, 2))
					) / 2;
			}
		}

		public override double Length
		{
			get
			{
				double stretch = StartAngle - EndAngle;

				if (stretch < 0)
					stretch += 2 * Math.PI;

				if(Direction == ArcDirection.CCW)
				{
					stretch = 2 * Math.PI - stretch;
				}

				double circumference = stretch * Radius;
				double height = End.X - Start.X;

				return Math.Sqrt(circumference * circumference + height * height);
			}
		}
	}
}
