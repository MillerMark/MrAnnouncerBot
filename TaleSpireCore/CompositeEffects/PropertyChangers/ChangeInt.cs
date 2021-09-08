using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	[PropertyType(typeof(int))]
	public class ChangeInt : BasePropertyChanger
	{
		public ChangeInt()
		{
		}

		public ChangeInt(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}


		protected override object ParseValue()
		{
			if (int.TryParse(Value, out int result))
				return result;

			return false;
		}

		public void SetValue(int newValue)
		{
			Value = newValue.ToString();
		}
	}
}
