using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TaleSpireCore
{
	public class MiniGrouperData
	{
		public bool Hidden { get; set; }
		public bool Flying { get; set; }
		public int BaseIndex { get; set; } = -1;
		public int RingHue { get; set; }
		public List<string> Members { get; set; } = new List<string>();
		public MiniGrouperData()
		{

		}
	}
}
