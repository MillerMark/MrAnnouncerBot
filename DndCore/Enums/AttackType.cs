using System;
using System.ComponentModel;
using System.Linq;

namespace DndCore
{
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum AttackType
	{
		None,
		Melee,
		[Description("Martial Melee")]
		MartialMelee,
		Range,
		[Description("Martial Range")]
		MartialRange,
		Spell,
		Area
	}
}