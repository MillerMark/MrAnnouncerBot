using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public abstract class BaseStampValueCommand<T> : BaseStampCommand
	{
		Dictionary<Guid, T> undoValues = new Dictionary<Guid, T>();

		public T RedoValue { get; set; }
		public string PropertyName { get; set; }

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);
			if (!(Data is ChangeData<T> textChangeData))
				return;
			PropertyName = textChangeData.PropertyName;
		}

		protected void SaveValue(IStampProperties stampProperties, T value)
		{
			if (undoValues.ContainsKey(stampProperties.Guid))
				undoValues[stampProperties.Guid] = value;
			else
				undoValues.Add(stampProperties.Guid, value);
		}

		protected T GetSavedValue(IStampProperties stampProperties)
		{
			if (!undoValues.ContainsKey(stampProperties.Guid))
			{
				System.Diagnostics.Debugger.Break();
				return default;
			}

			return undoValues[stampProperties.Guid];
		}

		public override void Redo(Map map)
		{
			if (Data is ChangeData<T> textChangeData)
			{
				RedoValue = textChangeData.Value;
				PropertyName = textChangeData.PropertyName;
			}

			base.Redo(map);
		}
	}
}
