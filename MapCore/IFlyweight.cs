using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public interface IFlyweight
	{
		// Needed for serialization.
		Guid Guid { get; set; }
		
		// Called after deserialization...
		void Reconstitute();
	}
}
