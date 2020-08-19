using System;
using System.Linq;

namespace DndCore
{
	public static class Validation
	{
		public static event ValidationEventHandler ValidationFailed;
		public static void AssertTrue(bool value, string displayText, string floatText, ValidationLevel validationLevel = ValidationLevel.Error)
		{
			if (value)
				return;
			OnValidationFailed(displayText, validationLevel, floatText);
		}
		public static void OnValidationFailed(string displayText, ValidationLevel validationLevel, string floatText)
		{
			ValidationEventArgs ea = new ValidationEventArgs(displayText, validationLevel, floatText);
			ValidationFailed?.Invoke(null, ea);
		}
	}
}
