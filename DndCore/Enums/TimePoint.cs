using System;
using System.ComponentModel;
using System.Linq;

namespace DndCore
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
