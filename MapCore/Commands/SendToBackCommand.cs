using System;
using System.Linq;

namespace MapCore
{
	public class SendToBackCommand : ZOrderCommand
	{

		protected override void ActivateRedo(Map map)
		{
			map.RemoveAllStamps(SelectedStamps);
			map.SortStampsByZOrder(SelectedStamps.Count);
			map.NormalizeZOrder(SelectedStamps.Count);
			map.InsertStamps(0, SelectedStamps);
		}

		public SendToBackCommand()
		{

		}
	}
}
