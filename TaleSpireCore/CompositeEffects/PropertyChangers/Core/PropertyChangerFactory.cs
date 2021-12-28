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

		public static BasePropertyChanger CreateFromModDetails(PropertyModDetails propertyModDetails, bool logErrors = true)
		{
			Type propertyType = propertyModDetails.GetPropertyType();
			if (propertyType == null)
				return null;
			BasePropertyChanger propertyChanger = CreateFromTypeName(propertyType.Name, logErrors);
			if (propertyChanger != null)
				propertyChanger.FullPropertyPath = propertyModDetails.GetName();
			return propertyChanger;
		}

		public static BasePropertyChanger CreateFromTypeName(string typeName, bool logErrors = true)
		{
			if (typeName == "Single")
				typeName = "Float";
			string fullTypeName = nameof(TaleSpireCore) + "." + BasePropertyChanger.STR_ChangePrefix + typeName;
			Type changerToCreate = Type.GetType(fullTypeName);
			if (changerToCreate == null)
			{
				if (logErrors)
					Talespire.Log.Error($"Unable to find type \"{fullTypeName}\"!");
				return null;
			}
			return Activator.CreateInstance(changerToCreate) as BasePropertyChanger;
		}
	}
}
