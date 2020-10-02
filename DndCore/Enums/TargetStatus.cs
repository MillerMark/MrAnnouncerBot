using System;
using System.Linq;

namespace DndCore
{
	public enum TargetStatus
	{
		None = 0,
		Friendly = 1,
		Adversarial = 2,
		Unknown = 4,
		AllTargets = Friendly | Adversarial | Unknown
	}
}
