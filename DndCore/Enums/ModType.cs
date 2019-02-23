using System;
using System.ComponentModel;
using System.Linq;

namespace DndCore
{
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum ModType
	{
		[Description("Incoming Attack")]
		incomingAttack,
		[Description("Outgoing Attack")]
		outgoingAttack,
		[Description("Condition")]
		condition,
		[Description("Player Property")]
		playerProperty
	}
}
