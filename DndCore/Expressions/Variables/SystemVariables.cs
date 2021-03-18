using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CodingSeb.ExpressionEvaluator;

namespace DndCore
{
	public interface ICard
	{
		string UserName { get; }
		string Id { get; }
		string Guid { get; }
	}
	public class SystemVariables : DndPropertyAccessor
	{
		const string VAR_MultiTargetNotificationOffset = "MultiTargetNotificationOffset";
		const string VAR_FriendlyTargets = "FriendlyTargets";
		const string VAR_CardRecipient = "CardRecipient";
		const string VAR_ThisCard = "ThisCard";
		const string VAR_CardUserName = "CardUserName";
		const string VAR_SkillCheckAbility = "SkillCheckAbility";
		const string VAR_SkillCheckKind = "SkillCheckKind";
		const string VAR_SavingThrowAbility = "SavingThrowAbility";
		const string VAR_CardId = "CardId";
		const string VAR_CardGuid = "CardGuid";
		const string VAR_DiceRoll = "DiceRoll";
		const string VAR_ViewerDieRollTotal = "ViewerDieRollTotal";
		public static int Offset = 0;
		public static Target FriendlyTargets = null;
		public static Creature Creature { get; set; }
		public static Target CardRecipient { get; set; }
		public static string CardGuid { get; set; }
		static object thisCard;
		public static object ThisCard
		{
			get => thisCard;
			set
			{
				if (thisCard == value)
					return;
				thisCard = value;
				if (thisCard is ICard iCard)
				{
					CardUserName = iCard.UserName;
					CardId = iCard.Id;
					CardGuid = iCard.Guid;
				}
			}
		}
		public static string CardUserName { get; set; }
		public static string CardId { get; set; }
		public static DiceRoll DiceRoll { get; set; }
		public static int ViewerDieRollTotal { get; set; }
		public static Ability SavingThrowAbility { get; set; }
		public static Ability SkillCheckAbility { get; set; }
		public static Skills SkillCheckKind { get; set; }


		List<string> KnownVariables = new List<string>();

		public SystemVariables()
		{
			KnownVariables.Add(VAR_MultiTargetNotificationOffset);
			KnownVariables.Add(VAR_FriendlyTargets);
			KnownVariables.Add(VAR_CardRecipient);
			KnownVariables.Add(VAR_ThisCard);
			KnownVariables.Add(VAR_CardUserName);
			KnownVariables.Add(VAR_SkillCheckAbility);
			KnownVariables.Add(VAR_SkillCheckKind);
			KnownVariables.Add(VAR_SavingThrowAbility);
			KnownVariables.Add(VAR_CardId);
			KnownVariables.Add(VAR_CardGuid);
			KnownVariables.Add(VAR_DiceRoll);
			KnownVariables.Add(VAR_ViewerDieRollTotal);
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
			AddCompletionInfo(completionInfo, VAR_CardUserName, "The name of the user who contributed the card that was just played, set set just before calling the card's CardPlayed or CardReceived events.", ExpressionType.text);
			AddCompletionInfo(completionInfo, VAR_SkillCheckAbility, "The ability of the skill check that the player is about to roll.", ExpressionType.@enum);
			AddCompletionInfo(completionInfo, VAR_SkillCheckKind, "The kind of the skill check that the player is about to roll.", ExpressionType.@enum);
			AddCompletionInfo(completionInfo, VAR_SavingThrowAbility, "The ability of the saving throw that the player is about to roll.", ExpressionType.@enum);
			AddCompletionInfo(completionInfo, VAR_CardId, "The id of the card that was just played or received, set just before calling the card's CardPlayed or CardReceived events.", ExpressionType.text);
			AddCompletionInfo(completionInfo, VAR_CardGuid, "The guid of the card that was just played or received, set just before calling the card's CardPlayed or CardReceived events.", ExpressionType.text);
			AddCompletionInfo(completionInfo, VAR_DiceRoll, "The pending dice roll, set just before rolling the dice.", ExpressionType.unknown);
			AddCompletionInfo(completionInfo, VAR_ViewerDieRollTotal, "The total of the last viewer die roll.", ExpressionType.number);

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
				case VAR_CardUserName:
					return CardUserName;
				case VAR_SkillCheckAbility:
					return SkillCheckAbility;
				case VAR_SkillCheckKind:
					return SkillCheckKind;
				case VAR_SavingThrowAbility:
					return SavingThrowAbility;
				case VAR_CardId:
					return CardId;
				case VAR_CardGuid:
					return CardGuid;
				case VAR_DiceRoll:
					return DiceRoll;
				case VAR_ViewerDieRollTotal:
					return ViewerDieRollTotal;
			}

			return GetValue<Creature>(variableName, Creature);
		}
	}
}

