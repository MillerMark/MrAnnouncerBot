using System;
using System.Linq;

namespace Streamloots
{
	public class PageViewModel
	{
		public string _id { get; set; }
		public string slug { get; set; }
		public OwnerViewModel owner { get; set; }
		public OwnerType type { get; set; }
		public PageViewModel()
		{

		}
	}
}
