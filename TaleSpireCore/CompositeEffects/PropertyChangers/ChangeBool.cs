using System;
using System.Linq;

namespace TaleSpireCore
{
	public class ChangeBool : BasePropertyChanger
	{
		public ChangeBool()
		{
		}

		public ChangeBool(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}


		public override object GetValue()
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
