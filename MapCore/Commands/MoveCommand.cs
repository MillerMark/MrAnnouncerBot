using System;
using System.Collections.Generic;
using System.Linq;

namespace MapCore
{
	public class MoveCommand : BaseStampCommand
	{

		public MoveCommand()
		{

		}

		protected override void ActivateRedo(Map map)
		{
			if (!(Data is MoveData moveData))
				return;

			foreach (IStampProperties stamp in SelectedStamps)
				stamp.Move(moveData.DeltaX, moveData.DeltaY);
		}

		protected override void ActivateUndo(Map map)
		{
			if (!(Data is MoveData moveData))
				return;

			foreach (IStampProperties stamp in SelectedStamps)
				stamp.Move(-moveData.DeltaX, -moveData.DeltaY);
		}
	}
}
