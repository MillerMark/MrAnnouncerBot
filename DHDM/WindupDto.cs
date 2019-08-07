using System;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class WindupDto
	{
		public string Effect { get; set; }
		public string Name { get; set; }
		public double Scale { get; set; }
		public DateTime Expiration { get; set; }
		public int FadeIn { get; set; }
		public int FadeOut { get; set; }
		public int Hue { get; set; }
		public int Saturation { get; set; }
		public int Brightness { get; set; }
		public string SoundFileName { get; set; }
		public double Rotation { get; set; }
		public int DegreesOffset { get; set; }
		public Vector Velocity { get; set; }
		public Vector Offset { get; set; }
		public Vector Force { get; set; }
		public double ForceAmount { get; set; }
		public WindupDto()
		{

		}
	}
}
