using System;

namespace DndCore
{
	public class ReturnTypeAttribute : Attribute
	{
		public ReturnTypeAttribute(Type returnType)
		{
			ReturnType = returnType;
		}

		public Type ReturnType { get; set; }
	}
}
