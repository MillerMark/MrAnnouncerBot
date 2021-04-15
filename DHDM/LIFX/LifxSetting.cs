//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class LifxSetting
	{
		public string Color { get; set; }
		public double Duration { get; set; }
		public double Brightness { get; set; }
		public DateTime CreationTime { get; set; }
		public double AgeSeconds => (DateTime.Now - CreationTime).TotalSeconds;
		public static bool operator ==(LifxSetting left, LifxSetting right)
		{
			if ((object)left == null)
				return (object)right == null;
			else
				return left.Equals(right);
		}
		public static bool operator !=(LifxSetting left, LifxSetting right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is LifxSetting)
				return Equals((LifxSetting)obj);
			else if (obj is IntPtr)
				return Equals((IntPtr)obj);
			else
				return base.Equals(obj);
		}

		public bool Equals(LifxSetting lifxSetting)
		{
			if (lifxSetting == null)
				return false;
			
			return lifxSetting.Brightness == Brightness && lifxSetting.Color == Color && lifxSetting.Duration == Duration;		
		}

		public bool Equals(IntPtr obj)
		{
			if (obj as object is LifxSetting lifxSetting)
				return Equals(lifxSetting);
			return false;
		}

		public LifxSetting()
		{
			CreationTime = DateTime.Now;
		}
	}
}