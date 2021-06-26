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

		public override bool TrySetProperty(object instance, string propertyName)
		{
			if (instance is Material material)
			{
				// TODO: Verify property name is in the shader????
				material.SetFloat(propertyName, (float)GetValue());
			
				return true;
			}

			return false;
		}

		public override bool CanSetProperty(object instance, string propertyName)
		{
			return instance is Material;
		}

		protected override object ParseValue()
		{
			if (float.TryParse(Value, out float result))
				return result;

			return 0;
		}

		public void SetValue(float value)
		{
			Value = value.ToString();
		}
	}
}
