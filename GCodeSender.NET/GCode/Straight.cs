﻿namespace GCodeSender.NET
{
	class Straight : Movement
	{
		public bool Rapid;

		public Straight(Vector3 start, Vector3 end, bool rapid) : base(start, end)
		{
			Rapid = rapid;
		}

		public override string GetGCode()
		{
			string code = Rapid ? "G0" : "G1";

			if(End.X != Start.X)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "X{0:0.###}", End.X);
			if (End.Y != Start.Y)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "Y{0:0.###}", End.Y);
			if (End.Z != Start.Z)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "Z{0:0.###}", End.Z);
			if (FeedRate.HasValue)
				code += string.Format(System.Globalization.CultureInfo.InvariantCulture, "F{0:F0}", FeedRate);
			return code;
		}

		public override double Length
		{
			get
			{
				return (End - Start).Magnitude;
			}
		}
	}
}
