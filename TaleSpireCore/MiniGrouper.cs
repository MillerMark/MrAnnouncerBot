using System;
using System.Linq;
using System.Collections.Generic;

namespace TaleSpireCore
{
	public class MiniGrouper : TaleSpireBehavior
	{
		bool moveToolActive;
		public List<string> ConnectedCreatures { get; set; }
		public MiniGrouper()
		{
			BoardToolManager.OnSwitchTool += BoardToolManager_OnSwitchTool;
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
	}
}
