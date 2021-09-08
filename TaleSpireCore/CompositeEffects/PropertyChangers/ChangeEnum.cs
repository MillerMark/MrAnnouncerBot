using System;
using System.Linq;

namespace TaleSpireCore
{
	[PropertyType(typeof(Enum))]
	public class ChangeEnum : ChangeInt
	{
		public ChangeEnum()
		{
		}

		public ChangeEnum(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}
	}
}
