using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Property
		{
			public static void ModifyColor(object parent, string childName, string propertyName, string valueStr)
			{
				object instance = GetInstance(parent, childName);
				if (instance == null)
					return;
				Log.Debug($"ModifyColor - ChangeColor changeColor = new ChangeColor(\"{propertyName}\", \"{valueStr}\");");
				ChangeColor changeColor = new ChangeColor(propertyName, valueStr);
				changeColor.ModifyProperty(instance);
			}

			public static void ModifyFloat(object parent, string childName, string propertyName, float value)
			{
				object instance = GetInstance(parent, childName);
				if (instance == null)
					return;
				Log.Debug($"ModifyFloat - ChangeFloat changeFloat = new ChangeFloat(\"{propertyName}\", {value});");
				ChangeFloat changeFloat = new ChangeFloat(propertyName, value.ToString());
				changeFloat.ModifyProperty(instance);
			}

			private static object GetInstance(object parent, string childName)
			{
				if (string.IsNullOrEmpty(childName))
					return parent;
				else if (parent is GameObject gameObject)
				{
					GameObject foundChild = gameObject.FindChild(childName);
					if (foundChild == null)
						Log.Error($"Unable to find child named \"{childName}\".");
					return foundChild;
				}
				return null;
			}

			public static void Modify(object parent, string childName, string propertyName, object value)
			{
				object instance = GetInstance(parent, childName);
				if (instance == null)
				{
					Log.Error($"Modify - Instance not found!");
					return;
				}

				Modify(instance, propertyName, value);
			}

			public static void Modify(object instance, string propertyName, object value, bool logErrors = true)
			{
				Log.Debug($"Modify - {propertyName} to {value}");
				PropertyModDetails propertyModDetails = BasePropertyChanger.GetPropertyModDetails(instance, propertyName, logErrors);
				if (propertyModDetails == null || !propertyModDetails.Found)
				{
					if (logErrors)
						Log.Error($"{propertyName} not found in instance.");
					return;
				}
				BasePropertyChanger propertyChanger = PropertyChangerFactory.CreateFromModDetails(propertyModDetails, logErrors);
				if (propertyChanger != null)
				{
					propertyChanger.ValueOverride = value;
					propertyChanger.ModifyPropertyWithDetails(propertyModDetails);
				}
				else if (logErrors)
					if (propertyModDetails.attachedPropertyType != null)
						Log.Error($"propertyChanger ({propertyName} to {value} of type {propertyModDetails.attachedPropertyType.Name}) is null!");
					else
						Log.Error($"propertyChanger ({propertyName} to {value}) is null!");
			}
		}
	}
}