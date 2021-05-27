﻿using System;
using System.Linq;
using System.Drawing;
using System.Reflection;
using UnityEngine;

namespace TaleSpireCore
{
	public abstract class BasePropertyChanger
	{
		public const string STR_ChangePrefix = "Change";

		public string Name { get; set; }
		public string Value { get; set; }

		public BasePropertyChanger(string propertyName, string valueStr)
		{
			Name = propertyName;
			Value = valueStr;
		}

		public BasePropertyChanger()
		{

		}

		public abstract object GetValue();

		public static float GetFloat(string str)
		{
			string trimmedStr = str.Trim();
			if (float.TryParse(trimmedStr, out float result))
				return result;

			Talespire.Log.Error($"Unable to convert \"{trimmedStr}\" to a float.");
			return 0f;
		}

		public void ModifyProperty(GameObject effect)
		{
			object instanceToSet = null;

			string[] split = Name.Split('.');
			object nextInstance = effect;
			PropertyInfo property = null;
			FieldInfo field = null;
			foreach (string propertyName in split)
			{
				field = null;
				property = null;

				if (propertyName.StartsWith("<") && propertyName.EndsWith(">"))
				{
					string componentTypeName = propertyName.Substring(1, propertyName.Length - 2);
					if (nextInstance is GameObject gameObject)
					{
						nextInstance = gameObject.GetComponent(componentTypeName);
						if (nextInstance == null)
						{
							Talespire.Log.Error($"Component \"{componentTypeName}\" not found in instance.");
							return;
						}
						continue;
					}
				}

				property = nextInstance.GetType().GetProperty(propertyName);
				if (property == null)
				{
					field = nextInstance.GetType().GetField(propertyName);
					if (field == null)
					{
						// TODO: Check to see if this is a material and there's a shader involved. 
						bool propertySet = TrySetProperty(nextInstance, propertyName);

						if (propertySet)
							continue;

						Talespire.Log.Error($"Property/Field {propertyName} not found in instance!");
						return;
					}
				}
				instanceToSet = nextInstance;
				nextInstance = property.GetValue(nextInstance);
			}
			
			if (property == null && field == null)
			{

				Talespire.Log.Error($"Property/Field {Name} not found in instance!");
				return;
			}

			if (property != null)
				property.SetValue(instanceToSet, GetValue());
			else
				field.SetValue(instanceToSet, GetValue());
		}

		// Descendants can override and return true if successful.
		protected virtual bool TrySetProperty(object instance, string propertyName)
		{
			return false;
		}

		public PropertyChangerDto ToPropertyChangerDto()
		{
			Talespire.Log.Debug("ToPropertyChangerDto()...");
			string typeName = GetType().Name;
			if (typeName.StartsWith(STR_ChangePrefix))
				typeName = typeName.Substring(STR_ChangePrefix.Length);
			Talespire.Log.Debug($"typeName = {typeName}");
			return new PropertyChangerDto() { Name = Name, Value = Value, Type = typeName };
		}
	}
}
