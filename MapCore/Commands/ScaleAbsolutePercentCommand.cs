using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class ScaleAbsolutePercentCommand : BaseStampAbsoluteValueCommand
	{

		public ScaleAbsolutePercentCommand()
		{

		}

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);
			foreach (IStampProperties stampProperties in selectedStamps)
				SaveValue(stampProperties, stampProperties.Scale);
		}

		protected override void ActivateRedo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				stampProperties.SetAbsoluteScaleTo(RedoValue);
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				stampProperties.SetAbsoluteScaleTo(GetValue(stampProperties));
		}
	}
}
