using System;

namespace DndCore
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ParamAttribute : Attribute
	{
		public ParamAttribute(int index, Type type, string name, string description, ParameterIs parameterIs, string editor = null)
		{
			Editor = editor;
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
		public string Editor { get; set; }
	}
}
