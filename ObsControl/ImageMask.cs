//#define profiling
using System;
using System.Linq;

namespace ObsControl
{
	public class ImageMask
	{
		public long color { get; set; }
		public string image_path { get; set; }
		public int opacity { get; set; }
		public bool stretch { get; set; }
		public ImageMask()
		{

		}
	}
}
