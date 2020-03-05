using System;
using System.Linq;
using System.Collections.Generic;
using MapCore;

namespace MapCore
{
	public interface IGroup : IItemProperties
	{
		void Ungroup(List<IItemProperties> ungroupedStamps);
		List<IItemProperties> Children { get; set; }
	}
}

