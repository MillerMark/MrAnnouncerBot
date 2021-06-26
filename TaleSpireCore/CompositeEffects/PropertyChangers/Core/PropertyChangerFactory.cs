using System;
using System.Runtime.Remoting;

namespace TaleSpireCore
{
	public static class PropertyChangerFactory
	{
		public static BasePropertyChanger CreateFrom(PropertyChangerDto propertyChangerDto)
		{
			return CreateFromTypeName(propertyChangerDto.Type);
		}

		public static BasePropertyChanger CreateFromModDetails(PropertyModDetails propertyModDetails)
		{
			BasePropertyChanger propertyChanger = CreateFromTypeName(propertyModDetails.GetPropertyType().Name);
			propertyChanger.Name = propertyModDetails.GetName();
			return propertyChanger;
		}

		public static BasePropertyChanger CreateFromTypeName(string typeName)
		{
			if (typeName == "Single")
				typeName = "Float";
			string fullTypeName = nameof(TaleSpireCore) + "." + BasePropertyChanger.STR_ChangePrefix + typeName;
			Type changerToCreate = Type.GetType(fullTypeName);
			if (changerToCreate == null)
			{
				Talespire.Log.Error($"Unable to find type \"{fullTypeName}\"!");
				return null;
			}
			return Activator.CreateInstance(changerToCreate) as BasePropertyChanger;
		}
	}
}
