using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class Feature
	{

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
		public DndTimeSpan Per { get; set; }
		public bool RequiresPlayerActivation { get; set; }
		public List<string> Parameters { get; set; }

		public static Feature FromDto(FeatureDto featureDto)
		{
			Feature result = new Feature();
			result.Name = DndUtils.GetName(featureDto.Name);
			result.Parameters = DndUtils.GetParameters(featureDto.Name);
			result.Conditions = featureDto.Conditions;
			result.OnActivate = featureDto.OnActivate;
			result.OnDeactivate = featureDto.OnDeactivate;
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
			History.Log($"Activating {Name}.");
			IsActive = true;
			if (Duration.HasValue())
			{
				string alarmName = $"{player.name}.{Name}";
				History.TimeClock.CreateAlarm(Duration.GetTimeSpan(), alarmName, player).AlarmFired += Feature_Expired;
			}
			if (!string.IsNullOrWhiteSpace(OnActivate))
				Expressions.Do(DndUtils.InjectParameters(OnActivate, Parameters, arguments), player);
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
			History.Log($"Deactivating {Name}.");
			IsActive = false;
			if (!string.IsNullOrWhiteSpace(OnDeactivate))
				Expressions.Do(DndUtils.InjectParameters(OnDeactivate, Parameters, arguments), player);
		}
	}
}
