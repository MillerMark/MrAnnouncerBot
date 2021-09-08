using Bounce.Unmanaged;
using System;
using System.Linq;

namespace TaleSpireCore
{
	[PropertyType(typeof(NGuid))]
	public class ChangeNGuid : BasePropertyChanger
	{
		public ChangeNGuid()
		{
		}

		public ChangeNGuid(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}

		protected override object ParseValue()
		{
			if (NGuid.TryParseAcceptingNull(Value, out NGuid guid))
				return guid;
			return null;
		}

		public void SetValue(string value)
		{
			Value = value;
		}
	}
}
