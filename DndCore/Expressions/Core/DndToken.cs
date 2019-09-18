using System;
using System.Linq;

namespace DndCore
{
	public class DndToken
	{
		public virtual string Name { get; set; }

		public virtual bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			return tokenName == Name;
		}

		public DndToken()
		{

		}
	}
}

