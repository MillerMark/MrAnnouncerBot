using System;
using System.Linq;

namespace MapCore
{
	public class ScaleCommand : BaseStampCommand
	{

		public ScaleCommand()
		{

		}

		protected override void ActivateRedo(Map map)
		{
			if (!(Data is ScaleData scaleData))
				return;
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.AdjustScale(scaleData.ScaleFactor);
		}

		protected override void ActivateUndo(Map map)
		{
			if (!(Data is ScaleData scaleData))
				return;
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.AdjustScale(1 / scaleData.ScaleFactor);
		}
	}
}
