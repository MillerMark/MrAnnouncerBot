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

		T GetValue(object instance, string propertyName)
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyName);
			if (property == null)
				return default;
			return (T)property.GetValue(instance);
		}

		void SetValue(object instance, string propertyName, T value)
		{
			PropertyInfo property = instance.GetType().GetProperty(propertyName);
			if (property == null)
				return;
			property.SetValue(instance, value);
		}

		protected override void PrepareForExecution(Map map)
		{
			base.PrepareForExecution(map);

			foreach (IItemProperties itemProperties in SelectedItems)
				SaveValue(itemProperties, GetValue(itemProperties, PropertyName));
		}

		protected override void ActivateRedo(Map map)
		{
			foreach (IItemProperties itemProperties in SelectedItems)
				SetValue(itemProperties, PropertyName, RedoValue);
		}

		protected override void ActivateUndo(Map map)
		{
			foreach (IItemProperties itemProperties in SelectedItems)
				SetValue(itemProperties, PropertyName, GetSavedValue(itemProperties));
		}
	}
}
