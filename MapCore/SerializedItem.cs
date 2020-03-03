using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	public class SerializedStamp : BaseStampProperties
	{
		static SerializedStampEventArgs serializedStampEventArgs = new SerializedStampEventArgs();
		public delegate void SerializedStampEventHandler(object sender, SerializedStampEventArgs ea);
		public static event SerializedStampEventHandler PrepareStampForSerialization;

		static void OnPrepareStampForSerialization(SerializedStamp stamp, IItemProperties properties)
		{
			serializedStampEventArgs.Stamp = stamp;
			serializedStampEventArgs.Properties = properties;
			PrepareStampForSerialization?.Invoke(null, serializedStampEventArgs);
		}

		// TODO: Consider hiding Children BUT still JSON serialize:
		public List<SerializedStamp> Children { get; set; }

		public override double Height { get; set; }
		public override double Width { get; set; }

		public override void TransferProperties(IStampProperties stampProperties)
		{
			base.TransferProperties(stampProperties);
			Height = stampProperties.Height;
			Width = stampProperties.Width;
		}

		public SerializedStamp()
		{
		}

		public void AddChild(SerializedStamp serializedStamp)
		{
			if (Children == null)
				Children = new List<SerializedStamp>();
			Children.Add(serializedStamp);
		}

		public static SerializedStamp From(IItemProperties item)
		{
			SerializedStamp result = new SerializedStamp();
			result.TransferProperties(item);
			OnPrepareStampForSerialization(result, item);
			return result;
		}

		public override IItemProperties Copy(double deltaX, double deltaY)
		{
			// Do nothing. Serialized items are never copied. Only live instances are copied.
			// Note: This may be a smell that the architecture is wrong.
			throw new NotSupportedException("Serialized items should never be copied.");
		}
	}
}
