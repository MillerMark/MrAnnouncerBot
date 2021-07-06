using System;

namespace DndCore
{
	public class TargetEventArgs : EventArgs
	{
		public Target Target { get; set; }
		public Character Player { get; set; }
		public bool AllowSelf { get; set; }
		public bool ShowUI { get; set; }
		public int MinTargets { get; set; } = 1;
		public int MaxTargets { get; set; } = 1;
		public WhatSide WhatSide { get; set; }
		public TargetEventArgs()
		{

		}
	}
}
