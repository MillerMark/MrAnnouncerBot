using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MapCore;

namespace DndMapSpike
{
	public class PropertyValueComparer
	{
		public List<PropertyValueData> Comparisons { get; set; } = new List<PropertyValueData>();
		public PropertyValueComparer()
		{

		}
		PropertyValueData GetComparison(string name)
		{
			return Comparisons.FirstOrDefault(x => x.Name == name);
		}

		void CreateComparison(object instance, PropertyInfo propertyInfo, DesignTimePropertyAttributes propertyAttributes)
		{
			PropertyValueData propertyValueData = new PropertyValueData(instance, propertyInfo, propertyAttributes.DisplayText, propertyAttributes.NumDigits, propertyAttributes.DependentProperty);
			propertyValueData.FirstInstance = instance;
			Comparisons.Add(propertyValueData);
		}

		DesignTimePropertyAttributes GetDesignTimePropertyAttributes(PropertyInfo propertyInfo)
		{
			DesignTimePropertyAttributes result = new DesignTimePropertyAttributes();

			PrecisionAttribute numDigitsAttribute = propertyInfo.GetCustomAttribute<PrecisionAttribute>();
			if (numDigitsAttribute != null)
				result.NumDigits = numDigitsAttribute.NumDecimalPlaces;

			DisplayTextAttribute displayTextAttribute = propertyInfo.GetCustomAttribute<DisplayTextAttribute>();
			if (displayTextAttribute != null)
				result.DisplayText = displayTextAttribute.DisplayText;

			DependentPropertyAttribute dependentPropertyAttribute = propertyInfo.GetCustomAttribute<DependentPropertyAttribute>();
			if (dependentPropertyAttribute != null)
				result.DependentProperty = dependentPropertyAttribute.DependentProperty;

			return result;
		}

		public void Compare(object stampProperties)
		{
			PropertyInfo[] properties = stampProperties.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!ReflectionHelper.IsEditable(propertyInfo))
					continue;

				DesignTimePropertyAttributes designTimePropertyAttributes = GetDesignTimePropertyAttributes(propertyInfo);

				PropertyValueData existingComparison = GetComparison(propertyInfo.Name);
				if (existingComparison == null)
					CreateComparison(stampProperties, propertyInfo, designTimePropertyAttributes);
				else
					existingComparison.Compare(stampProperties, propertyInfo);
			}
		}
	}
}

