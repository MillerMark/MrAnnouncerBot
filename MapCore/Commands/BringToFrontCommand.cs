using System;
using System.Linq;

namespace MapCore
{
	public class BringToFrontCommand : ZOrderCommand
	{

		protected override void ActivateRedo(Map map)
		{
			map.RemoveAllStamps(SelectedStamps);
			map.SortStampsByZOrder();
			map.NormalizeZOrder();
			map.AddStamps(SelectedStamps);
		}

		public BringToFrontCommand()
		{

		}
	}
}
