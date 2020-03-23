using System;
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
				SerializedItem serializedStamp = SerializedItem.From(stampProperties);
				storedStamps.Add(JsonConvert.SerializeObject(serializedStamp));
			}
		}

		List<Guid> DeserializeStoredStamps(Map map)
		{
			List<Guid> guidsRestored = new List<Guid>();
			List<SerializedItem> serializedStamps = new List<SerializedItem>();
			foreach (string storedStamp in storedStamps)
			{
				SerializedItem serializedStamp = JsonConvert.DeserializeObject<SerializedItem>(storedStamp);
				serializedStamps.Add(serializedStamp);
				guidsRestored.Add(serializedStamp.Guid);
			}
			map.ReconstituteItems(map.Items, serializedStamps);
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
