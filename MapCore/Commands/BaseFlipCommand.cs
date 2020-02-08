using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseFlipCommand : BaseStampCommand
	{

		public BaseFlipCommand()
		{

		}

		public override void Redo(Map map)
		{
			FlipSelection();
		}

		public override void Undo(Map map)
		{
			FlipSelection();
		}

		protected abstract void FlipSelection();
	}
}
