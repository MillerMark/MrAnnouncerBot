using System;
using System.Linq;

namespace GoogleHelper
{
	/// <summary>
	/// Use this attribute to mark a field that uniquely identify the instance as belonging to a particular row in a Google Sheet.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class IndexerAttribute : ColumnNameAttribute
	{
		public IndexerAttribute(string columnName = ""): base(columnName)
		{
			
		}

	}
}
