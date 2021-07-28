using System;
using System.Linq;

namespace DndCore
{
	public class WhatSideEventArgs : EventArgs
	{
		public WhatSideEventArgs(WhatSide whatSide)
		{
			WhatSide = whatSide;
		}

		public WhatSide WhatSide { get; set; }
	}
}
