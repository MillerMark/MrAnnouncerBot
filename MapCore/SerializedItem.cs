using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class SerializedItem
	{
		public delegate void SerializedStampEventHandler(object sender, SerializedStampEventArgs ea);
		public static event SerializedStampEventHandler PrepareItemForSerialization;

		static void OnPrepareStampForSerialization(SerializedItem stamp, IItemProperties properties)
		{
			SerializedStampEventArgs serializedStampEventArgs = new SerializedStampEventArgs();
			serializedStampEventArgs.Item = stamp;
			serializedStampEventArgs.Properties = properties;
			PrepareItemForSerialization?.Invoke(null, serializedStampEventArgs);
		}

		public Guid Guid { get; set; }
		public List<SerializedItem> Children { get; set; } = new List<SerializedItem>();
		public Dictionary<string, object> Properties { get; set; }
		public string TypeName { get; set; }
		public SerializedItem()
		{

		}

		public void GetValuesFrom(object instance)
		{
			Properties = new Dictionary<string, object>();
			TypeName = instance.GetType().Name;
			if (instance is IGuid guidable)
				Guid = guidable.Guid;

			PropertyInfo[] properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (!propertyInfo.CanWrite || !propertyInfo.CanRead)
					continue;

				Newtonsoft.Json.JsonIgnoreAttribute jsonIgnore = propertyInfo.GetCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>();
				if (jsonIgnore != null)
					continue;

				Properties.Add(propertyInfo.Name, propertyInfo.GetValue(instance));
			}
			// TODO: More here...
		}

		public object GetValue(string propertyName)
		{
			throw new NotImplementedException();
		}
		public void AssignPropertiesTo(object target)
		{
			if (target is IGuid guidable)
				guidable.Guid = Guid;

			Type targetType = target.GetType();

			foreach (string key in Properties.Keys)
			{
				SetValue(targetType, target, key, Properties[key]);
			}
		}

		private void SetValue(Type type, object target, string propertyName, object value)
		{

			// get the property information based on the type
			PropertyInfo property = type.GetProperty(propertyName);

			// Convert.ChangeType does not handle conversion to nullable types
			// if the property type is nullable, we need to get the underlying type of the property
			Type propertyType = property.PropertyType;
			var targetType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;

			// special case for enums
			if (targetType.IsEnum)
			{
				// we could be going from an int -> enum so specifically let
				// the Enum object take care of this conversion
				if (value != null)
				{
					value = Enum.ToObject(targetType, value);
				}
			}
			else if (targetType.FullName == "System.Guid")
			{
				value = Guid.Parse((string)value);
			}
			else
			{
				// returns an System.Object with the specified System.Type and whose value is
				// equivalent to the specified object.
				value = Convert.ChangeType(value, targetType);
			}

			// set the value of the property
			property.SetValue(target, value, null);
		}

		private bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
		}


		public static SerializedItem From(IItemProperties itemProperties)
		{
			SerializedItem serializedItem = new SerializedItem();
			serializedItem.GetValuesFrom(itemProperties);
			OnPrepareStampForSerialization(serializedItem, itemProperties);
			return serializedItem;
		}
		public void AddChild(SerializedItem serializedStamp)
		{
			Children.Add(serializedStamp);
		}
	}
}
