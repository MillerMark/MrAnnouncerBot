namespace MapCore
{
	public class ChangeData<T>
	{
		public ChangeData(string propertyName, T value)
		{
			Value = value;
			PropertyName = propertyName;
		}

		public string PropertyName { get; set; }
		public T Value { get; set; }
	}
}
