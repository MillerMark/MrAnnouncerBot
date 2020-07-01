using System;
using System.Linq;

namespace DndCore
{
	public static class KnownQualifiers
	{
		public const string Spell = "spell_";
		public static bool IsSpellQualifier(string expression)
		{
			return expression.StartsWith(Spell);
		}
		public static bool StartsWithKnownQualifier(string expression)
		{
			if (IsSpellQualifier(expression))
				return true;

			return false;
		}
	}
}

