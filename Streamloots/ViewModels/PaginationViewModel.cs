using System;
using System.Linq;

namespace Streamloots
{
	public class PaginationViewModel
	{
		public CursorsViewModel cursors { get; set; }
		public string next { get; set; }
		public PaginationViewModel()
		{

		}
	}
}
