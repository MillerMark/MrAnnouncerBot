using System;
using System.Linq;

namespace DndCore
{
	public enum SpellRangeType
	{
		DistanceFeet,
		SelfPlusFeetLine,
		DistanceMiles,
		Self,
		SelfPlusFlatRadius,
		SelfPlusSphereRadius,
		SelfPlusCone,
		SelfPlusCube,
		Unlimited,
		Sight,
		Touch,
		Special
	}
}
