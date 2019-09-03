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
			result.Name = GetName(featureDto.Name);
			result.Parameters = GetParameters(featureDto.Name);
			result.Conditions = featureDto.Conditions;
			result.OnActivate = featureDto.OnActivate;
			result.OnDeactivate = featureDto.OnDeactivate;
			result.Duration = DndTimeSpan.FromDurationStr(featureDto.Duration);
			result.Per = DndTimeSpan.FromDurationStr(featureDto.Per);
			result.Limit = featureDto.Limit;
			// Left off here.
			return result;
		}
		static string GetName(string name)
		{
			if (name.IndexOf("(") >= 0)
				return name.EverythingBefore("(");
			return name;
		}

		public void Activate(string parameters, Character player)
		{
			IsActive = true;
			if (!string.IsNullOrWhiteSpace(OnActivate))
				Expressions.Do(InjectParameters(OnActivate, parameters), player);
		}

		public static List<string> GetParameters(string name)
		{
			List<string> result = new List<string>();
			if (name.IndexOf("(") < 0)
				return result;

			char[] trimChars = { ')', ' ', '\t' };
			string parameters = name.EverythingAfter("(").Trim(trimChars);
			string[] allParameters = parameters.Split(',');
			foreach (string parameter in allParameters)
			{
				result.Add(parameter.Trim());
			}
			return result;

		}
		void AddParameters(string name)
		{
			
		}

		public bool ConditionsSatisfied(List<string> args, Character player)
		{
			if (string.IsNullOrWhiteSpace(Conditions))
				return true;

			return Expressions.GetBool(InjectParameters(Conditions, args), player);
		}

		public void Deactivate(string parameters, Character player)
		{
			IsActive = false;
			if (!string.IsNullOrWhiteSpace(OnDeactivate))
				Expressions.Do(InjectParameters(OnDeactivate, parameters), player);
		}

		string InjectParameters(string str, List<string> parameters)
		{
			for (int i = 0; i < Parameters.Count; i++)
			{
				string searchStr = Parameters[i];
				string replaceStr = parameters[i];
				str = str.Replace(searchStr, replaceStr);
			}
			return str;
		}

		public string InjectParameters(string str, string parameters)
		{
			string[] parameterList = parameters.Split(',');
			return InjectParameters(str, parameterList.ToList());
		}
	}
}
