using CodingSeb.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DndCore
{
	public class DndEnumValue<TEnum> : DndVariable where TEnum : struct
	{
		public bool HasElement(string tokenName)
		{
			return Enum.TryParse(tokenName, out TEnum _);
		}

		public object GetValue(string variableName)
		{
			if (Enum.TryParse(variableName, out TEnum result))
				return result;
			return null;
		}

		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			return HasElement(tokenName);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Character player)
		{
			return GetValue(variableName);
		}

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			List<PropertyCompletionInfo> result = new List<PropertyCompletionInfo>();
			foreach (TEnum element in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
			{
				string elementName = Enum.GetName(typeof(TEnum), element);
				string elementDescription;
				MemberInfo[] enumElementMembers = typeof(TEnum).GetMember(elementName);
				MemberInfo elementInfo = enumElementMembers.FirstOrDefault(x => x.DeclaringType == typeof(TEnum));
				DescriptionAttribute descriptionAttribute = elementInfo.GetCustomAttribute<DescriptionAttribute>();
				if (descriptionAttribute != null)
					elementDescription = descriptionAttribute.Description;
				else
					elementDescription = $"{typeof(TEnum).Name}.{elementName}";
				result.Add(new PropertyCompletionInfo() { Name = elementName, Description = elementDescription, EnumTypeName = typeof(TEnum).Name, Type = ExpressionType.@enum });
			}
			return result;
		}
	}
}

