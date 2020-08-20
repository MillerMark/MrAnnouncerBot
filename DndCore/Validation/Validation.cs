using System;
using System.Linq;

namespace DndCore
{
	public static class Validation
	{
		public static event ValidationEventHandler ValidationFailing;
		public static event ValidationEventHandler ValidationFailed;
		public static void OnValidationFailed(string dungeonMasterMessage, string floatText, ValidationAction validationAction)
		{
			ValidationEventArgs ea = new ValidationEventArgs(dungeonMasterMessage, floatText, validationAction);
			ValidationFailing?.Invoke(null, ea);
			if (ea.OverrideWarning)
				return;

			ValidationFailed?.Invoke(null, ea);
		}
	}
}
