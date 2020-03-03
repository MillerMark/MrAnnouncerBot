using System;
using System.Collections.Generic;
using System.Linq;

namespace MapCore
{
	// TODO: Check this ancestry. IStampProperties feels way too specific.
	public interface IItemGroup : IStampProperties
	{
		void Ungroup(List<IItemProperties> ungroupedStamps);
	}
}
