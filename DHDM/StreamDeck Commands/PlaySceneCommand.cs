using System;
using System.Linq;
using DndCore;
using TwitchLib.Client.Models;

namespace DHDM
{
	public class PlaySceneCommand : BaseStreamDeckCommand, IDungeonMasterCommand
	{
		string sceneName;

		public void Execute(IDungeonMasterApp dungeonMasterApp, ChatMessage chatMessage)
		{
			dungeonMasterApp.PlayScene(sceneName);
		}

		public bool Matches(string message)
		{
			const string playSceneCommand = "PlayScene";
			if (!message.StartsWith(playSceneCommand + "("))
				return false;

			sceneName = message.Substring(playSceneCommand.Length + 1).Trim();
			if (sceneName.EndsWith(")"))
				sceneName = sceneName.EverythingBeforeLast(")");

			return true;
		}
	}
}
