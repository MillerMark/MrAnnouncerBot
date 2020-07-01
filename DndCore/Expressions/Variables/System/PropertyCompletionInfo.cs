using System;
using System.Linq;

namespace DndCore
{
	public class PropertyCompletionInfo
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public ExpressionType Type { get; set; }
		public string EnumTypeName { get; set; }
		public PropertyCompletionInfo()
		{

		}
	}
}

