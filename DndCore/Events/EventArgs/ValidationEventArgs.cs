using System;
using System.Linq;

namespace DndCore
{
	public class ValidationEventArgs : EventArgs
	{
		public ValidationEventArgs(string displayText, ValidationLevel validationLevel, string floatText)
		{
			FloatText = floatText;
			ValidationLevel = validationLevel;
			DisplayText = displayText;
			// TODO: consider using Environment.StackTrace.
		}

		public string DisplayText { get; set; }
		public ValidationLevel ValidationLevel { get; set; }
		public string FloatText { get; set; }
	}
}
