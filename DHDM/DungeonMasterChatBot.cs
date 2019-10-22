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
			Commands.Add(new SpellSlotSelectorCommand());
			Commands.Add(new HealthDamageCommand());
			Commands.Add(new ScrollCloseCommand());
			Commands.Add(new SkillCheckCommand());
			Commands.Add(new SavingThrowCommand());
			Commands.Add(new AdvanceClockCommand());
			Commands.Add(new SelectShortcutCommand());
			Commands.Add(new StaticCommands());
			Commands.Add(new GetDataCommand());
			Commands.Add(new SetVantageCommand());
			Commands.Add(new BreakConcentrationCommand());
			Commands.Add(new MoveFredCommand());
			Commands.Add(new HiddenThresholdCommand());
		}
		public override void HandleMessage(ChatMessage chatMessage, TwitchClient twitchClient)
		{
			foreach (IDungeonMasterCommand dungeonMasterCommand in Commands)
			{
				if (dungeonMasterCommand.Matches(chatMessage.Message))
				{
					dungeonMasterCommand.Execute(DungeonMasterApp, chatMessage);
					return;
				}
			}
		}
		public void Initialize(IDungeonMasterApp dungeonMasterApp)
		{
			const string MarkMillerUserId = "237584851";
			const string GentryUserId = "163482168";
			const string HumperBotId = "274121151";
			const string DungeonMasterId = "455518839";
			userIds.Clear();
			ListenTo(GentryUserId);
			ListenTo(MarkMillerUserId);
			ListenTo(HumperBotId);
			ListenTo(DungeonMasterId);
			CreateCommandHandlers();
		}
		public IDungeonMasterApp DungeonMasterApp { get; set; }
	}
}
