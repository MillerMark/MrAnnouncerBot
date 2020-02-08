using System;
using System.Linq;

namespace MapCore
{
	public class ScaleCommand : BaseStampCommand
	{

		public ScaleCommand()
		{

		}

		public override void Redo(Map map)
		{
			if (!(Data is ScaleData scaleData))
				return;
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.AdjustScale(scaleData.ScaleFactor);
		}

		public override void Undo(Map map)
		{
			if (!(Data is ScaleData scaleData))
				return;
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.AdjustScale(1 / scaleData.ScaleFactor);
		}
	}
}
