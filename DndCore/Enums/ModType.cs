using System;
using System.Linq;
using System.ComponentModel;

namespace DndCore
{
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum ModType
	{
		[Description("Incoming Attack")]
		incomingAttack = 0,
		[Description("Outgoing Attack")]
		outgoingAttack = 1,
		[Description("Condition")]
		condition = 2,
		[Description("Player Property")]
		playerProperty = 3
	}
}
