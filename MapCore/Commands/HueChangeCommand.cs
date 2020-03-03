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

		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);
			foreach (IStampProperties stampProperties in SelectedStamps)
					SaveValue(stampProperties, stampProperties.HueShift);
		}

		protected override void ActivateRedo(Map map)
		{
			foreach (IItemProperties itemProperties in SelectedItems)
				if (itemProperties is IStampProperties stampProperties)
					stampProperties.HueShift = RedoValue;
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IItemProperties itemProperties in SelectedItems)
				if (itemProperties is IStampProperties stampProperties)
					stampProperties.HueShift = GetSavedValue(stampProperties);
		}
	}
}
