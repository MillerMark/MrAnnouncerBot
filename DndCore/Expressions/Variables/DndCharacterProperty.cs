using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class DndCharacterProperty : DndPropertyAccessor
	{
		public delegate void AskValueEventHandler(object sender, AskValueEventArgs ea);
		public static event AskValueEventHandler AskingValue;
		private static AskValueEventArgs askValueEventArgs;
		
		object AskValue(Creature player, string caption, object currentValue, string memberName, string memberTypeName)
		{
			if (askValueEventArgs == null)
				askValueEventArgs = new AskValueEventArgs(player, caption, memberName, memberTypeName, currentValue);
			else
			{
				askValueEventArgs.Player = player;
				askValueEventArgs.Caption = caption;
				askValueEventArgs.MemberName = memberName;
				askValueEventArgs.MemberTypeName = memberTypeName;
				askValueEventArgs.Value = currentValue;
			}

			AskingValue?.Invoke(this, askValueEventArgs);
			return askValueEventArgs.Value;
		}

		void CheckValue(Creature player, MemberInfo member, ref object value)
		{
			AskAttribute askAttr = member?.Get<AskAttribute>();
			if (askAttr == null)
				return;

			string askCaption = askAttr.Caption;
			string memberType = null;
			if (member is PropertyInfo propInfo)
				memberType = propInfo.PropertyType.Name;
			else if (member is FieldInfo fieldInfo)
				memberType = fieldInfo.FieldType.Name;

			value = AskValue(player, askCaption, value, member.Name, memberType);
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Creature player)
		{
			if (fieldNames.IndexOf(variableName) >= 0)
			{
				FieldInfo field = typeof(Character).GetField(variableName);

				object value = field?.GetValue(player);
				CheckValue(player, field, ref value);
				return value;
			}

			if (propertyNames.IndexOf(variableName) >= 0)
			{
				PropertyInfo property = typeof(Character).GetProperty(variableName);

				object value = property?.GetValue(player);
				CheckValue(player, property, ref value);
				return value;
			}

			return player.GetState(variableName);
		}

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			/* We don't use a prefix for the active character's properties. */
			return AddPropertiesAndFields<Character>(null, "Active Character", false /* usePrefix */);
		}

		public override bool Handles(string tokenName, Creature player, CastedSpell castedSpell)
		{
			if (!(player is Character))
				return false;

			if (Handles<Character>(tokenName, false))  // No prefix for active character's properties.
				return true;

			return player.HasState(tokenName) || tokenName.StartsWith("_");
		}
	}
}

