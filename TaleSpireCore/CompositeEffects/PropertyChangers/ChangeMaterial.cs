using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class ChangeMaterial : BasePropertyChanger
	{
		public ChangeMaterial()
		{
		}

		public ChangeMaterial(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}


		protected override object ParseValue()
		{
			return Talespire.Material.Get(Value);
		}

		public void SetValue(string materialName)
		{
			Value = materialName;
		}
	}
}
