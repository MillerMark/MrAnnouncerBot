using MapCore;

namespace MapCore
{
	public class WallData
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int WallLength { get; set; }

		public int StartColumn { get; set; }
		public int StartRow { get; set; }
		public int EndColumn { get; set; }
		public int EndRow { get; set; }

		public WallData()
		{
		}
	}
}
