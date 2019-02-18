using System;
using System.Linq;

namespace DndCore
{
	public enum Conditions
	{
		none = 0,
		blinded = 1,
		charmed = 2,
		deafened = 4,
		fatigued = 8,
		frightened = 16,
		grappled = 32,
		incapacitated = 64,
		invisible = 128,
		paralyzed = 256,
		petrified = 512,
		poisoned = 1024,
		prone = 2048,
		restrained = 4096,
		stunned = 8192,
		unconscious = 16384,
		sleep = 32768  // Added this to support monster condition immunities.
	}
}
