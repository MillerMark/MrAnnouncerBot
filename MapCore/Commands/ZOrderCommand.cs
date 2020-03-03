using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public abstract class ZOrderCommand : BaseStampAbsoluteValueCommand
	{

		public ZOrderCommand()
		{

		}
		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);
			foreach (IItemProperties stampProperties in map.Stamps)
				SaveValue(stampProperties, stampProperties.ZOrder);
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IItemProperties stampProperties in map.Stamps)
				stampProperties.ZOrder = GetSavedInt(stampProperties);
		}
	}
}
