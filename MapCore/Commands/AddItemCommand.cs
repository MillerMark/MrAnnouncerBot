using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class AddItemCommand : BaseStampCommand
	{

		Guid stampGuid;
		public AddItemCommand()
		{

		}

		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);
			if (Data is ItemPropertiesData stampData)
			{
				stampGuid = stampData.ItemProperties.Guid;
			}
		}

		protected override void ActivateRedo(Map map)
		{
			if (Data is ItemPropertiesData stampData)
			{
				map.AddItem(stampData.ItemProperties);
				map.SelectItemsByGuid(stampData.ItemProperties.Guid);
			}
		}

		protected override void ActivateUndo(Map map)
		{
			IItemProperties stampFromGuid = map.GetItemFromGuid(stampGuid);
			if (stampFromGuid != null)
			{
				map.RemoveItem(stampFromGuid);
				map.ClearStampSelection();
			}
		}
	}
}
