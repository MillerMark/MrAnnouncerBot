using System;
using System.Linq;

namespace DndCore
{
	public class CharacterClass
	{
		public string Name { get; set; }
		public int Level { get; set; }
		public string HitDice { get; set; }

		public CharacterClass(string name, int level, string hitDice = "")
		{
			Name = name;
			Level = level;
			HitDice = hitDice;
		}

		public CharacterClass()
		{

		}

		public override string ToString()
		{
			return $"{Name} {Level}";
		}
	}
}