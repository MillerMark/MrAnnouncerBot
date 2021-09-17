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
		public string videoAnchorHorizontal { get; set; }
		public string videoAnchorVertical { get; set; }
		public string videoWidth { get; set; }
		public string videoHeight { get; set; }
		public double defaultX { get; set; }
		public double defaultY { get; set; }
	}
}
