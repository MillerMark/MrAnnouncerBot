//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class ParameterCompletionAttribute : Attribute
	{
		public ParameterCompletionAttribute(Type editorType, int parameterNumber)
		{
			ParameterOrder = parameterNumber;
			EditorType = editorType;
		}

		public Type EditorType { get; set; }
		public int ParameterOrder { get; set; }
	}
}
