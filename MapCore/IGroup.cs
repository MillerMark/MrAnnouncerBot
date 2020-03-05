using System;
using System.Linq;
using System.Collections.Generic;
using MapCore;

namespace MapCore
{
	public interface IGroup : IItemProperties
	{
		void Ungroup(List<IItemProperties> ungroupedStamps);
		void Deserialize(SerializedStamp serializedStamp);
		List<IItemProperties> Children { get; set; }
	}
}

