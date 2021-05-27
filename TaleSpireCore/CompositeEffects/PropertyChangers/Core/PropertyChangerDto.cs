using System;
using System.Linq;

namespace TaleSpireCore
{
	public class PropertyChangerDto
	{
		
		public string Name { get; set; }    // "<ParticleSystem>.Material._TintColor"
		public string Value { get; set; }		// Can be anything!
		public string Type { get; set; }		// Is the type of the property that we are going to change.
		public PropertyChangerDto()
		{

		}
	}
}
