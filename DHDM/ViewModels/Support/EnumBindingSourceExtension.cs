using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace DHDM
{
	public class EnumBindingSourceExtension : MarkupExtension
	{
		private Type enumType;
		public Type EnumType
		{
			get { return enumType; }
			set
			{
				if (value == enumType)
					return;

				if (value != null)
				{
					Type newEnumType = Nullable.GetUnderlyingType(value) ?? value;
					if (!newEnumType.IsEnum)
						throw new ArgumentException("Type must be an Enum.");
				}

				enumType = value;
			}
		}

		public EnumBindingSourceExtension() { }

		public EnumBindingSourceExtension(Type enumType)
		{
			this.EnumType = enumType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (null == enumType)
				throw new InvalidOperationException("The EnumType must be specified.");

			Type actualEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
			Array enumValues = Enum.GetValues(actualEnumType);

			if (actualEnumType == enumType)
				return enumValues;

			Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
			enumValues.CopyTo(tempArray, 1);
			return tempArray;
		}
	}
}
