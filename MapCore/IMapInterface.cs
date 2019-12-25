using System;
using System.Linq;

namespace MapCore
{
	public interface IMapInterface
	{
		bool FloorSpaceExists(int column, int row);
	}
}
