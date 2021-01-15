using System;
using System.Linq;
using System.Collections.Generic;

namespace Streamloots
{
	public class SetCardResult
	{
		public List<SetCardViewModel> cards { get; set; }
		public SetCardResult()
		{
			
		}
	}
	public class CollectionPaginatedResult
	{
		public string _id { get; set; }
		public List<CollectionViewModel> data { get; set; }
		public PaginationViewModel pagination { get; set; }

		public CollectionPaginatedResult()
		{

		}
	}
}
