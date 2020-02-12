using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class HueChangeCommand : BaseStampAbsoluteValueCommand
	{

		public HueChangeCommand()
		{

		}

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);
			foreach (IStampProperties stampProperties in selectedStamps)
				SaveValue(stampProperties, stampProperties.HueShift);
		}

		protected override void ActivateRedo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				stampProperties.HueShift = RedoValue;
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				stampProperties.HueShift = GetValue(stampProperties);
		}
	}
}
