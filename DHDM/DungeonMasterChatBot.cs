using System;
using DndCore;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class DungeonMasterChatBot : BaseChatBot
	{
		private const string commandSeparator = " | ";
		List<IDungeonMasterCommand> commands;
		
		public List<IDungeonMasterCommand> Commands
		{
			get {
				if (commands == null)
					commands = new List<IDungeonMasterCommand>();
				return commands;
			}
		}
		
		public DungeonMasterChatBot()
		{

		}



		void CreateCommandHandlers()
		{
			Commands.Clear();
			Commands.Add(new PlayerSelectorCommand());
			Commands.Add(new PlaySceneCommand());
			Commands.Add(new StopPlaying());
			Commands.Add(new PlaySoundCommand());
			Commands.Add(new HealthDamageCommand());
			Commands.Add(new ScrollCloseCommand());
			Commands.Add(new SpellScrollsToggleCommand());
			Commands.Add(new SkillCheckNowCommand());
			Commands.Add(new SkillCheckCommand());
			Commands.Add(new TargetSkillCheckCommand());
			Commands.Add(new RollSaveCommand());
			Commands.Add(new SavingThrowCommand());
			Commands.Add(new TargetSavingThrowCommand());
			Commands.Add(new ClearAllConditionsCommand());
			Commands.Add(new SavingThrowNowCommand());
			Commands.Add(new ChangeVolumeCommand());
			Commands.Add(new ChangeWealthCommand());
			Commands.Add(new InstantRollCommand());
			Commands.Add(new DigitCommand());
			Commands.Add(new ApplyCommand());
			Commands.Add(new TargetMoveCommands());
			Commands.Add(new ClearDiceCommand());
			Commands.Add(new ApplyLastDamageCommand());
			Commands.Add(new ClearNumbersCommand());
			Commands.Add(new ChangeCommand());
			Commands.Add(new InGameUICommand());
			Commands.Add(new NextTurnCommand());
			Commands.Add(new SetPropertyCommand());
			Commands.Add(new AdvanceClockCommand());
			Commands.Add(new SelectShortcutCommand());
			Commands.Add(new StaticCommands());
			Commands.Add(new GetDataCommand());
			Commands.Add(new SetVantageCommand());
			Commands.Add(new RollDiceCommand());
			Commands.Add(new BreakConcentrationCommand());
			Commands.Add(new ReStackConditionsCommand());
			Commands.Add(new ToggleTargetCommand());
			Commands.Add(new TogglePlayerTargetCommand());
			Commands.Add(new ToggleNpcCommand());
			Commands.Add(new ToggleConditionCommand());
			Commands.Add(new TalkNpcCommand());
			Commands.Add(new TargetCommand());
			Commands.Add(new ChangePlayerStateCommand());
			Commands.Add(new MoveFredCommand());
			Commands.Add(new HiddenThresholdCommand());
		}

		string StripCommandsFromMessage(string message, out string commands)
		{
			if (message.Contains(commandSeparator))
			{
				commands = message.EverythingAfter(commandSeparator);
				return message.EverythingBefore(commandSeparator);
			}
			commands = "";
			return message;
		}

		public override void HandleMessage(ChatMessage chatMessage, TwitchClient twitchClient, Character activePlayer)
		{
			foreach (IDungeonMasterCommand dungeonMasterCommand in Commands)
			{
				string streamDeckCommand = StripCommandsFromMessage(chatMessage.Message, out string commands);
				if (dungeonMasterCommand.Matches(streamDeckCommand))
				{
					Expressions.Do(commands, activePlayer);
					dungeonMasterCommand.Execute(DungeonMasterApp, chatMessage);
					return;
				}
			}
		}

		public void Initialize(IDungeonMasterApp dungeonMasterApp)
		{
			const string MarkMillerUserId = "237584851";
			const string MattUserId = "191566012";
			const string HumperBotId = "274121151";
			const string DungeonMasterId = "455518839";
			userIds.Clear();
			ListenTo(MattUserId);
			ListenTo(MarkMillerUserId);
			ListenTo(HumperBotId);
			ListenTo(DungeonMasterId);
			CreateCommandHandlers();
		}
		public IDungeonMasterApp DungeonMasterApp { get; set; }
	}
}
