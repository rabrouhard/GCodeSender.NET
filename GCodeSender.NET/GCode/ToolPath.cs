using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeSender.NET
{
	class ToolPath : List<GCodeCommand>
	{
		public void SaveCommands(StreamWriter file)
		{
			file.WriteLine("G90");
			file.WriteLine("G21");
			file.WriteLine();

			foreach (GCodeCommand Command in this)
			{
				file.WriteLine(Command.GetGCode());
			}

			file.Close();
		}

		public Bounds GetDimensions()
		{
			Bounds b = new Bounds();

			foreach (var Command in this)
			{
				var MoveCommand = Command as Movement;

				if (MoveCommand == null)
					continue;

				b.ExpandTo(MoveCommand.Start.X, MoveCommand.Start.Y);
				b.ExpandTo(MoveCommand.End.X, MoveCommand.End.Y);
			}

			return b;
		}

		public double GetTravelDistance()
		{
			double d = 0;

			foreach (var Command in this)
			{
				var MoveCommand = Command as Movement;

				if (MoveCommand == null)
					continue;

				d += MoveCommand.Length;
			}
			return d;
		}

		public ToolPath ApplyHeightMap(HeightMap map)
		{
			ToolPath adjusted = new ToolPath();

			foreach (GCodeCommand command in this)
			{
				if (command is OtherCode)
				{
					adjusted.Add(command);
					continue;
				}
				else
				{
					Movement m = (Movement)command;

					int divisions = (int)Math.Ceiling(m.Length / map.GridSize);

					if (m is Straight)
					{
						Straight s = (Straight)m;

						if (s.Rapid)
						{
							Vector3 newEnd = s.End;
							newEnd.Z += map.GetHeightAt(s.End.X, s.End.Y);

							adjusted.Add(new Straight(s.Start, newEnd, true));
						}
						else
						{
							Vector3 pos = s.Start;

							for (int x = 1; x <= divisions; x++)
							{
								Vector3 end = s.Start.Interpolate(s.End, (double)x / (double)divisions);
								end.Z += map.GetHeightAt(end.X, end.Y);
								Straight st = new Straight(pos, end, false);
								if (x == 1)
									st.FeedRate = s.FeedRate;
								adjusted.Add(st);
								pos = end;
							}
						}
					}
					if (m is Arc)
					{
						Arc a = (Arc)m;

						Vector3 pos = a.Start;

						double stretch = a.StartAngle - a.EndAngle;

						if (stretch <= 0)
							stretch += 2 * Math.PI;

						if (a.Direction == ArcDirection.CCW)
						{
							stretch = 2 * Math.PI - stretch;
						}

						if (stretch <= 0)
							stretch += 2 * Math.PI;

						for (int x = 1; x <= divisions; x++)
						{
							Vector3 end = new Vector3(a.Radius, 0, 0);

							if (a.Direction != ArcDirection.CCW)
								end.Roll(a.StartAngle + stretch * (double)x / (double)divisions);
							else
								end.Roll(a.StartAngle - stretch * (double)x / (double)divisions);

							end += a.Center;

							end.Z = a.Start.Z + (a.End.Z - a.Start.Z) * (double)x / (double)divisions;

							end.Z += map.GetHeightAt(end.X, end.Y);

							Arc arc = new Arc(pos, end, a.Center, a.Direction);

							if (x == 1)
								arc.FeedRate = a.FeedRate;

							adjusted.Add(arc);
							pos = end;
						}
					}
				}
			}

			return adjusted;
		}
	}
}
