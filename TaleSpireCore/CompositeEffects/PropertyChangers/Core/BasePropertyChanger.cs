using System;
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

		public void ModifyProperty(object instance)
		{
			try
			{
				object instanceToSet = null;

				string[] split = Name.Split('.');
				object nextInstance = instance;
				PropertyInfo property = null;
				FieldInfo field = null;
				foreach (string propertyName in split)
				{
					field = null;
					property = null;

					//Talespire.Log.Debug($"propertyName = \"{propertyName}\"...");

					if (propertyName.StartsWith("<") && propertyName.EndsWith(">"))
					{
						string componentTypeName = propertyName.Substring(1, propertyName.Length - 2);
						//Talespire.Log.Debug($"Getting <\"{componentTypeName}\">...");
						if (nextInstance is GameObject gameObject)
						{
							nextInstance = gameObject.GetComponent(componentTypeName);
							if (nextInstance == null)
							{
								//Talespire.Log.Error($"Component \"{componentTypeName}\" not found in instance.");
								return;
							}
							continue;
						}
					}

					//Talespire.Log.Debug($"property = nextInstance.GetType().GetProperty(\"{propertyName}\");");
					property = nextInstance.GetType().GetProperty(propertyName);
					if (property == null)
					{
						//Talespire.Log.Debug($"property not found. Trying field...");
						field = nextInstance.GetType().GetField(propertyName);
						if (field == null)
						{
							//Talespire.Log.Debug($"field not found. Trying TrySetProperty...");
							bool propertySet = TrySetProperty(nextInstance, propertyName);

							if (propertySet)
								continue;

							//Talespire.Log.Error($"Property/Field \"{propertyName}\" not found in instance!");
							return;
						}
					}
					instanceToSet = nextInstance;
					//Talespire.Log.Debug($"nextInstance = property.GetValue(nextInstance);");
					if (property != null)
						nextInstance = property.GetValue(nextInstance);
					else if (field != null)
						nextInstance = field.GetValue(nextInstance);
					else
						nextInstance = null;

					//if (nextInstance == null)
					//	Talespire.Log.Debug($"nextInstance is null!");
				}

				//Talespire.Log.Debug($"Setting {Name} to {GetValue()}...");
				if (property != null)
					property.SetValue(instanceToSet, GetValue());
				else if (field != null)
					field.SetValue(instanceToSet, GetValue());
			}
			catch (Exception ex)
			{
				Talespire.Log.Error($"Error while modifying property {Name}!");
				Talespire.Log.Exception(ex);
			}
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
