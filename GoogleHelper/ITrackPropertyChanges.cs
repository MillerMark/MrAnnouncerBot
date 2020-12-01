using System;
using System.Linq;
using System.Collections.Generic;

namespace GoogleHelper
{
	public interface ITrackPropertyChanges
	{
		List<string> ChangedProperties { get; set; }
		bool IsDirty { get; set; }
	}
}
