using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MapCore
{
	public class ChangeBooleanPropertyCommand : BaseStampBooleanValueCommand
	{
		public ChangeBooleanPropertyCommand()
		{

		}

		bool GetBoolValue(IStampProperties instance, string propertyName)
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyName);
			if (property == null)
				return false;
			return (bool)property.GetValue(instance);
		}

		void SetBoolValue(IStampProperties instance, string propertyName, bool value)
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
				SaveValue(stampProperties, GetBoolValue(stampProperties, PropertyName));
		}

		protected override void ActivateRedo(Map map)
		{
			
			foreach (IStampProperties stampProperties in SelectedStamps)
				SetBoolValue(stampProperties, PropertyName, RedoValue);
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IStampProperties stampProperties in SelectedStamps)
				SetBoolValue(stampProperties, PropertyName, GetSavedValue(stampProperties));
		}
	}
}
