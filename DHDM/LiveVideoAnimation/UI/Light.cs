using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class Light
	{
		public List<LightSequenceData> SequenceData { get; set; } = new List<LightSequenceData>();
		public string ID { get; set; }
	}
}
