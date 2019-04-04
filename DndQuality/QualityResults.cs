using System;
using System.Collections.Generic;
using System.Linq;

namespace DndQuality
{
	public class QualityResults
	{
		List<QualityIssue> issues = new List<QualityIssue>();
		public QualityResults()
		{

		}

		public QualityIssue FindFirstIssue(Guid guid)
		{
			return issues.FirstOrDefault(x => x.ID == guid);
		}

		public List<QualityIssue> Issues { get => issues; }
		public int Count { get => issues.Count; }
	}
}
