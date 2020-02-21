using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public abstract class BaseStampBooleanValueCommand : BaseStampCommand
	{
		Dictionary<Guid, bool> undoValues = new Dictionary<Guid, bool>();

		public bool RedoValue { get; set; }
		public string PropertyName { get; set; }

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);
			if (!(Data is BoolChangeData boolChangeData))
				return;
			PropertyName = boolChangeData.PropertyName;
		}

		protected void SaveValue(IStampProperties stampProperties, bool value)
		{
			if (undoValues.ContainsKey(stampProperties.Guid))
				undoValues[stampProperties.Guid] = value;
			else
				undoValues.Add(stampProperties.Guid, value);
		}

		protected bool GetSavedValue(IStampProperties stampProperties)
		{
			if (!undoValues.ContainsKey(stampProperties.Guid))
			{
				System.Diagnostics.Debugger.Break();
				return false;
			}

			return undoValues[stampProperties.Guid];
		}

		public override void Redo(Map map)
		{
			if (Data is BoolChangeData boolChangeData)
			{
				RedoValue = boolChangeData.Value;
				PropertyName = boolChangeData.PropertyName;
			}

			base.Redo(map);
		}
	}
}
