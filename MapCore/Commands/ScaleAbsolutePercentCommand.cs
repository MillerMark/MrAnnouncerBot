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

		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);
			foreach (IStampProperties stampProperties in SelectedStamps)
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
				stampProperties.SetAbsoluteScaleTo(GetSavedValue(stampProperties));
		}
	}
}
