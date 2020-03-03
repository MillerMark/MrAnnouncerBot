using System;
using System.Linq;

namespace MapCore
{
	public class SendToBackCommand : ZOrderCommand
	{

		protected override void ActivateRedo(Map map)
		{
			map.RemoveAllStamps(SelectedItems);
			map.SortStampsByZOrder(SelectedItems.Count);
			map.NormalizeZOrder(SelectedItems.Count);
			map.InsertStamps(0, SelectedItems);
		}

		public SendToBackCommand()
		{

		}
	}
}
