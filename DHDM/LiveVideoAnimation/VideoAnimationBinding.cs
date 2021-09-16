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
		public string SourceName { get; set; }
	}
}
