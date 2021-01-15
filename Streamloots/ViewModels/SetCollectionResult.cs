using System;
using System.Linq;
using System.Collections.Generic;

namespace Streamloots
{
	public class SetCollectionResult
	{
		public List<SetCardViewModel> data { get; set; }
		public PaginationViewModel pagination { get; set; }

		public SetCollectionResult()
		{

		}
	}
}
