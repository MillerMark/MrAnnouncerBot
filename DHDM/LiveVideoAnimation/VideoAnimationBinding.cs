using System;
using System.Linq;
using SheetsPersist;

namespace DHDM
{
	[Document("Live Video Animation")]
	[Sheet("Bindings")]
	public class VideoAnimationBinding
	{
		public string SceneName { get; set; }
		public string MovementFileName { get; set; }

		[Column("Camera1⬤")]
		public string Camera1 { get; set; }

		[Column("Camera2⬤")]
		public string Camera2 { get; set; }

		[Column("Camera3⬤")]
		public string Camera3 { get; set; }

		[Column("Camera4⬤")]
		public string Camera4 { get; set; }

		public double StartTimeOffset { get; set; } = 0;
		public double TimeStretchFactor { get; set; } = 1;
	}

}
