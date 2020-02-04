using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DHDM
{
	public class BaseStreamDeckCommand
	{
		protected const string PlayerSpecifier = @"(\s\[" + RegexConstants.PlayerFirstInitials + @"\])*";
		public string TargetPlayer { get; set; }

		protected void SetTargetPlayer(GroupCollection groups)
		{
			TargetPlayer = null;
			string lastEntry = groups[groups.Count - 1].Value;
			if (lastEntry == null)
				return;
			lastEntry = lastEntry.Trim();
			if (!lastEntry.StartsWith("["))
				return;
			TargetPlayer = lastEntry[1].ToString();
		}

		protected List<int> GetPlayerIds(IDungeonMasterApp dungeonMasterApp, bool testAllPlayers)
		{
			List<int> playerIds = new List<int>();
			if (TargetPlayer == null)
				playerIds.Add(dungeonMasterApp.GetActivePlayerId());
			else if (testAllPlayers)
				playerIds.Add(int.MaxValue);
			else
				playerIds.Add(dungeonMasterApp.GetPlayerIdFromNameStart(TargetPlayer));
			return playerIds;
		}
	}
}
