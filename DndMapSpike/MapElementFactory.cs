using System;
using System.Linq;
using MapCore;

namespace DndMapSpike
{
	public static class MapElementFactory
	{
		public static IItemProperties CreateItemFrom(SerializedItem stamp)
		{
			// Coding Gorilla's suggestion has been implemented here!!!
			// ![](5BB3346892E7D77EE7412ACA3E59628A.png;;36,0,203,187)
			// TODO: Also be able to reconstruct Characters and CharacterGroups
			if (stamp.TypeName == "StampGroup")
				return StampGroup.From(stamp);

			if (stamp.TypeName == "MapCharacterGroup")
				return MapCharacterGroup.From(stamp);

			if (stamp.TypeName == "MapCharacter")
				return MapCharacter.From(stamp);
			return Stamp.From(stamp);
		}
	}
}

