using System;
using System.Linq;

namespace TaleSpireCore
{
	public enum InteractiveTargetingMode
	{
		None,
		Creatures,  // Can allow for multiple target drops.
		CreatureSelect,  // Select a single creature
		Point,      // Only allow one target drop
		Sphere,     // Only allow one target drop
		Cylinder,   // Only allow one target drop
		Cube,       // Only allow one target drop
		Cone        // Only allow one target drop
	}
}