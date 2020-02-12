using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseFlipCommand : BaseStampCommand
	{

		public BaseFlipCommand()
		{

		}

		protected override void ActivateRedo(Map map)
		{
			FlipSelection();
		}

		protected override void ActivateUndo(Map map)
		{
			FlipSelection();
		}

		protected abstract void FlipSelection();
	}
}
