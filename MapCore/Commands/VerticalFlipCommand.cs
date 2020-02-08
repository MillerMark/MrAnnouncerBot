using System;
using System.Linq;

namespace MapCore
{
	public class VerticalFlipCommand : BaseFlipCommand
	{

		public VerticalFlipCommand()
		{

		}

		protected override void FlipSelection()
		{
			foreach (IStampProperties stamp in SelectedStamps)
				stamp.FlipVertically = !stamp.FlipVertically;
		}
	}
}
