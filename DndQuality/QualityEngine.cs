using System;
using System.Collections.Generic;
using DndCore.ViewModels;
using System.Linq;

namespace DndQuality
{
	public static class QualityEngine
	{
		public static readonly Guid DescriptionIsEmpty = new Guid("{c9d20bfd-5b28-4247-ad6f-183b5945dc8a}");

		static void CheckDescription(QualityResults results, string description)
		{
			if (string.IsNullOrWhiteSpace(description))
				results.Issues.Add(new QualityIssue(Severity.Warning, "Description is empty", DescriptionIsEmpty));
		}
		public static QualityResults CheckItem(ItemViewModel item)
		{
			QualityResults results = new QualityResults();
			CheckDescription(results, item.description);
			return results;
			
		}
	}
}
