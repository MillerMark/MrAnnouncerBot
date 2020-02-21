using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class SaturationChangeCommand : BaseStampAbsoluteValueCommand
	{

		public SaturationChangeCommand()
		{

		}

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);
			foreach (IStampProperties stampProperties in selectedStamps)
				SaveValue(stampProperties, stampProperties.Saturation);
		}

		protected override void ActivateRedo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				stampProperties.Saturation = RedoValue;
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				stampProperties.Saturation = GetSavedValue(stampProperties);
		}
	}
}
