using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class SystemVariables : DndVariable
	{
		const string VAR_MultiTargetNotificationOffset = "MultiTargetNotificationOffset";
		const string VAR_FriendlyTargets = "FriendlyTargets";
		const string STR_CreaturePrefix = "Creature_";
		public static int Offset = 0;
		public static Target FriendlyTargets = null;
		public static Creature Creature { get; set; }
		static Dictionary<string, PropertyInfo> creatureProperties = new Dictionary<string, PropertyInfo>();


		List<string> KnownVariables = new List<string>();

		static SystemVariables()
		{
			PropertyInfo[] creaturePropertyArray = typeof(Creature).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in creaturePropertyArray)
				creatureProperties.Add(propertyInfo.Name, propertyInfo);
		}

		public SystemVariables()
		{
			KnownVariables.Add(VAR_MultiTargetNotificationOffset);
			KnownVariables.Add(VAR_FriendlyTargets);
		}

		public override bool Handles(string tokenName, Creature creature, CastedSpell castedSpell)
		{
			if (tokenName.StartsWith(STR_CreaturePrefix))
			{
				string propertyName = tokenName.Substring(STR_CreaturePrefix.Length);
				if (creatureProperties.ContainsKey(propertyName))
					return true;
			}
			
			return KnownVariables.Contains(tokenName);
		}

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			return null;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Creature player)
		{
			switch (variableName)
			{
				case VAR_MultiTargetNotificationOffset:
					return Offset;
				case VAR_FriendlyTargets:
					return FriendlyTargets;
			}

			if (variableName.StartsWith(STR_CreaturePrefix))
			{
				string propertyName = variableName.Substring(STR_CreaturePrefix.Length);
				if (creatureProperties.Keys.Contains(propertyName))
					return creatureProperties[propertyName].GetValue(Creature);
			}

			return null;
		}
	}
}

