using System;
using System.Linq;

namespace MapCore
{
	public class StampPropertiesData
	{
		public IStampProperties StampProperties { get; set; }
		public StampPropertiesData(IStampProperties stampProperties)
		{
			StampProperties = stampProperties;
		}
		public StampPropertiesData()
		{

		}
	}
}
