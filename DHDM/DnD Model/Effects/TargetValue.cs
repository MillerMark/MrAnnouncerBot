namespace DHDM
{
	public class TargetValue
	{
		public double value;
		public double relativeVariance;
		public double absoluteVariance;
		public double min;
		public double max;
		public double drift;
		public TargetBinding targetBinding;
		public TargetValue(double value, double relativeVariance = 0, double absoluteVariance = 0, double min = 0, double max = 1, double drift = 0, TargetBinding targetBinding = TargetBinding.truncate)
		{
			this.value = value;
			this.relativeVariance = relativeVariance;
			this.absoluteVariance = absoluteVariance;
			this.min = min;
			this.max = max;
			this.drift = drift;
			this.targetBinding = targetBinding;
		}

		public TargetValue()
		{
		}
	}
}
