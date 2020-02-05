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

		static void OnPrepareStampForSerialization(SerializedStamp stamp, IStampProperties properties)
		{
			serializedStampEventArgs.Stamp = stamp;
			serializedStampEventArgs.Properties = properties;
			PrepareStampForSerialization?.Invoke(null, serializedStampEventArgs);
		}

		// TODO: Consider hiding Children BUT still JSON serialize:
		public List<SerializedStamp> Children { get; set; }

		public override int Height { get; set; }
		public override int Width { get; set; }

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

		public static SerializedStamp From(IStampProperties stampProperties)
		{
			SerializedStamp result = new SerializedStamp();
			result.TransferProperties(stampProperties);
			OnPrepareStampForSerialization(result, stampProperties);
			return result;
		}
	}
}
