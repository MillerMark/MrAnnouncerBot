using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapCore
{
	public class MapRegion: IFlyweight
	{
		[JsonIgnore]
		public IMapInterface ParentMap { get; set; }

		public Guid Guid { get; set; }

		[JsonIgnore]
		public List<Tile> Tiles { get; set; } = new List<Tile>();

		List<Guid> tileGuids;

		public MapRegion(IMapInterface map)
		{
			ParentMap = map;
		}

		List<Guid> GetTileGuids()
		{
			List<Guid> guids = new List<Guid>();
			foreach (Tile tile in Tiles)
				guids.Add(tile.Guid);

			return guids;
		}

		void InvalidateTileGuids()
		{
			tileGuids = null;
		}

		public void AddTile(Tile tile)
		{
			Tiles.Add(tile);
			InvalidateTileGuids();
		}

		public List<Guid> TileGuids
		{
			get
			{
				return tileGuids;
			}
			set
			{
				if (tileGuids == null)
					tileGuids = GetTileGuids();
				tileGuids = value;
			}
		}

		// TODO: Call this after serializing this from disk.
		public void Reconstitute()
		{
			foreach (Guid guid in TileGuids)
			{
				AddTile(ParentMap.GetFlyweight<Tile>(guid));
			}
		}

		public double Width { get; private set; }
		public double Height { get; private set; }
	}
}
