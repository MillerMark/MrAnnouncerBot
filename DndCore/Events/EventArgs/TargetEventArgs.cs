﻿using System;

namespace DndCore
{
	public enum TargetType
	{
		None,
		Friendly,
		Foe
	}
	
	public class TargetEventArgs : EventArgs
	{
		public Target Target { get; set; }
		public Character Player { get; set; }
		public bool AllowSelf { get; set; }
		public bool ShowUI { get; set; }
		public int MinTargets { get; set; } = 1;
		public int MaxTargets { get; set; } = 1;
		public TargetType SuggestedTargetType { get; set; }
		public TargetStatus TargetStatus { get; set; }
		public TargetEventArgs()
		{

		}
	}
}
