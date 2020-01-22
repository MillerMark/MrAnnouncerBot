using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public interface IFlyweight
	{
		// Needed for serialization.
		Guid Guid { get; set; }
		bool NeedsGuid();


		// Called before serialization...
		void PrepareForSerialization();

		// Called after deserialization...
		void Reconstitute();
	}
}
