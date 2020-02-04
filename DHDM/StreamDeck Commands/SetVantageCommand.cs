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

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.SetVantage(vantageKind);
		}

		public bool Matches(string message)
		{
			if (message == "SetVantage(Normal)")
				vantageKind = VantageKind.Normal;
			else if (message == "SetVantage(Advantage)")
				vantageKind = VantageKind.Advantage;
			else if (message == "SetVantage(Disadvantage)")
				vantageKind = VantageKind.Disadvantage;
			else
				return false;

			return true;
		}
	}
}
