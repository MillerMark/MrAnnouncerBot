using System;
using System.Linq;
using System.ComponentModel;

namespace DndCore.Enums
{
	// TODO: Kill this and everything related to it:
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum ComparisonFilterOption
	{
		TargetSizeLessThan,
		TargetSizeGreaterThan,
		TargetSizeEqualTo,
		CreatureChoice,
		None
	}
}
