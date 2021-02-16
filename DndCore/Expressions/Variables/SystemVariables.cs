using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public class SystemVariables : DndPropertyAccessor
	{
		const string VAR_MultiTargetNotificationOffset = "MultiTargetNotificationOffset";
		const string VAR_FriendlyTargets = "FriendlyTargets";
		const string VAR_CardRecipient = "CardRecipient";
		const string VAR_ThisCard = "ThisCard";
		const string VAR_DiceRoll = "DiceRoll";
		public static int Offset = 0;
		public static Target FriendlyTargets = null;
		public static Creature Creature { get; set; }
		public static Target CardRecipient { get; set; }
		public static object ThisCard { get; set; }
		public static DiceRoll DiceRoll { get; set; }


		List<string> KnownVariables = new List<string>();

		public SystemVariables()
		{
			KnownVariables.Add(VAR_MultiTargetNotificationOffset);
			KnownVariables.Add(VAR_FriendlyTargets);
			KnownVariables.Add(VAR_CardRecipient);
			KnownVariables.Add(VAR_ThisCard);
			KnownVariables.Add(VAR_DiceRoll);
		}

		public override bool Handles(string tokenName, Creature creature, CastedSpell castedSpell)
		{
			return Handles<Creature>(tokenName) || KnownVariables.Contains(tokenName);
		}

		void AddCompletionInfo(List<PropertyCompletionInfo> completionInfo, string name, string description, ExpressionType expressionType)
		{
			completionInfo.Add(new PropertyCompletionInfo() { Name = name, Description = description, Type = expressionType });
		}

		public override List<PropertyCompletionInfo> GetCompletionInfo()
		{
			List<PropertyCompletionInfo> completionInfo = AddPropertiesAndFields<Creature>();
			AddCompletionInfo(completionInfo, VAR_MultiTargetNotificationOffset, "The delay offset (in ms) when multiple target events (like Magic's onReceived) are called, typically incremented by about 150ms between each target notification. Can be passed to FloatPlayerText to stagger notification text.", ExpressionType.number);
			AddCompletionInfo(completionInfo, VAR_FriendlyTargets, "A Target instance holding friendly targeted creatures, from a recent call to GetFriendlyTargets().", ExpressionType.unknown);
			AddCompletionInfo(completionInfo, VAR_CardRecipient, "The recipient of a recently gifted card (wrapped in a Target), set when a card is given to a player or an NPC/Monster. Useful for passing as the first parameter to a GiveMagic call.", ExpressionType.unknown);
			AddCompletionInfo(completionInfo, VAR_ThisCard, "The recently received card, set just before calling the card's CardReceived event. Useful for passing as the third parameter to a GiveMagic call.", ExpressionType.unknown);
			AddCompletionInfo(completionInfo, VAR_DiceRoll, "The pending dice roll, set just before rolling the dice.", ExpressionType.unknown);
			
			return completionInfo;
		}

		public override object GetValue(string variableName, ExpressionEvaluator evaluator, Creature player)
		{
			switch (variableName)
			{
				case VAR_MultiTargetNotificationOffset:
					return Offset;
				case VAR_FriendlyTargets:
					return FriendlyTargets;
				case VAR_CardRecipient:
					return CardRecipient;
				case VAR_ThisCard:
					return ThisCard;
				case VAR_DiceRoll:
					return DiceRoll;
			}

			return GetValue<Creature>(variableName, Creature);
		}
	}
}

