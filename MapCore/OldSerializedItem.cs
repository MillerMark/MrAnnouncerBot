using System;
using System.Linq;
using System.Collections.Generic;

namespace MapCore
{
	//public class OldSerializedItem : BaseStampProperties
	//{
	//	static SerializedStampEventArgs serializedStampEventArgs = new SerializedStampEventArgs();
	//	public delegate void SerializedStampEventHandler(object sender, SerializedStampEventArgs ea);
	//	public static event SerializedStampEventHandler PrepareStampForSerialization;

	//	static void OnPrepareStampForSerialization(OldSerializedItem stamp, IItemProperties properties)
	//	{
	//		serializedStampEventArgs.Item = stamp;
	//		serializedStampEventArgs.Properties = properties;
	//		PrepareStampForSerialization?.Invoke(null, serializedStampEventArgs);
	//	}

	//	// TODO: Consider hiding Children BUT still JSON serialize:
	//	public List<OldSerializedItem> Children { get; set; }

	//	public override double Height { get; set; }
	//	public override double Width { get; set; }

	//	public override void TransferProperties(IStampProperties stampProperties)
	//	{
	//		base.TransferProperties(stampProperties);
	//		Height = stampProperties.Height;
	//		Width = stampProperties.Width;
	//	}

	//	public OldSerializedItem()
	//	{
	//	}

	//	public void AddChild(OldSerializedItem serializedStamp)
	//	{
	//		if (Children == null)
	//			Children = new List<OldSerializedItem>();
	//		Children.Add(serializedStamp);
	//	}

	//	void TransferProperties(IArrangeable arrangeable)
	//	{
	//		FlipHorizontally = arrangeable.FlipHorizontally;
	//		FlipVertically = arrangeable.FlipVertically;
	//		Rotation = arrangeable.Rotation;
	//	}

	//	public static OldSerializedItem From(IItemProperties item)
	//	{
	//		OldSerializedItem result = new OldSerializedItem();
	//		result.GetPropertiesFrom(item);
	//		if (item is IArrangeable arrangeable)
	//			result.TransferProperties(arrangeable);
	//		
	//		OnPrepareStampForSerialization(result, item);
	//		return result;
	//	}

	//	public override IItemProperties Copy(double deltaX, double deltaY)
	//	{
	//		// Do nothing. Serialized items are never copied. Only live instances are copied.
	//		// Note: This may be a smell that the architecture is wrong.
	//		throw new NotSupportedException("Serialized items should never be copied.");
	//	}
	//}
}
