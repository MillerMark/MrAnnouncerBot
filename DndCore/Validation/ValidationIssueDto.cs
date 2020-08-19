using System;
using System.Linq;

namespace DndCore
{
	public class ValidationIssueDto
	{
		public ValidationLevel ValidationLevel { get; set; }
		public string Text { get; set; }
		public string FloatText { get; set; }
		public int PlayerId { get; set; }
		public ValidationIssueDto()
		{

		}
	}
}
