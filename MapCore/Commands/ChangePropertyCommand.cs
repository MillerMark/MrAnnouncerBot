using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MapCore
{
	public class ChangePropertyCommand<T> : BaseStampValueCommand<T>
	{
		public ChangePropertyCommand()
		{

		}

		T GetValue(IStampProperties instance, string propertyName)
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyName);
			if (property == null)
				return default;
			return (T)property.GetValue(instance);
		}

		void SetValue(IStampProperties instance, string propertyName, T value)
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
				SaveValue(stampProperties, GetValue(stampProperties, PropertyName));
		}

		protected override void ActivateRedo(Map map)
		{

			foreach (IStampProperties stampProperties in SelectedStamps)
				SetValue(stampProperties, PropertyName, RedoValue);
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				SetValue(stampProperties, PropertyName, GetSavedValue(stampProperties));
		}
	}
}
