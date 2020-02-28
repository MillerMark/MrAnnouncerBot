using System;
using System.Linq;
using MapCore;

namespace DndMapSpike
{
	public static class ItemFactory
	{
		public static IItemProperties CreateStampFrom(SerializedStamp stamp)
		{
			// TODO: Also be able to reconstruct Characters and CharacterGroups
			if (stamp.TypeName == "StampGroup")
				return StampGroup.From(stamp);
			return Stamp.From(stamp);
		}
	}
}

