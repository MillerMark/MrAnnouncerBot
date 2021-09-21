using System;
using GoogleHelper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHDM
{
  [SheetName("Live Video Animation")]
	[TabName("VideoFeeds")]
	public class VideoFeed
	{
		[Indexer]
		public string sourceName { get; set; }
		public string sceneName { get; set; }
		public double videoAnchorHorizontal { get; set; }
		public double videoAnchorVertical { get; set; }
		public double videoWidth { get; set; }
		public double videoHeight { get; set; }
		public double defaultX { get; set; }
		public double defaultY { get; set; }
	}
}
