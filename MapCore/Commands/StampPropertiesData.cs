using System;
using System.Linq;

namespace MapCore
{
	public class ItemPropertiesData
	{
		public IItemProperties ItemProperties { get; set; }
		public ItemPropertiesData(IItemProperties itemProperties)
		{
			ItemProperties = itemProperties;
		}
		public ItemPropertiesData()
		{

		}
	}
}
