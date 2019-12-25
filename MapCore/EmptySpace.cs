using System;
using System.Linq;

namespace MapCore
{
	public class EmptySpace: Tile
	{

		public EmptySpace(int column, int row, IMapInterface iMap) : base(column, row, iMap)
		{
			SpaceType = SpaceType.Empty;
		}
	}
}
