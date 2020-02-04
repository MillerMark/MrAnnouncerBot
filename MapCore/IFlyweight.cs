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
		string TypeName { get; set; }


		// Called before serialization...
		void PrepareForSerialization();

		// Called after deserialization...
		void Reconstitute();
	}
}
