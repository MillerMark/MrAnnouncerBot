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
		public string Limit { get; set; }
		public string Name { get; set; }
		public string OnActivate { get; set; }
		public string OnDeactivate { get; set; }
		public DndTimeSpan Per { get; set; }
		public bool RequiresPlayerActivation { get; set; }
		public List<string> Parameters { get; set; }

		public static Feature FromDto(FeatureDto featureDto)
		{
			Feature result = new Feature();
			result.Name = GetName(featureDto.Name);
			result.AddParameters(featureDto.Name);
			result.Conditions = featureDto.Conditions;
			result.Duration = DndTimeSpan.FromDurationStr(featureDto.Duration);
			// Left off here.
			return result;
		}
		static string GetName(string name)
		{
			if (name.IndexOf("(") >= 0)
				return name.EverythingBefore("(");
			return name;
		}

		public void Activate(string parameters)
		{
			IsActive = true;
			if (!string.IsNullOrWhiteSpace(InjectParameters(parameters)))
				Expressions.Do(OnActivate);
		}

		void AddParameters(string name)
		{
			if (name.IndexOf("(") < 0)
				return;
			if (Parameters == null)
				Parameters = new List<string>();
			Parameters.Clear();
			char[] trimChars = { ')', ' ', '\t' };
			string parameters = name.EverythingAfter("(").Trim(trimChars);
			string[] allParameters = parameters.Split(',');
			foreach (string parameter in allParameters)
			{
				Parameters.Add(parameter);
			}
		}

		public bool ConditionsSatisfied(string parameters)
		{
			if (!string.IsNullOrWhiteSpace(Conditions))
				return true;

			return Expressions.GetBool(InjectParameters(parameters));
		}
		public bool ConditionsSatisfied(List<string> args)
		{
			if (!string.IsNullOrWhiteSpace(Conditions))
				return true;

			return Expressions.GetBool(InjectParameters(args));
		}

		public void Deactivate(string parameters)
		{
			IsActive = false;
			if (!string.IsNullOrWhiteSpace(InjectParameters(parameters)))
				Expressions.Do(OnDeactivate);
		}

		string InjectParameters(List<string> parameters)
		{
			throw new NotImplementedException();
		}

		public string InjectParameters(string parameters)
		{
			string[] parameterList = parameters.Split(',');
			return InjectParameters(parameterList.ToList());
		}
	}
}
