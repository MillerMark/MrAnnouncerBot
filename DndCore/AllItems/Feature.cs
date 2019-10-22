using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class Feature
	{
		public static event FeatureEventHandler FeatureActivated;
		public static event FeatureEventHandler FeatureDeactivated;
		public static void OnFeatureActivated(object sender, FeatureEventArgs ea)
		{
			FeatureActivated?.Invoke(sender, ea);
		}
		public static void OnFeatureDeactivated(object sender, FeatureEventArgs ea)
		{
			FeatureDeactivated?.Invoke(sender, ea);
		}
		public Feature()
		{

		}

		public string Conditions { get; set; }
		public DndTimeSpan Duration { get; set; }
		public bool IsActive { get; private set; }
		public string Limit { get; set; }  // Could be an expression.
		public string Name { get; set; }
		public string OnActivate { get; set; }
		public string OnDeactivate { get; set; }
		public string OnPlayerCastsSpell { get; set; }
		public string OnPlayerSwingsWeapon { get; set; }
		public string OnPlayerStartsTurn { get; set; }
		public string OnPlayerSaves { get; set; }
		public string OnRollComplete { get; set; }
		public DndTimeSpan Per { get; set; }
		public bool RequiresPlayerActivation { get; set; }
		public List<string> Parameters { get; set; }
		public string Description { get; set; }
		public string ActivationMessage { get; set; }
		public string DeactivationMessage { get; set; }
		public TurnPart ActivationTime { get; set; }
		public string ShortcutName { get; set; }
		public bool Magic { get; set; }

		public static Feature FromDto(FeatureDto featureDto)
		{
			Feature result = new Feature();
			result.Name = DndUtils.GetName(featureDto.Name);
			result.Parameters = DndUtils.GetParameters(featureDto.Name);
			result.Conditions = featureDto.Conditions;
			result.Description = featureDto.Description;
			result.OnActivate = featureDto.OnActivate;
			result.ActivationMessage = featureDto.ActivationMessage;
			result.OnDeactivate = featureDto.OnDeactivate;
			result.DeactivationMessage = featureDto.DeactivationMessage;
			result.OnPlayerCastsSpell = featureDto.OnPlayerCastsSpell;
			result.OnPlayerSwingsWeapon = featureDto.OnPlayerSwingsWeapon;
			result.OnPlayerStartsTurn = featureDto.OnPlayerStartsTurn;
			result.OnPlayerSaves = featureDto.OnPlayerSaves;
			result.OnRollComplete = featureDto.OnRollComplete;
			result.ShortcutName = featureDto.ShortcutName;
			result.Magic = MathUtils.IsChecked(featureDto.Magic);
			result.ActivationTime = PlayerActionShortcut.GetTurnPart(featureDto.ActivationTime);
			result.RequiresPlayerActivation = MathUtils.IsChecked(featureDto.RequiresActivation);
			result.Duration = DndTimeSpan.FromDurationStr(featureDto.Duration);
			result.Per = DndTimeSpan.FromDurationStr(featureDto.Per);
			result.Limit = featureDto.Limit;
			// Left off here.
			return result;
		}

		public void Activate(string arguments, Character player, bool forceActivation = false)
		{
			if (IsActive && !forceActivation)
				return;
			if (player != null)
				History.Log($"Activating {player.name}'s {Name}.");
			else
				History.Log($"Activating {Name}.");
			IsActive = true;
			if (Duration.HasValue())
			{
				string alarmName = $"{player.name}.{Name}";
				History.TimeClock.CreateAlarm(Duration.GetTimeSpan(), alarmName, player).AlarmFired += Feature_Expired;
			}
			if (!string.IsNullOrWhiteSpace(OnActivate))
				Expressions.Do(DndUtils.InjectParameters(OnActivate, Parameters, arguments), player);
			OnFeatureActivated(player, new FeatureEventArgs(this));
		}

		private void Feature_Expired(object sender, DndTimeEventArgs ea)
		{
			if (IsActive)
				Deactivate(string.Empty, ea.Alarm.Player);
		}

		public bool ConditionsSatisfied(List<string> args, Character player)
		{
			if (string.IsNullOrWhiteSpace(Conditions))
				return true;

			return Expressions.GetBool(DndUtils.InjectParameters(Conditions, Parameters, args), player);
		}

		public void Deactivate(string arguments, Character player, bool forceDeactivation = false)
		{
			if (!IsActive && !forceDeactivation)
				return;
			if (player != null)
				History.Log($"Deactivating {player.name}'s {Name}.");
			else
				History.Log($"Deactivating {Name}.");
			IsActive = false;
			if (!string.IsNullOrWhiteSpace(OnDeactivate))
				Expressions.Do(DndUtils.InjectParameters(OnDeactivate, Parameters, arguments), player);
			OnFeatureDeactivated(player, new FeatureEventArgs(this));
		}

		public void SpellJustCast(string arguments, Character player, CastedSpell spell)
		{
			if (!IsActive)
				return;
			if (!string.IsNullOrWhiteSpace(OnPlayerCastsSpell))
				Expressions.Do(DndUtils.InjectParameters(OnPlayerCastsSpell, Parameters, arguments), player, null, spell);
		}

		public void WeaponJustSwung(string arguments, Character player)
		{
			if (!IsActive)
				return;
			if (!string.IsNullOrWhiteSpace(OnPlayerSwingsWeapon))
				Expressions.Do(DndUtils.InjectParameters(OnPlayerSwingsWeapon, Parameters, arguments), player);
		}

		public void PlayerStartsTurn(string arguments, Character player)
		{
			if (!IsActive)
				return;
				if (!string.IsNullOrWhiteSpace(OnPlayerStartsTurn))
					Expressions.Do(DndUtils.InjectParameters(OnPlayerStartsTurn, Parameters, arguments), player);
		}

		
		// TODO: Call when a player rolls a saving throw (to implement DangerSense).
		public void PlayerSaves(string arguments, Character player)
		{
			if (!IsActive)
				return;
			if (!string.IsNullOrWhiteSpace(OnPlayerSaves))
				Expressions.Do(DndUtils.InjectParameters(OnPlayerSaves, Parameters, arguments), player);
		}

		public void RollIsComplete(string arguments, Character player)
		{
			if (!IsActive)
				return;
			if (!string.IsNullOrWhiteSpace(OnRollComplete))
				Expressions.Do(DndUtils.InjectParameters(OnRollComplete, Parameters, arguments), player);
		}
	}
}
