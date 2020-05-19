using System;
using System.Linq;

namespace GoogleHelper
{
	public abstract class ColumnNameAttribute : Attribute
	{
		public string ColumnName { get; set; }

		public ColumnNameAttribute(string columnName = "")
		{
			ColumnName = columnName;
		}
	}
}
