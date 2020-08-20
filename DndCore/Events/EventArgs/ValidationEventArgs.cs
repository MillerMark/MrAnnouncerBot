using System;
using System.Linq;

namespace DndCore
{
	public class ValidationEventArgs : EventArgs
	{
		public ValidationEventArgs(string dungeonMasterMessage, string floatText, ValidationAction validationAction)
		{
			FloatText = floatText;
			ValidationAction = validationAction;
			DungeonMasterMessage = dungeonMasterMessage;
		}

		public string DungeonMasterMessage { get; set; }
		public ValidationAction ValidationAction { get; set; }
		public string FloatText { get; set; }
		public bool OverrideWarning { get; set; }
	}
}
