﻿using System;
using System.Linq;

namespace TaleSpireCore
{
	[Flags]
	public enum TargetKind
	{
		Enemy = 1,
		Friendly = 2,
		Neutral = 4,
		All = 7
	}
}