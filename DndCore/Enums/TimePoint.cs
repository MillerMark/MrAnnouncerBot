using System;
using System.Linq;
using System.ComponentModel;

namespace DndCore.Enums
{
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum TimePoint
	{
		None,
		Immediately,
		[Description("Start of Turn")]
		StartOfTurn,
		[Description("End of Turn")]
		EndOfTurn
	}
}
