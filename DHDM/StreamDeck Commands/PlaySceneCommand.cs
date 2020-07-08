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

			int bracketStart = sceneName.IndexOf("[");
			if (sceneName.EndsWith("]") && bracketStart > 0)
			{
				if (sceneName.Contains("\\["))
				{
					sceneName = sceneName.Replace("\\[", "[").Replace("\\]", "]");
				}
				else
				{
					string numSceneStr = sceneName.Substring(bracketStart + 1);
					sceneName = sceneName.Substring(0, bracketStart);
					numSceneStr = numSceneStr.Substring(0, numSceneStr.Length - 1);
					if (int.TryParse(numSceneStr, out int numScenes))
					{
						int sceneNumber = new Random().Next(numScenes) + 1;
						sceneName += sceneNumber;
					}
				}
			}

			return true;
		}
	}
}
