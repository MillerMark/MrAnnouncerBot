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

		void CreateComparison(IStampProperties instance, PropertyInfo propertyInfo, string displayText)
		{
			PropertyValueData propertyValueData = new PropertyValueData(instance, propertyInfo, displayText);
			propertyValueData.FirstInstance = instance;
			Comparisons.Add(propertyValueData);
		}

		public void Compare(IStampProperties stampProperties)
		{
			PropertyInfo[] properties = stampProperties.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				foreach (Attribute attribute in propertyInfo.GetCustomAttributes())
				{
					if (attribute is EditablePropertyAttribute editablePropertyAttribute)
					{
						PropertyValueData existingComparison = GetComparison(propertyInfo.Name);
						if (existingComparison == null)
							CreateComparison(stampProperties, propertyInfo, editablePropertyAttribute.DisplayText);
						else
							existingComparison.Compare(stampProperties, propertyInfo);
					}
				}
			}
		}
	}
}

