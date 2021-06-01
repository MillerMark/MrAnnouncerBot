using System;
using System.Linq;

namespace TaleSpireCore
{
	public class ChangeString : BasePropertyChanger
	{
		public ChangeString()
		{
		}

		public ChangeString(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}

		public override object GetValue()
		{
			return Value;
		}

		public void SetValue(string value)
		{
			Value = value;
		}
	}
}
