using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum SpellComponents
	{
		Verbal = 1,
		Somatic = 2,
		Material = 4
	}
}
