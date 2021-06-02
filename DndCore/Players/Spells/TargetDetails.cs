using System;
using System.Linq;

namespace DndCore
{
	public class TargetDetails
	{
		// TODO: Any new props added here should also be transferred in the Clone method!!!
		public string OriginalData { get; set; }
		public TargetDetails(string originalData)
		{
			OriginalData = originalData;
		}
		public TargetDetails()
		{

		}

		/// <when>Changing Properties in Class: "Be sure to include new properties in {className}'s {methodName} method."</when>
		public TargetDetails Clone()
		{
			TargetDetails targetDetails = new TargetDetails()
			{
				OriginalData = OriginalData,
			};
			return targetDetails;
		}
	}
}
