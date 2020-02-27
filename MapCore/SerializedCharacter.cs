using System;
using System.Linq;

namespace MapCore
{
	public class SerializedCharacter : BaseItemProperties
	{
		public override double Height { get; set; }
		public override double Width { get; set; }
		public SerializedCharacter()
		{

		}

		public override void TransferProperties(IItemProperties itemProperties)
		{
			base.TransferProperties(itemProperties);
			Height = itemProperties.Height;
			Width = itemProperties.Width;
		}

		public static SerializedCharacter From(IItemProperties itemProperties)
		{
			SerializedCharacter result = new SerializedCharacter();
			result.TransferProperties(itemProperties);
			return result;
		}
	}
}
