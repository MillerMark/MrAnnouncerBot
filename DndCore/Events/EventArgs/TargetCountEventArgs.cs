using System;
using System.Linq;

namespace DndCore
{
	public class TargetCountEventArgs : EventArgs
	{
		public int Count { get; set; }
		public WhatSide WhatSide { get; set; }
		public TargetCountEventArgs()
		{

		}
	}
}
