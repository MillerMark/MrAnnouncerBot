﻿//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class TextCommandNameAttribute : Attribute
	{
		public TextCommandNameAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}
