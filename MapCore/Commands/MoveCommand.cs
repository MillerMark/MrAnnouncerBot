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

		public override void Redo(Map map)
		{
			if (!(Data is MoveData moveData))
				return;

			foreach (IStampProperties stamp in SelectedStamps)
				stamp.Move(moveData.DeltaX, moveData.DeltaY);
		}

		public override void Undo(Map map)
		{
			if (!(Data is MoveData moveData))
				return;

			foreach (IStampProperties stamp in SelectedStamps)
				stamp.Move(-moveData.DeltaX, -moveData.DeltaY);
		}
	}
}
