using System;
using System.Linq;

namespace DndCore
{
	public class DndToken
	{
		public virtual string Name { get; set; }

		public virtual bool Handles(string tokenName, Creature creature, CastedSpell castedSpell)
		{
			return tokenName == Name;
		}

		public DndToken()
		{

		}
	}
}

