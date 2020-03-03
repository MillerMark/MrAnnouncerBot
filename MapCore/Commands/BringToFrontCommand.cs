using System;
using System.Linq;

namespace MapCore
{
	public class BringToFrontCommand : ZOrderCommand
	{

		protected override void ActivateRedo(Map map)
		{
			map.RemoveAllStamps(SelectedItems);
			map.SortStampsByZOrder();
			map.NormalizeZOrder();
			map.AddStamps(SelectedItems);
		}

		public BringToFrontCommand()
		{

		}
	}
}
