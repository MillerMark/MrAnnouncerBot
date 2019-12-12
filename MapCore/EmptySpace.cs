using System;
using System.Linq;

namespace MapCore
{
	public class EmptySpace: BaseSpace
	{

		public EmptySpace(int column, int row): base(column, row)
		{
			SpaceType = SpaceType.Empty;
		}
	}
}
