using System;
using System.Linq;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace TaleSpireCore
{
	public abstract class BasePropertyChanger
	{
		public const string STR_ChangePrefix = "Change";

		public string Name { get; set; }
		public string Value { get; set; }
		public object ValueOverride { get; set; }

		public BasePropertyChanger(string propertyName, string valueStr)
		{
			Name = propertyName;
			Value = valueStr;
		}

		public BasePropertyChanger()
		{

		}

		protected abstract object ParseValue();

		public object GetValue()
		{
			if (ValueOverride != null)
				return ValueOverride;
			return ParseValue();
		}

		public static float GetFloat(string str)
		{
			string trimmedStr = str.Trim('(', ')').Trim();
			if (float.TryParse(trimmedStr, out float result))
				return result;

			Talespire.Log.Error($"Unable to convert \"{trimmedStr}\" to a float.");
			return 0f;
		}

		public void ModifyProperty(object instance, bool logErrors = false)
		{
			try
			{
				PropertyModDetails propertyModDetails = GetPropertyModDetails(instance, Name, logErrors);

				if (Name.EndsWith("LifeTime"))
					Talespire.Log.Debug($"Setting {Name} to {GetValue()} (in {GetType().Name})...");
				propertyModDetails.SetValue(this);
			}
			catch (Exception ex)
			{
				Talespire.Log.Error($"Error while modifying property {Name}!");
				Talespire.Log.Exception(ex);
			}
		}

		public void ModifyPropertyWithDetails(PropertyModDetails propertyModDetails)
		{
			try
			{
				Talespire.Log.Debug($"ModifyPropertyWithDetails - propertyModDetails.SetValue(this);");
				propertyModDetails.SetValue(this);
			}
			catch (Exception ex)
			{
				Talespire.Log.Error($"ModifyPropertyWithDetails - Error while modifying property {Name}!");
				Talespire.Log.Exception(ex);
			}
		}

		static bool IsAttachedMaterialProperty(Material material, string propertyName, out Type type)
		{
			type = null;
			int materialProperties = material.shader.GetPropertyCount();
			for (int i = 0; i < materialProperties; i++)
			{
				string shaderPropertyName = material.shader.GetPropertyName(i);
				if (shaderPropertyName == propertyName)
				{
					ShaderPropertyType propertyType = material.shader.GetPropertyType(i);
					switch (propertyType)
					{
						case ShaderPropertyType.Color:
							type = typeof(UnityEngine.Color);
							return true;
						case ShaderPropertyType.Vector:
							type = typeof(Vector3);
							return true;
						case ShaderPropertyType.Float:
							type = typeof(float);
							return true;
						case ShaderPropertyType.Range:
							// TODO: Check this - may not be correct.
							type = typeof(RangeInt);
							return true;
						case ShaderPropertyType.Texture:
							type = typeof(Texture);
							return true;
					}
				}
			}
			return false;
		}

		static bool IsAttachedProperty(object instance, string propertyName, out Type type)
		{
			type = null;
			if (instance is Material material)
			{
				if (IsAttachedMaterialProperty(material, propertyName, out type))
					return true;
			}

			return false;
		}

		public static PropertyModDetails GetPropertyModDetails(object instance, string fullPropertyName, bool logErrors = false)
		{
			bool logDetails = logErrors; // fullPropertyName.EndsWith("LifeTime");
			PropertyModDetails propertyModDetails = new PropertyModDetails();
			propertyModDetails.instance = null;
			if (logDetails)
			{
				Talespire.Log.Debug($"fullPropertyName: {fullPropertyName}");
			}
			string[] split = fullPropertyName.Split('.');
			object nextInstance = instance;
			propertyModDetails.property = null;
			propertyModDetails.field = null;
			propertyModDetails.attachedPropertyName = string.Empty;
			foreach (string propertyName in split)
			{
				propertyModDetails.field = null;
				propertyModDetails.property = null;

				if (logDetails)
					Talespire.Log.Debug($"propertyName = \"{propertyName}\"...");

				if (propertyName.StartsWith("<") && propertyName.EndsWith(">"))
				{
					string componentTypeName = propertyName.Substring(1, propertyName.Length - 2);
					if (logDetails)
						Talespire.Log.Debug($"Getting <\"{componentTypeName}\">...");
					if (nextInstance is GameObject gameObject)
					{
						if (logDetails)
							Talespire.Log.Debug($"nextInstance = gameObject.GetComponent(\"{componentTypeName}\")...");
						nextInstance = gameObject.GetComponent(componentTypeName);
						if (nextInstance == null)
						{
							if (logDetails)
								Talespire.Log.Debug($"Component[] components = gameObject.GetComponents(typeof(Component))...");
							Component[] components = gameObject.GetComponents(typeof(Component));
							if (components != null)
							{
								if (logDetails)
									Talespire.Log.Debug($"foreach (Component component in components)");
								foreach (Component component in components)
								{
									if (logDetails)
										Talespire.Log.Debug($"if (component.GetType().Name == componentTypeName)");
									if (component == null)
										Talespire.Log.Error($"Missing component (most likely a script?)");
									if (component?.GetType().Name == componentTypeName)
									{
										if (logDetails)
											Talespire.Log.Debug($"  Found \"{component.GetType().Name}\" through iteration!");
										nextInstance = component;
										break;
									}
								}
							}

							if (nextInstance == null)
							{
								if (logDetails)
									Talespire.Log.Error($"Component \"{componentTypeName}\" not found in instance.");

								break;
							}
						}
						continue;
					}
				}

				if (logDetails)
					Talespire.Log.Debug($"property = nextInstance.GetType().GetProperty(\"{propertyName}\");");
				propertyModDetails.property = nextInstance.GetType().GetProperty(propertyName);
				if (propertyModDetails.property == null)
				{
					if (logDetails)
						Talespire.Log.Debug($"property not found. Trying field...");
					propertyModDetails.field = nextInstance.GetType().GetField(propertyName);
					if (propertyModDetails.field == null)
					{
						if (logDetails)
							Talespire.Log.Debug($"field not found. Trying TrySetProperty...");
						if (IsAttachedProperty(nextInstance, propertyName, out Type propertyType))
						{
							propertyModDetails.instance = nextInstance;
							propertyModDetails.attachedPropertyName = propertyName;
							propertyModDetails.attachedPropertyType = propertyType;
							break;
						}

						propertyModDetails.instance = null;
						if (logDetails)
							Talespire.Log.Error($"Property/Field \"{propertyName}\" not found in instance!");
						break;
					}
				}
				propertyModDetails.SetInstance(ref nextInstance);
				if (logDetails)
					if (nextInstance == null)
						Talespire.Log.Debug($"nextInstance is null!");
			}
			return propertyModDetails;
		}

		// Descendants can override and return true if successful.
		public virtual bool TrySetProperty(object instance, string propertyName)
		{
			return false;
		}

		public virtual bool CanSetProperty(object instance, string propertyName)
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
