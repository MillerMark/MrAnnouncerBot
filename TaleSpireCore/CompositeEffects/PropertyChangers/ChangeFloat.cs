using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class ChangeFloat : BasePropertyChanger
	{
		public ChangeFloat()
		{
		}

		public ChangeFloat(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}

		public override object GetValue()
		{
			if (float.TryParse(Value, out float result))
				return result;

			return 0;
		}
	}
}
