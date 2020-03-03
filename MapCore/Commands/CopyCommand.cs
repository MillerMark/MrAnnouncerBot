using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class CopyCommand : BaseStampCommand
	{
		List<Guid> originalStampSelection = new List<Guid>();
		public CopyCommand()
		{

		}

		void Copy(Map map, double deltaX, double deltaY)
		{
			List<IItemProperties> copiedStamps = new List<IItemProperties>();

			List<IItemProperties> zOrderedSelection = map.SelectedItems.OrderBy(x => x.ZOrder).ToList();
			foreach (IItemProperties item in zOrderedSelection)
			{
				IItemProperties newStamp = item.Clone(deltaX, deltaY);
				
				if (newStamp != null)
				{
					map.AddItem(newStamp);
					copiedStamps.Add(newStamp);
				}
			}

			originalStampSelection.AddRange(map.SelectedItems.Select(stampProperties => stampProperties.Guid));
			SetSelectedItems(map, copiedStamps);
		}

		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);

		}

		protected override void ActivateRedo(Map map)
		{
			if (!(Data is MoveData moveData))
				return;

			Copy(map, moveData.DeltaX, moveData.DeltaY);
		}
		protected override void ActivateUndo(Map map)
		{
			map.RemoveAllStamps(SelectedItems);
			map.SelectedItems.Clear();
			SetSelectedItems(map, originalStampSelection);
		}
	}
}
