using System;
using System.Linq;

namespace DndCore
{
	public static class CreatureSizes
	{
		public const CreatureSize LargeOrSmaller = CreatureSize.Tiny | CreatureSize.Small | CreatureSize.Medium | CreatureSize.Large;
		public const CreatureSize All = CreatureSize.Tiny | CreatureSize.Small | CreatureSize.Medium | CreatureSize.Large | CreatureSize.Huge | CreatureSize.Gargantuan;

	}
}

