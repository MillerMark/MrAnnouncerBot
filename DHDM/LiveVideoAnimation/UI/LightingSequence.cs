using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace DHDM
{
	public class LightingSequence
	{
		public List<Light> Lights { get; set; } = new List<Light>();

		public LightingSequence()
		{

		}
		public LightingSequence Compress()
		{
			LightingSequence result = new LightingSequence();
			foreach (Light light in Lights)
				result.Lights.Add(light.Compress());
			return result;
		}

		public LightingSequence Decompress()
		{
			LightingSequence result = new LightingSequence();
			foreach (Light light in Lights)
				result.Lights.Add(light.Decompress());
			return result;
		}
	}
}
