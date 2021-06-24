using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Property
		{
			public static void ModifyColor(object parent, string childName, string propertyName, string valueStr)
			{
				object instance = null;
				if (childName == null)
					instance = parent;
				else if (parent is GameObject gameObject)
					instance = gameObject.FindChild(childName);
				if (instance == null)
				{
					Log.Error($"Unable to find child named \"{childName}\".");
					return;
				}
				Log.Debug($"ModifyColor - ChangeColor changeColor = new ChangeColor(\"{propertyName}\", \"{valueStr}\");");
				ChangeColor changeColor = new ChangeColor(propertyName, valueStr);
				changeColor.ModifyProperty(instance);
			}

			public static void ModifyFloat(object parent, string childName, string propertyName, float value)
			{
				object instance = null;
				if (childName == null)
					instance = parent;
				else if (parent is GameObject gameObject)
					instance = gameObject.FindChild(childName);
				if (instance == null)
				{
					Log.Error($"Unable to find child named \"{childName}\".");
					return;
				}
				Log.Debug($"ModifyFloat - ChangeFloat changeFloat = new ChangeFloat(\"{propertyName}\", {value});");
				ChangeFloat changeFloat = new ChangeFloat(propertyName, value.ToString());
				changeFloat.ModifyProperty(instance);
			}
		}
	}
}