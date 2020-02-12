using System;
using System.Linq;

namespace MapCore
{
	public class RotateRightCommand : BaseStampCommand
	{

		public RotateRightCommand()
		{

		}

		protected override void ActivateRedo(Map map)
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.RotateRight();
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.RotateLeft();
		}
	}
}
