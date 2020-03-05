﻿using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapCore
{
	public abstract class StampRestoreBaseCommand : ZOrderCommand
	{
		protected List<string> storedStamps = new List<string>();

		public StampRestoreBaseCommand()
		{

		}

		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);
			SerializeStamps(SelectedItems);
		}

		void SerializeStamps(List<IItemProperties> selectedStamps)
		{
			storedStamps.Clear();
			foreach (IItemProperties stampProperties in selectedStamps)
			{
				SerializedStamp serializedStamp = SerializedStamp.From(stampProperties);
				storedStamps.Add(JsonConvert.SerializeObject(serializedStamp));
			}
		}

		List<Guid> DeserializeStoredStamps(Map map)
		{
			List<Guid> guidsRestored = new List<Guid>();
			List<SerializedStamp> serializedStamps = new List<SerializedStamp>();
			foreach (string storedStamp in storedStamps)
			{
				SerializedStamp serializedStamp = JsonConvert.DeserializeObject<SerializedStamp>(storedStamp);
				serializedStamps.Add(serializedStamp);
				guidsRestored.Add(serializedStamp.Guid);
			}
			map.ReconstituteItems(map.Stamps, serializedStamps);
			return guidsRestored;
		}

		protected override void ActivateUndo(Map map)
		{
			List<Guid> deserializeStoredStamps = DeserializeStoredStamps(map);
			map.SortStampsByZOrder();
			base.ActivateUndo(map);
			map.SelectItemsByGuid(deserializeStoredStamps);
		}
	}
}
