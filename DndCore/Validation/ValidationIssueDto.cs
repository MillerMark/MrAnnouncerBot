using System;
using System.Linq;
using Newtonsoft.Json;

namespace DndCore
{
	public class ValidationIssueDto
	{
		public ValidationAction ValidationAction { get; set; }
		public string FloatText { get; set; }
		public int PlayerId { get; set; }
		public ValidationIssueDto()
		{

		}
	}
}
