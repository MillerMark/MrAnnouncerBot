using System;
using System.Linq;

namespace MapCore
{
	public class RotateLeftCommand : BaseStampCommand
	{

		public RotateLeftCommand()
		{

		}

		public override void Redo(Map map)
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.RotateLeft();
		}

		public override void Undo(Map map)
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.RotateRight();
		}
	}
}
