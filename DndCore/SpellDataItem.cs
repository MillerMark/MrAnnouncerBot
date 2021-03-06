﻿using System;
using System.Linq;

namespace DndCore
{
	public class SpellDataItem
	{
		public string Name { get; set; }
		public bool RequiresConcentration { get; set; }
		public bool MorePowerfulAtHigherLevels { get; set; }
		public bool IsConcentratingNow { get; set; }
		public bool FromChargedItem { get; set; }
		public SpellDataItem(string name, bool requiresConcentration, bool morePowerfulAtHigherLevels, bool isConcentratingNow, bool fromChargedItem)
		{
			IsConcentratingNow = isConcentratingNow;
			Name = name;
			RequiresConcentration = requiresConcentration;
			MorePowerfulAtHigherLevels = morePowerfulAtHigherLevels;
			FromChargedItem = fromChargedItem;
		}
	}
}
