namespace MapCore
{
	public class EnumChangeData
	{
		public EnumChangeData(string propertyName, int value)
		{
			Value = value;
			PropertyName = propertyName;
		}

		public string PropertyName { get; set; }
		public int Value { get; set; }
	}
}
