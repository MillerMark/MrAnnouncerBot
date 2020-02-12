using System;
using System.Linq;

namespace MapCore
{
	public class DeleteStampsCommand : StampRestoreBaseCommand
	{

		public DeleteStampsCommand()
		{
			ClearSelectionAfterRedo = true;
		}

		protected override void ActivateRedo(Map map)
		{
			map.RemoveAllStamps(SelectedStamps);
		}
	}
}
