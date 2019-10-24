using System;
using System.Linq;
using System.Windows;
using DndCore;

namespace DndUI
{
	public class AbilityEventArgs : RoutedEventArgs
	{
		public Ability Ability { get; set; }
		public VantageKind VantageKind { get; set; }
		public AbilityEventArgs(RoutedEvent routedEvent, Ability ability, VantageKind vantageKind) : base(routedEvent)
		{
			VantageKind = vantageKind;
			Ability = ability;
		}
	}
}
