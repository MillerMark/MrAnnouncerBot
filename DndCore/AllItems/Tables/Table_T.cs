using System;
using System.Linq;

namespace DndCore
{
	public class Table<T> where T : BaseRow
	{
		public string Name { get; set; }
		public T[] Rows { get; set; }

		public Table()
		{

		}
	}
}

