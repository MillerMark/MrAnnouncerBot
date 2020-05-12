using System;
using System.Linq;

namespace GoogleHelper
{
	/// <summary>
	/// Use this to map a property to a particular column (with a header of the same name) in a Google Sheet.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ColumnAttribute : ColumnNameAttribute
	{
		public ColumnAttribute(string columnName = ""): base(columnName)
		{
		}
	}
}
