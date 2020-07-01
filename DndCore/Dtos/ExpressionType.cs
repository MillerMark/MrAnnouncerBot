using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum ExpressionType
	{
		unknown = 0,
		boolean = 1,
		number = 2,
		text = 4,
		@enum = 8
	}
}
