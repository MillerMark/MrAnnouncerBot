using System;
using System.Linq;
using System.Collections.Generic;

namespace TaleSpireCore
{
	public class CharacterPositions
	{
		public List<CharacterPosition> Characters { get; set; }
		public CharacterPositions()
		{
			Characters = new List<CharacterPosition>();
		}

		public void PruneSides(WhatSide whatSide)
		{
			Characters = Characters.Where(x =>
				whatSide.HasFlag(WhatSide.Friendly) && Talespire.Target.IsAlly(x.ID) ||
				whatSide.HasFlag(WhatSide.Enemy) && Talespire.Target.IsEnemy(x.ID) ||
				whatSide.HasFlag(WhatSide.Neutral) && Talespire.Target.IsNeutral(x.ID)
				).ToList();
		}
	}
}
