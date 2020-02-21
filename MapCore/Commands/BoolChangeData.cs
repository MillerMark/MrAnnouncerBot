namespace MapCore
{
	public class BoolChangeData
	{
		public BoolChangeData(string propertyName, bool isChecked)
		{
			Value = isChecked;
			PropertyName = propertyName;
		}

		public string PropertyName { get; set; }
		public bool Value { get; set; }
	}
}
