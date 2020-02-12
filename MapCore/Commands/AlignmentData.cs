using System;
using System.Linq;

namespace MapCore
{
	public class AlignmentData : DoubleData
	{
		public StampAlignment Alignment { get; set; }
		public double SpaceBetween { get; set; }
		public AlignmentData(StampAlignment alignment, double value) : base(value)
		{
			Alignment = alignment;
		}
		public AlignmentData()
		{

		}

		public AlignmentData(StampAlignment alignment, double value, double spaceBetween) : this(alignment, value)
		{
			SpaceBetween = spaceBetween;
		}
	}
}
