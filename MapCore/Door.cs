using System;
using System.Linq;

namespace MapCore
{
	public class Door
	{
		public const int Span = 160;
		public const int Thickness = 94;
		public DoorPosition Position { get; set; }
		public bool Open { get; set; }
		public bool Locked { get; set; }
		public bool Lockable { get; set; }
		public bool Secret { get; set; }
		public bool Known { get; set; }
		public int HitPoints { get; set; }
		// TODO: Consider adding resistances and vulnerabilities.
		public int ArmorClass { get; set; }
		public Door()
		{
			Position = DoorPosition.Left;
			Open = false;
			Locked = false;
			Lockable = false;
			Secret = false;
			Known = true;
			HitPoints = 18;
			ArmorClass = 15;
		}

		public Door(DoorPosition position) : base()
		{
			Position = position;
		}

		public void MakeSecret()
		{
			Secret = true;
			Known = false;
		}
	}
}
