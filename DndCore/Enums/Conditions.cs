using System;
using System.Linq;
using System.ComponentModel;

namespace DndCore
{
	[Flags]
	public enum Conditions
	{
		None = 0,
		Blinded = 1,
		Charmed = 2,
		Deafened = 4,
		Fatigued = 8,  // Exhaustion
		Frightened = 16,
		Grappled = 32,
		Incapacitated = 64,
		Invisible = 128,
		Paralyzed = 256,
		Petrified = 512,
		Poisoned = 1024,
		Prone = 2048,
		Restrained = 4096,
		Stunned = 8192,
		Unconscious = 16384,
		Sleep = 32768,  // Added this to support monster condition immunities as some monsters are immune to mods that cause sleep.
		Dead = 65536,
		Burning = 131072,   // Home-brew condition added.
		Climbing = 262144,  // Home-brew condition added.
		Flying = 524288,		// Home-brew condition added.
		Raging = 1048576,		// Home-brew condition added.
		Swimming = 2097152  // Home-brew condition added.
	}
}
