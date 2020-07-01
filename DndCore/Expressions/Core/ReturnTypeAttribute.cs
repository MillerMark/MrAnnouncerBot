using System;

namespace DndCore
{
	public class ReturnTypeAttribute : Attribute
	{
		public ReturnTypeAttribute(Type returnType)
		{
			ReturnType = returnType;
		}
		public ReturnTypeAttribute(ExpressionType expressionReturnType)
		{
			ExpressionReturnType = expressionReturnType;
		}

		public Type ReturnType { get; set; }
		public ExpressionType ExpressionReturnType { get; set; } = ExpressionType.unknown;
		public bool Matches(Type type)
		{
			if (type == ReturnType)
				return true;
			return TypeHelper.Matches(ExpressionReturnType, type);
		}
	}
}
