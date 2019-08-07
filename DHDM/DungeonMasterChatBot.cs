using System;
using DndCore;
using System.Linq;
using System.Collections.Generic;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class DungeonMasterChatBot : BaseChatBot
	{
		List<IDungeonMasterCommand> commands;
		public DungeonMasterChatBot()
		{

		}

		public override void HandleMessage(ChatMessage chatMessage, TwitchClient twitchClient)
		{
			string message = chatMessage.Message;
			if (int.TryParse(message, out int result))
			{
				twitchClient.SendMessage("HumperBot", DungeonMasterApp.SetHiddenThreshold(result));
				return;
			}
			
			if (message.Length == 1)
			{
				int playerId = DungeonMasterApp.GetPlayerIdFromNameStart(message);
				if (playerId >= 0)
				{
					DungeonMasterApp.SelectCharacter(playerId);
					return;
				}
			}
			int multiplier = -1;
			string[] damageLines = message.Split('-');
			if (damageLines.Length == 1)
			{
				damageLines = message.Split('+');
				multiplier = 1;
			}
			if (damageLines.Length > 1)
			{
				string characterName = damageLines[0];
				int playerId = DungeonMasterApp.GetPlayerIdFromNameStart(characterName);
				if (playerId >= 0)
				{
					string damageStr = damageLines[1];
					if (int.TryParse(damageStr, out int damageHealth))
					{
						DamageHealthChange damageHealthChange = new DamageHealthChange();
						damageHealthChange.DamageHealth = multiplier * damageHealth;
						damageHealthChange.PlayerIds.Add(playerId);
						DungeonMasterApp.ApplyDamageHealthChange(damageHealthChange);
						return;
					}
				}
			}
			switch (message.ToLower())
			{
				case "wm":
					DungeonMasterApp.RollWildMagic();
					break;
				case "wmc":
					DungeonMasterApp.RollWildMagicCheck();
					break;
				case "sk":
					DungeonMasterApp.RollSkillCheck(Skills.acrobatics);
					break;
			}
		}
		public void Initialize(IDungeonMasterApp dungeonMasterApp)
		{
			const string MarkMillerUserId = "237584851";
			const string GentryUserId = "163482168";
			userIds.Clear();
			ListenTo(GentryUserId);
			ListenTo(MarkMillerUserId);
		}
		public IDungeonMasterApp DungeonMasterApp { get; set; }
	}
}
