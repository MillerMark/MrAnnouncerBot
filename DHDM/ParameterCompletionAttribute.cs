//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class ParameterCompletionAttribute : Attribute
	{
		public ParameterCompletionAttribute(Type editorType, string parameterName)
		{
			ParameterName = parameterName;
			EditorType = editorType;
		}

		public Type EditorType { get; set; }
		public string ParameterName { get; set; }
	}
}
