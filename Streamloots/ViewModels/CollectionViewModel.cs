using System;
using System.Linq;

namespace Streamloots
{
	public class CollectionViewModel
	{
		public string _id { get; set; }
		public PageViewModel page { get; set; }
		public string pageId { get; set; }
		public Statistics statistics { get; set; }
		public bool watched { get; set; }
		public CollectionViewModel()
		{

		}
	}
}
