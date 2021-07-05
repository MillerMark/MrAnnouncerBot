using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum WhatSide
	{
		None = 0,
		Enemy = 1,
		Friendly = 2,
		Neutral = 4,
		Unknown = 8,
		All = Enemy | Friendly | Neutral | Unknown
	}
}
