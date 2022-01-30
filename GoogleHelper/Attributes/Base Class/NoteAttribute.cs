using System;
using System.Linq;

namespace GoogleHelper
{
	public class NoteAttribute : Attribute
	{
		public string ColumnNote { get; set; }

		public NoteAttribute(string columnComment = "")
		{
			ColumnNote = columnComment;
		}
	}
}
