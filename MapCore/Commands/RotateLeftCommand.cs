using System;
using System.Linq;

namespace MapCore
{
	public class RotateLeftCommand : BaseStampCommand
	{

		public RotateLeftCommand()
		{

		}

		protected override void ActivateRedo(Map map)
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.RotateLeft();
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.RotateRight();
		}
	}
}
