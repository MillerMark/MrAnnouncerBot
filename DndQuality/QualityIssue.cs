using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DndCore;

namespace DndQuality
{
	public class QualityIssue
	{
		public Guid ID { get; set; }
		public Severity Severity { get; set; }
		public string Description { get; set; }

		public QualityIssue(Severity severity, string description, Guid guid)
		{
			ID = guid;
			Severity = severity;
			Description = description;
		}
	}
}
