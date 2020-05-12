using System;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class HiddenThresholdCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		int hiddenThreshold;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			if (hiddenThreshold == int.MinValue)
			{
				hiddenThreshold = dungeonMasterApp.GetLastSpellSave();
				dungeonMasterApp.TellDungeonMaster($"Setting SAVE hidden threshold to last Spell Save ({hiddenThreshold})...");
			}
			dungeonMasterApp.SetSaveHiddenThreshold(hiddenThreshold);
		}

		public bool Matches(string message)
		{
			if (int.TryParse(message, out hiddenThreshold))
				return true;

			if (message == "SetHiddenThreshold(LastSpell)")
			{
				hiddenThreshold = int.MinValue;
				return true;
			}

			return false;
		}
	}
}
