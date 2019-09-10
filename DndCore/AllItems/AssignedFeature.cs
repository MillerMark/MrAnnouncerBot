using System;
using System.Linq;
using System.Collections.Generic;

namespace DndCore
{
	public class AssignedFeature
	{
		Character Player { get; set; }
		public Feature Feature { get; set; }
		public List<string> Args { get; set; }

		public AssignedFeature(Character player)
		{
			Player = player;
			Args = new List<string>();
		}

		public bool ConditionsSatisfied()
		{
			return Feature.ConditionsSatisfied(Args, Player);
		}

		static AssignedFeature From(Feature feature, string[] arguments, Character player)
		{
			AssignedFeature assignedFeature = new AssignedFeature(player);
			assignedFeature.Feature = feature;
			if (arguments != null)
				assignedFeature.Args = arguments.ToList();
			return assignedFeature;
		}

		public static AssignedFeature From(string featureName, Character player)
		{
			int parenIndex = featureName.IndexOf("(");
			Feature feature;
			string[] arguments = null;
			if (parenIndex >= 0)
			{
				string featureStrippedName = featureName.EverythingBefore("(");
				feature = AllFeatures.Get(featureStrippedName);
				string argumentStr = featureName.EverythingAfter("(");
				parenIndex = featureName.IndexOf(")");
				if (parenIndex >= 0)
					argumentStr = argumentStr.EverythingBefore(")");

				arguments = argumentStr.Split(',');
			}
			else
				feature = AllFeatures.Get(featureName);

			return From(feature, arguments, player);
		}

		public void Activate(bool forceActivation = false)
		{
			Feature.Activate(GetArgStr(), Player, forceActivation);
		}

		private string GetArgStr()
		{
			return string.Join(",", Args);
		}

		public void Deactivate(bool forceDeactivation = false)
		{
			Feature.Deactivate(GetArgStr(), Player, forceDeactivation);
		}

		public bool HasConditions()
		{
			return !string.IsNullOrWhiteSpace(Feature.Conditions);
		}
	}
}
