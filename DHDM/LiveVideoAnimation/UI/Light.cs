using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class Light
	{
		public List<LightSequenceData> SequenceData { get; set; } = new List<LightSequenceData>();
		public string ID { get; set; }
		public Light Compress()
		{
			Light result = new Light() { ID = ID };
			LightSequenceData lastSequenceData = null;
			foreach (LightSequenceData lightSequenceData in SequenceData)
			{
				if (lastSequenceData == null || !lastSequenceData.SameColor(lightSequenceData))
				{
					LightSequenceData clonedSequence = lightSequenceData.Clone();
					result.SequenceData.Add(clonedSequence);
					lastSequenceData = clonedSequence;
				}
				else 
					lastSequenceData.Duration += lightSequenceData.Duration;
			}
			return result;
		}
		public Light Decompress()
		{
			Light result = new Light() { ID = ID };
			foreach (LightSequenceData lightSequenceData in SequenceData)
			{
				result.SequenceData.AddRange(lightSequenceData.Decompress());
			}
			return result;
		}
	}
}
