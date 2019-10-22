using System;
using System.Linq;
using System.Windows;
using DndCore;

namespace DndUI
{
	public class SkillCheckEventArgs : RoutedEventArgs
	{
		public Skills Skill { get; }
		public VantageKind VantageKind { get; set; }
		public SkillCheckEventArgs(RoutedEvent routedEvent, Skills skill, VantageKind vantageKind) : base(routedEvent)
		{
			VantageKind = vantageKind;
			Skill = skill;
		}
	}
}
