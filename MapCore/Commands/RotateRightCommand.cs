using System;
using System.Linq;

namespace MapCore
{
	public class RotateRightCommand : BaseStampCommand
	{

		public RotateRightCommand()
		{

		}

		public override void Redo(Map map)
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.RotateRight();
		}

		public override void Undo(Map map)
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.RotateLeft();
		}
	}
}
