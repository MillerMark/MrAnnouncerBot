using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapCore
{
	public class MapRegion
	{
		[JsonIgnore]
		public List<Tile> Spaces { get; set; } = new List<Tile>();
		List<Guid> spaceGuids;
		public List<Guid> SpaceGuids
		{
			get
			{
				return spaceGuids;
			}
			set
			{
				if (spaceGuids == null)
					spaceGuids = GetSpaceGuids();
				spaceGuids = value;
			}
		}
		public double Width { get; private set; }
		public double Height { get; private set; }
	}
}
