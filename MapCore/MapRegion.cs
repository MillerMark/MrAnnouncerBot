using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapCore
{
	public class MapRegion: IFlyweight
	{
		public string TypeName { get; set; } = typeof(MapRegion).FullName;
		public RegionType RegionType { get; set; }

		[JsonIgnore]
		public IMapInterface ParentMap { get; set; }

		public Guid Guid { get; set; }

		public bool NeedsGuid()
		{
			return Guid == Guid.Empty;
		}

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
			tile.Parent = this;
			Tiles.Add(tile);
			InvalidateTileGuids();
		}

		public List<Guid> TileGuids
		{
			get
			{
				if (tileGuids == null)
					tileGuids = GetTileGuids();
				return tileGuids;
			}
			set
			{
				tileGuids = value;
			}
		}

		// TODO: Call this after serializing this from disk.
		public void Reconstitute()
		{
			List<Guid> tileGuids = TileGuids;
			foreach (Guid guid in tileGuids)
			{
				AddTile(ParentMap.GetFlyweight<Tile>(guid));
			}
		}

		public void PrepareForSerialization()
		{

		}

		public double Width { get; private set; }
		public double Height { get; private set; }
	}
}
