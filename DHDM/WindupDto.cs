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
		public int Lifespan { get; set; }
		public int FadeIn { get; set; }
		public int FadeOut { get; set; }
		public int Hue { get; set; }
		public int Saturation { get; set; }
		public int Brightness { get; set; }
		public string SoundFileName { get; set; }
		public double Rotation { get; set; }
		public double AutoRotation { get; set; }
		public int DegreesOffset { get; set; }
		public Vector Velocity { get; set; }
		public Vector Offset { get; set; }
		public Vector Force { get; set; }
		public double ForceAmount { get; set; }
		public bool FlipHorizontal { get; set; }
		public bool FlipVertical { get; set; }
		public WindupDto()
		{
			Saturation = 100;
			Brightness = 100;
			FadeIn = 400;
			Scale = 1;
		}
	}
}
