using System;
using System.Linq;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	/// <summary>
	/// This actually supports two kinds of properties. Those defined in the Google Sheet Dnd - Properties, 
	/// **and** those set at runtime.
	/// </summary>
	public class DndProperty : DndVariable
	{
		public override bool Handles(string tokenName, Creature player, CastedSpell castedSpell)
		{
			PropertyDto property = AllProperties.Get(tokenName);
			if (property != null)
				return true;

			if (globalKnownProps.ContainsKey(tokenName))
				return true;

			return false;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Creature player)
		{
			PropertyDto property = AllProperties.Get(variableName);
			if (property != null)
				return evaluator.Evaluate(property.Expression);
			if (globalKnownProps.ContainsKey(variableName))
				return globalKnownProps[variableName];
			return null;
		}

		// HACK: Concerned about storing these separately. Should they be stored with the transfered spell instead.
		static Dictionary<string, object> globalKnownProps = new Dictionary<string, object>();

		public static void GlobalSet(string propertyName, object value)
		{
			if (globalKnownProps.ContainsKey(propertyName))
				globalKnownProps[propertyName] = value;
			else
				globalKnownProps.Add(propertyName, value);
		}

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			List<PropertyCompletionInfo> result = new List<PropertyCompletionInfo>();
			foreach (string key in globalKnownProps.Keys)
				result.Add(new PropertyCompletionInfo() { Name = key, Description = $"Known Property: {key}" });

			foreach (PropertyDto propertyDto in AllProperties.Properties)
				result.Add(new PropertyCompletionInfo() { Name = propertyDto.Name, Description = propertyDto.Description, Type = propertyDto.Type });
			return result;
		}
	}
}

