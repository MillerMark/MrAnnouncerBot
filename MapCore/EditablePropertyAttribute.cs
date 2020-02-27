using System;
using System.Linq;

namespace MapCore
{
	public class EditablePropertyAttribute : Attribute
	{
		public int NumDecimalPlaces { get; set; } = 8;

		public string DisplayText { get; set; }
		public string DependentProperty { get; set; }


		// TODO: Expect to be able to bind custom property editors, so figure that out Mark!!!.
		public EditablePropertyAttribute()
		{
		}

		public EditablePropertyAttribute(string displayText, string dependentProperty = null, int numDecimalPlaces = 8)
		{
			NumDecimalPlaces = numDecimalPlaces;
			DependentProperty = dependentProperty;
			DisplayText = displayText;
		}

		public EditablePropertyAttribute(int numDecimalPlaces)
		{
			NumDecimalPlaces = numDecimalPlaces;
		}
	}
}
