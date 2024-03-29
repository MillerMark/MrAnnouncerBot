﻿using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	[PropertyType(typeof(Vector3))]
	public class ChangeVector3 : BasePropertyChanger
	{
		public ChangeVector3()
		{
		}

		public ChangeVector3(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}

		protected override object ParseValue()
		{
			string[] parts = Value.Trim('(', ')').Split(',');
			float x = 0;
			float y = 0;
			float z = 0;
			if (parts.Length != 3)
			{
				Talespire.Log.Error($"{nameof(ChangeVector3)} needs *three* values separated by commas.");
				return new Vector3(0, 0, 0);
			}

			x = GetFloat(parts[0]);
			y = GetFloat(parts[1]);
			z = GetFloat(parts[2]);

			return new Vector3(x, y, z);
		}

		public void SetValue(string text)
		{
			Value = text;
		}
	}
}
