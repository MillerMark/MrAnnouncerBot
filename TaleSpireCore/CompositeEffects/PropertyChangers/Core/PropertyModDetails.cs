﻿using System;
using System.Linq;
using System.Reflection;

namespace TaleSpireCore
{
	public class PropertyModDetails
	{
		public object instanceToSet;
		public PropertyInfo property;
		public FieldInfo field;
		public string attachedPropertyName;
		public Type attachedPropertyType;
		public PropertyModDetails()
		{

		}

		public void SetInstance(ref object nextInstance)
		{
			instanceToSet = nextInstance;
			if (property != null)
				nextInstance = property.GetValue(nextInstance);
			else if (field != null)
				nextInstance = field.GetValue(nextInstance);
			else
				nextInstance = null;
		}

		public void SetValue(BasePropertyChanger propertyChanger)
		{
			if (property != null)
				property.SetValue(instanceToSet, propertyChanger.GetValue());
			else if (field != null)
				field.SetValue(instanceToSet, propertyChanger.GetValue());
			else
			{
				propertyChanger.TrySetProperty(instanceToSet, attachedPropertyName);
				return;
			}

			//	object value = null;
			//	if (property != null && property.Name.EndsWith("LifeTime"))
			//		value = property.GetValue(instanceToSet);
			//	else if (field != null && field.Name.EndsWith("LifeTime"))
			//		value = field.GetValue(instanceToSet);
			//	Talespire.Log.Warning($"After setting LifeTime, the set value is \"{value}\".");
		}

		public Type GetPropertyType()
		{
			if (property != null)
				return property.PropertyType;
			else if (field != null)
				return field.FieldType;
			else
				return attachedPropertyType;
		}

		public string GetName()
		{
			if (property != null)
				return property.Name;
			else if (field != null)
				return field.Name;
			else
				return attachedPropertyName;
		}

		public bool Found => GetPropertyType() != null;
	}
}
