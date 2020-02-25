using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public abstract class BaseStampIntValueCommand : BaseStampCommand
	{
		Dictionary<Guid, int> undoValues = new Dictionary<Guid, int>();

		public int RedoValue { get; set; }
		public string PropertyName { get; set; }

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);
			if (!(Data is EnumChangeData enumChangeData))
				return;
			PropertyName = enumChangeData.PropertyName;
		}

		protected void SaveValue(IStampProperties stampProperties, int value)
		{
			if (undoValues.ContainsKey(stampProperties.Guid))
				undoValues[stampProperties.Guid] = value;
			else
				undoValues.Add(stampProperties.Guid, value);
		}

		protected int GetSavedValue(IStampProperties stampProperties)
		{
			if (!undoValues.ContainsKey(stampProperties.Guid))
			{
				System.Diagnostics.Debugger.Break();
				return 0;
			}

			return undoValues[stampProperties.Guid];
		}

		public override void Redo(Map map)
		{
			if (Data is EnumChangeData enumChangeData)
			{
				RedoValue = enumChangeData.Value;
				PropertyName = enumChangeData.PropertyName;
			}

			base.Redo(map);
		}
	}
}
