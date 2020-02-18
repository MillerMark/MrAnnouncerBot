using System;
using System.Linq;

namespace DndCore
{
	public class FeatureDto
	{
		public string Name { get; set; }
		public string Index { get; set; }
		public string RequiresActivation { get; set; }
		public string ActivationTime { get; set; }
		public string ShortcutName { get; set; }
		public string ShortcutAvailableWhen { get; set; }
		public string rollMod { get; set; }
		public string Magic { get; set; }
		public string ActivateWhen { get; set; }
		public string OnStartGame { get; set; }
		public string OnShortcutAvailabilityChange { get; set; }
		public string OnActivate { get; set; }
		public string ActivationMessage { get; set; }
		public string OnDeactivate { get; set; }
		public string DeactivationMessage { get; set; }
		public string OnPlayerStartsTurn { get; set; }
		public string OnPlayerRaisesWeapon { get; set; }
		public string AfterPlayerSwingsWeapon { get; set; }
		public string BeforePlayerRollsDice { get; set; }
		public string OnPlayerCastsSpell { get; set; }
		public string OnPlayerSaves { get; set; }
		public string OnRollComplete { get; set; }
		public string Duration { get; set; }
		public string Limit { get; set; }
		public string Per { get; set; }
		public string Description { get; set; }
		public FeatureDto()
		{

		}
	}
}
