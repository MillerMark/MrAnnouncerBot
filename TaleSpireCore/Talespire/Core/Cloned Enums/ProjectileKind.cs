using System;
using System.Linq;

namespace TaleSpireCore
{
	//! Cloned Enum - maintain sync with FireCollisionEventOn enum in D&D and TaleSpireExplore solutions.
	public enum ProjectileKind
	{
		ToVolume,
		DistributeAmongAllTargets,
		EachTarget
	}
}