using System;
using System.Linq;

namespace MapCore
{
	public class HorizontalFlipCommand : BaseFlipCommand
	{

		public HorizontalFlipCommand()
		{

		}

		protected override void FlipSelection()
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.FlipHorizontally = !stamp.FlipHorizontally;
		}
	}
}
