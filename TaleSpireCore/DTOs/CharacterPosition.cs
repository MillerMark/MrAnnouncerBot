using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TaleSpireCore
{
	public class CharacterPosition
	{
		public string Name { get; set; }
		public string ID { get; set; }
		public VectorDto Position { get; set; }
		public float FlyingAltitude { get; set; }
	}
}
