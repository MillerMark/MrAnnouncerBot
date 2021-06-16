using System;
using System.Linq;

namespace TaleSpireCore
{
	public enum InteractiveTargetingMode
	{
		Creatures,  // Can allow for multiple target drops.
		Point,      // Only allow one target drop
		Sphere,     // Only allow one target drop
		Cylinder,   // Only allow one target drop
		Cube,       // Only allow one target drop
		Cone        // Only allow one target drop
	}
}