using System;
using System.Linq;

namespace DHDM
{
	public class ViewerRollDto
	{
		public string RollId { get; set; }
		public string Label { get; set; }
		public string UserName { get; set; }
		public string RollStr { get; set; }
		public string FontColor { get; set; }
		public string OutlineColor { get; set; }
		public int QueuePosition { get; set; }
		public ViewerRollDto()
		{

		}
	}
}
