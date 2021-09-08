using System;
using System.Linq;

namespace TaleSpireCore
{
	public class PropertyTypeAttribute : Attribute
	{
		public Type PropertyType { get; set; }
		public PropertyTypeAttribute(Type propertyType)
		{
			PropertyType = propertyType;
		}
		public PropertyTypeAttribute()
		{

		}
	}
}
