using System;
using System.Linq;
using System.ComponentModel;

namespace DndCore.Enums
{
	[TypeConverter("DndCore.EnumDescriptionTypeConverter")]
	public enum RechargeOdds
	{
		[Description("0/6")]
		ZeroInSix = 0,
		[Description("1/6")]
		OneInSix = 1,
		[Description("2/6")]
		TwoInSix = 2,
		[Description("3/6")]
		ThreeInSix = 3,
		[Description("4/6")]
		FourInSix = 4,
		[Description("5/6")]
		FiveInSix = 5,
		[Description("6/6")]
		SixInSix = 6
	}
}
