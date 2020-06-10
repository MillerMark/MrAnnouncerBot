using System;

namespace DndCore
{
	public class TooltipAttribute : Attribute
	{
		public TooltipAttribute(string displayText)
		{
			DisplayText = displayText;
		}

		public string DisplayText { get; set; }
	}
}
