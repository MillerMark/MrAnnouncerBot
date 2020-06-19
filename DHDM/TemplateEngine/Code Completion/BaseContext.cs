//#define profiling
using System;
using System.Linq;
using System.Reflection;
using DndCore;

namespace DHDM
{
	public abstract class BaseContext : DndVariable
	{
		public override bool Handles(string tokenName, Character player, CastedSpell castedSpell)
		{
			return tokenName == GetType().Name;
		}

		static BaseContext()
		{
		}

		public static void LoadAll()
		{
			Assembly dndCore = typeof(BaseContext).Assembly;

			foreach (Type type in dndCore.GetTypes())
			{
				if (!type.IsClass)
					continue;
				if (type.BaseType == typeof(BaseContext))
				{
					Expressions.AddVariable((DndVariable)Activator.CreateInstance(type));
				}
			}
		}
	}
}
