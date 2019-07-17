using System;
using System.Linq;
using System.Windows;
using DndCore;

namespace DndUI
{
	public class SkillCheckEventArgs : RoutedEventArgs
	{
		public Skills Skill { get; }
		public SkillCheckEventArgs(RoutedEvent routedEvent, Skills skill) : base(routedEvent)
		{
			Skill = skill;
		}
	}
}
