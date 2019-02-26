using System;
using System.ComponentModel;
using System.Linq;

namespace DndCore
{
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum AttackKind
	{
		Magical = 1,
		[Description("Non-magical")]
		NonMagical = 2,
		Any = 3
	}
}