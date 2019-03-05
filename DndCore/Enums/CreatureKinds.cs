using System;
using System.Linq;

namespace DndCore
{
	[Flags]
	public enum CreatureKinds
	{
		None = 0,
		Aberrations = 1,
		Beasts = 2,
		Celestials = 4,
		Constructs = 8,
		Dragons = 16,
		Elemental = 32,
		Fey = 64,
		Fiends = 128,
		Giants = 256,
		Humanoids = 512,
		Monstrosities = 1024,
		Oozes = 2048,
		Plants = 4096,
		Undead = 8192,
	}
}