﻿using System;
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
			List<IStampProperties> stampsToGroup = new List<IStampProperties>();
			stampsToGroup.AddRange(SelectedStamps);
			map.RemoveAllStamps(SelectedStamps);
			IStampGroup stampGroup = map.CreateGroup(stampsToGroup);
			map.AddStamp(stampGroup);
			map.SortStampsByZOrder();
			map.NormalizeZOrder();
			ClearSelectedStamps(map);
			AddSelectedStamp(map, stampGroup);
		}

		void UngroupSelection(Map map)
		{
			List<UngroupedStamps> ungroupedStamps = new List<UngroupedStamps>();
			foreach (IStampProperties stamp in SelectedStamps)
			{
				if (stamp is IStampGroup stampGroup)
				{
					UngroupedStamps ungroupedStampsThisGroup = new UngroupedStamps(stampGroup);
					ungroupedStampsThisGroup.StartingZOrder = stampGroup.ZOrder;
					stampGroup.Ungroup(ungroupedStampsThisGroup.Stamps);
					ungroupedStamps.Add(ungroupedStampsThisGroup);
					map.RemoveStamp(stampGroup);
				}
			}

			List<UngroupedStamps> sortedUngroups = ungroupedStamps.OrderByDescending(x => x.StartingZOrder).ToList();

			foreach (UngroupedStamps ungroup in sortedUngroups)
			{
				RemoveSelectedStamp(map, ungroup.StampGroup);
				foreach (IStampProperties stamp in ungroup.Stamps)
				{
					AddSelectedStamp(map, stamp);

					map.AddStamp(stamp);
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
