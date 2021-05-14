using System;
using System.Runtime.Remoting;

namespace TaleSpireCore
{
	public static class PropertyChangerFactory
	{
		public static BasePropertyChanger CreateFrom(PropertyChangerDto propertyChangerDto)
		{
			string typeName = nameof(TaleSpireCore) + "." + BasePropertyChanger.STR_ChangePrefix + propertyChangerDto.Type;
			Type changerToCreate = Type.GetType(typeName);
			if (changerToCreate == null)
			{
				Talespire.Log.Error($"Unable to find type \"{typeName}\"!");
				return null;
			}
			return Activator.CreateInstance(changerToCreate) as BasePropertyChanger;
		}
	}
}
