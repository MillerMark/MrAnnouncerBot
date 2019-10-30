using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum SpellComponents
	{
		None = 0,
		Verbal = 1,
		Somatic = 2,
		Material = 4,
		All = Verbal | Somatic | Material
	}
}
