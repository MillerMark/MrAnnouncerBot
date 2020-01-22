using System;
using System.Linq;

namespace MapCore
{
	public interface IMapInterface
	{
		bool FloorSpaceExists(int column, int row);
		Tile GetTile(int column, int row);
		void FloorTypeChanged(Tile tile);
		bool HasWallLeft(int column, int row);
		bool HasWallRight(int column, int row);
		bool HasWallTop(int column, int row);
		bool HasWallBottom(int column, int row);
		T GetFlyweight<T>(Guid parentGuid) where T : class;
	}
}
