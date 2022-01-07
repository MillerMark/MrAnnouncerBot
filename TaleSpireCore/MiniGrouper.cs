using System;
using System.Linq;
using System.Collections.Generic;
using Bounce.ManagedCollections;
using UnityEngine;

namespace TaleSpireCore
{
	public class MiniGrouper : TaleSpireBehavior
	{
		System.Timers.Timer updateMiniIndicatorTimer;
		bool moveToolActive;
		public List<string> Members { get; set; } = new List<string>();
		public Color IndicatorColor { get; set; } = UnityEngine.Color.red;
		public MiniGrouper()
		{
			BoardToolManager.OnSwitchTool += BoardToolManager_OnSwitchTool;
			updateMiniIndicatorTimer = new System.Timers.Timer();
			updateMiniIndicatorTimer.Interval = 250;
			updateMiniIndicatorTimer.Elapsed += UpdateMiniIndicatorTimer_Elapsed;
		}

		~MiniGrouper()
		{
			UnhookEvents();
		}

		private void UnhookEvents()
		{
			Talespire.Log.Debug($"MiniGrouper.UnhookEvents!!!!!");
			BoardToolManager.OnSwitchTool -= BoardToolManager_OnSwitchTool;
		}

		void OnDestroy()
		{
			UnhookEvents();
		}


		void OnDisable()
		{
			UnhookEvents();
		}

		private void BoardToolManager_OnSwitchTool(BoardTool obj)
		{
			if (obj is CreatureMoveBoardTool)
			{
				moveToolActive = true;
				// Something is moving!
			}
			else if (obj is DefaultBoardTool)
			{
				if (moveToolActive)
				{
					moveToolActive = false;
					// Were WE moved???
					CreatureBoardAsset selected = Talespire.Minis.GetSelected();
					if (selected?.Creature.CreatureId.ToString() == OwnerID)
					{
						Talespire.Log.Debug($"Grouper was just moved!!!");
					}
				}
			}
		}

		void RemoveMember(CreatureBoardAsset creatureBoardAsset)
		{
			string id = creatureBoardAsset.CreatureId.ToString();
			Members.Remove(creatureBoardAsset.CreatureId.ToString());
			Talespire.Minis.IndicatorChangeColor(id, UnityEngine.Color.black);
			UpdateMiniColorsSoon();
		}

		void AddMember(CreatureBoardAsset creatureBoardAsset)
		{
			string id = creatureBoardAsset.CreatureId.ToString();
			Members.Add(id);
			UpdateMiniColors();
			UpdateMiniColorsSoon();
		}

		void UpdateMiniColorsSoon()
		{
			updateMiniIndicatorTimer.Start();
		}

		private void UpdateMiniColorsSafe()
		{
			UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
			{
				UpdateMiniColors();
			});
		}

		private void UpdateMiniColors()
		{
			foreach (string memberId in Members)
				Talespire.Minis.IndicatorChangeColor(memberId, IndicatorColor);

			Talespire.Minis.IndicatorChangeColor(OwnerID, IndicatorColor);
		}

		private void UpdateMiniIndicatorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateMiniIndicatorTimer.Stop();
			UpdateMiniColorsSafe();
		}

		public void ToggleMember(CreatureBoardAsset creatureBoardAsset)
		{
			if (Members.Contains(creatureBoardAsset.CreatureId.ToString()))
				RemoveMember(creatureBoardAsset);
			else
				AddMember(creatureBoardAsset);
		}

		public void RefreshIndicators()
		{
			UpdateMiniColors();
		}
	}
}
