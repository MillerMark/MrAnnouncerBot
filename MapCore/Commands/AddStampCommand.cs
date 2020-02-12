using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class AddStampCommand : BaseStampCommand
	{

		Guid stampGuid;
		public AddStampCommand()
		{

		}

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);
			if (Data is StampPropertiesData stampData)
			{
				stampGuid = stampData.StampProperties.Guid;
			}
		}

		protected override void ActivateRedo(Map map)
		{
			if (Data is StampPropertiesData stampData)
			{
				map.AddStamp(stampData.StampProperties);
				map.SelectStampsByGuid(stampData.StampProperties.Guid);
			}
		}

		protected override void ActivateUndo(Map map)
		{
			IStampProperties stampFromGuid = map.GetStampFromGuid(stampGuid);
			if (stampFromGuid != null)
			{
				map.RemoveStamp(stampFromGuid);
				map.ClearStampSelection();
			}
		}
	}
}
