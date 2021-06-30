using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum WhatSide
	{
		Enemy = 1,
		Friendly = 2,
		Neutral = 4,
		All = 7
	}
}
