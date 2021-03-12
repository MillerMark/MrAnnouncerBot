using System;

namespace DndCore
{
	public class ValidationResult
	{
		public ValidationAction ValidationAction { get; set; }
		public string MessageToDm { get; set; }
		public string MessageOverPlayer { get; set; }
		public ValidationResult()
		{

		}
	}
}
