using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MapCore
{
	public class ChangeIntPropertyCommand : BaseStampIntValueCommand
	{
		public ChangeIntPropertyCommand()
		{

		}

		int GetIntValue(IStampProperties instance, string propertyName)
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyName);
			if (property == null)
				return 0;
			return (int)property.GetValue(instance);
		}

		void SetIntValue(IStampProperties instance, string propertyName, int value)
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyName);
			if (property == null)
				return;
			property.SetValue(instance, value);
		}

		protected override void PrepareForExecution(Map map, List<IStampProperties> selectedStamps)
		{
			base.PrepareForExecution(map, selectedStamps);

			foreach (IStampProperties stampProperties in selectedStamps)
				SaveValue(stampProperties, GetIntValue(stampProperties, PropertyName));
		}

		protected override void ActivateRedo(Map map)
		{

			foreach (IStampProperties stampProperties in SelectedStamps)
				SetIntValue(stampProperties, PropertyName, RedoValue);
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				SetIntValue(stampProperties, PropertyName, GetSavedValue(stampProperties));
		}
	}
}
