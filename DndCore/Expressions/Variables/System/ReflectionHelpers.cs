using System;
using System.Linq;
using System.Reflection;

namespace DndCore
{
	public static class ReflectionHelpers
	{
		/// <summary>
		/// Gets the attribute on the member of the specified type if one exists.
		/// </summary>
		public static T Get<T>(this MemberInfo member) where T : class
		{
			object[] attributes = member.GetCustomAttributes(true);
			foreach (object attribute in attributes)
				if (attribute is T typedAttribute)
					return typedAttribute;
			return null;
		}
	}
}

