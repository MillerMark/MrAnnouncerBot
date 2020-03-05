using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class StampGroupingCommand : BaseStampCommand
	{
		public StampGroupingCommand()
		{

		}

		void GroupSelection(Map map)
		{
			// ![](AD557BE1AE706B2524FC0A86843645DC.png) CodeBase Alpha!!!
			List<IItemProperties> stampsToGroup = new List<IItemProperties>();
			stampsToGroup.AddRange(SelectedItems);
			map.RemoveAllStamps(SelectedItems);
			IGroup stampGroup = map.CreateGroup(stampsToGroup);
			map.AddItem(stampGroup);
			map.SortStampsByZOrder();
			map.NormalizeZOrder();
			ClearSelectedStamps(map);
			AddSelectedItem(map, stampGroup);
		}

		void UngroupSelection(Map map)
		{
			List<UngroupedStamps> ungroupedStamps = new List<UngroupedStamps>();
			foreach (IItemProperties stamp in SelectedItems)
			{
				if (stamp is IGroup stampGroup && !stampGroup.Locked)
				{
					UngroupedStamps ungroupedStampsThisGroup = new UngroupedStamps(stampGroup);
					ungroupedStampsThisGroup.StartingZOrder = stampGroup.ZOrder;
					stampGroup.Ungroup(ungroupedStampsThisGroup.Stamps);
					ungroupedStamps.Add(ungroupedStampsThisGroup);
					map.RemoveItem(stampGroup);
				}
			}

			List<UngroupedStamps> sortedUngroups = ungroupedStamps.OrderByDescending(x => x.StartingZOrder).ToList();

			foreach (UngroupedStamps ungroup in sortedUngroups)
			{
				RemoveSelectedItem(map, ungroup.StampGroup);
				foreach (IItemProperties stamp in ungroup.Stamps)
				{
					AddSelectedItem(map, stamp);

					map.AddItem(stamp);
				}
			}
		}

		protected override void ActivateRedo(Map map)
		{
			if (Data is GroupOperation operation)
			{
				GroupOperation = operation;
				switch (operation)
				{
					case GroupOperation.Group:
						GroupSelection(map);
						break;
					case GroupOperation.Ungroup:
						UngroupSelection(map);
						break;
				}
			}
		}

		protected override void ActivateUndo(Map map)
		{
			if (Data is GroupOperation operation)
			{
				GroupOperation = operation;
				switch (operation)
				{
					case GroupOperation.Group:
						UngroupSelection(map);
						break;
					case GroupOperation.Ungroup:
						GroupSelection(map);
						break;
				}
			}
		}
		public GroupOperation GroupOperation { get; set; }
	}
}
