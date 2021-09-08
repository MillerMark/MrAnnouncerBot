using System;
using System.Linq;

namespace TaleSpireCore
{
	[PropertyType(typeof(bool))]
	public class ChangeBool : BasePropertyChanger
	{
		public ChangeBool()
		{
		}

		public ChangeBool(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}


		protected override object ParseValue()
		{
			if (bool.TryParse(Value, out bool result))
				return result;

			return false;
		}

		public void SetValue(bool newValue)
		{
			if (newValue)
				Value = "true";
			else
				Value = "false";
		}
	}
}
