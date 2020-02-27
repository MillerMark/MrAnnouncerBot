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
			List<IStampProperties> copiedStamps = new List<IStampProperties>();

			List<IStampProperties> zOrderedSelection = map.SelectedStamps.OrderBy(x => x.ZOrder).ToList();
			foreach (IStampProperties stamp in zOrderedSelection)
			{
				IStampProperties newStamp = stamp.Copy(deltaX, deltaY);
				if (newStamp != null)
				{
					map.AddItem(newStamp);
					copiedStamps.Add(newStamp);
				}
			}

			originalStampSelection.AddRange(map.SelectedStamps.Select(stampProperties => stampProperties.Guid));
			SetSelectedStamps(map, copiedStamps);
		}

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);

		}

		protected override void ActivateRedo(Map map)
		{
			if (!(Data is MoveData moveData))
				return;

			Copy(map, moveData.DeltaX, moveData.DeltaY);
		}
		protected override void ActivateUndo(Map map)
		{
			map.RemoveAllStamps(SelectedStamps);
			map.SelectedStamps.Clear();
			SetSelectedStamps(map, originalStampSelection);
		}
	}
}
