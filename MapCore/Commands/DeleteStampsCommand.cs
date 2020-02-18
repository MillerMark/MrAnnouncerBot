using System;
using System.Linq;

namespace MapCore
{
	public class DeleteStampsCommand : StampRestoreBaseCommand
	{

		public DeleteStampsCommand()
		{
		}

		protected override void ActivateRedo(Map map)
		{
			map.RemoveAllStamps(SelectedStamps);
			map.SelectedStamps.Clear();
		}
	}
}
