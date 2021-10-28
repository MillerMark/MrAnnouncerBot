using System;
using System.Linq;
using GoogleHelper;

namespace DHDM
{
	[SheetName("Live Video Animation")]
	[TabName("Bindings")]
	public class VideoAnimationBinding
	{
		public string SceneName { get; set; }
		public string MovementFileName { get; set; }
		public string Camera1 { get; set; }
		public string Camera2 { get; set; }
		public string Camera3 { get; set; }
		public string Camera4 { get; set; }
		public double StartTimeOffset { get; set; } = 0;
		public double TimeStretchFactor { get; set; } = 1;
	}

}
