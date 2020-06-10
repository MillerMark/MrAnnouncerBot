using System;

namespace DndCore
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ParamAttribute : Attribute
	{
		public ParamAttribute(int index, Type type, string name, string description, ParameterIs parameterIs = ParameterIs.Required)
		{
			ParameterIs = parameterIs;
			Description = description;
			Name = name;
			Type = type;
			Index = index;
		}

		public int Index { get; set; }
		public Type Type { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ParameterIs ParameterIs { get; set; }
	}
}
