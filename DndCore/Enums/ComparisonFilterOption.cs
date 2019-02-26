using System;
using System.ComponentModel;
using System.Linq;

namespace DndCore
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
