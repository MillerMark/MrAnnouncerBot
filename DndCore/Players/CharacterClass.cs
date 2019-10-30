using System;
using System.Linq;

namespace DndCore
{
	public class CharacterClass
	{
		public event LevelChangedEventHandler LevelChanged;
		protected virtual void OnLevelChanged(object sender, LevelChangedEventArgs ea)
		{
			LevelChanged?.Invoke(sender, ea);
		}
		public string Name { get; set; }
		public Class Class { get; set; }
		public SubClass SubClass { get; set; }

		int level;
		public int Level
		{
			get
			{
				return level;
			}
			set
			{
				if (level == value)
					return;

				int oldLevel = level;
				level = value;
				OnLevelChanged(this, new LevelChangedEventArgs(oldLevel, level));
			}
		}

		public string HitDice { get; set; }

		public CharacterClass(string name, int level, string hitDice = "")
		{
			Name = name;
			Class = DndUtils.ToClass(name);
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