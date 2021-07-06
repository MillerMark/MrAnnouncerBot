using System;

namespace DndCore
{
	[Flags]
	public enum TargetKind
	{
		None = 0,
		Self = 1,
		Creatures = 2,
		Location = 4,
		Object = 8,
		Volume = 16,
		Wall = 32,
		Ray = 64,
		Other = 128
	}
}
