using System;
using System.Linq;

namespace MapCore
{
	public class EditablePropertyAttribute : Attribute
	{
		public string DisplayText { get; set; }


		// TODO: Expect to be able to bind custom property editors, so figure that out Mark!!!.
		public EditablePropertyAttribute()
		{

		}

		public EditablePropertyAttribute(string displayText)
		{
			DisplayText = displayText;
		}
	}
}
