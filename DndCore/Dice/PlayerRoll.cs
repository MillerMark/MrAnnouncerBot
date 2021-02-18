using System;
using System.Linq;

namespace DndCore
{
	public class PlayerRoll
	{
		public int roll;
		public string name;
		public string data;
		public int id;
		public int modifier;
		public int cardModifierTotal;
		public string cardModifiersStr { get; set; }
		public bool success;
		public bool isCrit;
		public bool isCompleteFail;
	}
}
