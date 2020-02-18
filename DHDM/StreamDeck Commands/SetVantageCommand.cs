using DndCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class SetVantageCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		VantageKind vantageKind;
		string playerInitial;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetVantage(vantageKind, dungeonMasterApp.GetPlayerIdFromNameStart(playerInitial));
		}

		public bool Matches(string message)
		{
			const string normalStart = "SetVantage(Normal,";
			const string advantageStart = "SetVantage(Advantage,";
			const string disadvantageStart = "SetVantage(Disadvantage,";
			string remainingMessage;
			if (message.StartsWith(normalStart))
			{
				vantageKind = VantageKind.Normal;
				remainingMessage = message.Substring(normalStart.Length);
			}
			else if (message.StartsWith(advantageStart))
			{
				vantageKind = VantageKind.Advantage;
				remainingMessage = message.Substring(advantageStart.Length);
			}
			else if (message.StartsWith(disadvantageStart))
			{
				vantageKind = VantageKind.Disadvantage;
				remainingMessage = message.Substring(disadvantageStart.Length);
			}
			else
				return false;

			remainingMessage = remainingMessage.Trim();
			if (remainingMessage.Length > 0)
				playerInitial = remainingMessage[0].ToString();
			else
				return false;

			return true;
		}
	}
}
