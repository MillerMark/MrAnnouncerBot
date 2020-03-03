using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class ContrastChangeCommand : BaseStampAbsoluteValueCommand
	{

		public ContrastChangeCommand()
		{

		}

		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);
			foreach (IStampProperties stampProperties in SelectedStamps)
				SaveValue(stampProperties, stampProperties.Contrast);
		}

		protected override void ActivateRedo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				stampProperties.Contrast = RedoValue;
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				stampProperties.Contrast = GetSavedValue(stampProperties);
		}
	}
}
