using System;
using System.Collections.Generic;
using System.Linq;

namespace MapCore
{
	public interface IStampGroup : IStampProperties
	{
		void Ungroup(List<IStampProperties> ungroupedStamps);
	}
}
