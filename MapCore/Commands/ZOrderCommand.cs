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
		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);
			foreach (IStampProperties stampProperties in map.Stamps)
				SaveValue(stampProperties, stampProperties.ZOrder);
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stampProperties in map.Stamps)
				stampProperties.ZOrder = (int)Math.Round(GetValue(stampProperties));
		}
	}
}
