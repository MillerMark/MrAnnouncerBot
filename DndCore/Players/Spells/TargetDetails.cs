using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DndCore
{
	public class TargetDetails
	{
		// TODO: Any new props added here should also be transferred in the Clone method!!!
		public string OriginalData { get; set; }
		public List<TargetDetails> ChildDetails = new List<TargetDetails>();
		public TargetKind Kind { get; set; }
		public SpellTargetShape Shape { get; set; }
		public bool CenteredOnSelf { get; set; }
		public double MaxCreatures { get; set; }
		public int Dimensions { get; set; }
		public SizeUnits SizeUnits { get; set; }
		public string CreatureQualifications { get; set; } // A text description of what kind of creatures can be targeted.

		public TargetDetails(string originalData)
		{
			OriginalData = originalData;
			string[] parts = originalData.Split('|');
			if (parts.Length > 0)
			{
				Initialize(parts[0]);
				for (int i = 1; i < parts.Length; i++)
					ChildDetails.Add(new TargetDetails(parts[i]));
			}

		}
		public TargetDetails()
		{

		}

		public TargetDetails Clone()
		{
			TargetDetails result = new TargetDetails()
			{
				OriginalData = OriginalData,
				Kind = Kind,
				Shape = Shape,
				CenteredOnSelf = CenteredOnSelf,
				MaxCreatures = MaxCreatures,
				Dimensions = Dimensions,
				SizeUnits = SizeUnits,
				CreatureQualifications = CreatureQualifications
			};

			foreach (TargetDetails childTargetDetails in ChildDetails)
				result.ChildDetails.Add(childTargetDetails.Clone());

			return result;
		}

		void ProcessPart(string part)
		{
			string trimmedPart = part.Trim();
			if (string.IsNullOrEmpty(trimmedPart))
				return;

			if (trimmedPart == "o")
				Kind |= TargetKind.Object;
			else if (trimmedPart == ".")
				Kind |= TargetKind.Location;
			else if (trimmedPart == "*")
			{
				Kind |= TargetKind.Self;
				MaxCreatures += 1;
			}
			else if (trimmedPart == "inf")
			{
				Kind |= TargetKind.Creatures;
				MaxCreatures = double.MaxValue;
			}
			else if (trimmedPart.EndsWith("*"))
			{
				CenteredOnSelf = true;
				trimmedPart = trimmedPart.Remove(trimmedPart.Length - 1);
			}

			if (trimmedPart.Contains("mile"))
			{
				System.Diagnostics.Debugger.Break();
			}

			Match sizeShape = Regex.Match(trimmedPart, $"^(\\d+)'\\s+(\\w+)$");
			if (sizeShape.Success)
				SizeUnits = SizeUnits.Feet;
			else
			{
				sizeShape = Regex.Match(trimmedPart, $"^(\\d+)\\s+mile\\s+(\\w+)$");
				if (sizeShape.Success)
					SizeUnits = SizeUnits.Miles;
			}

			if (sizeShape.Success)
			{
				string dimensionStr = sizeShape.Groups[1].Value;
				if (int.TryParse(dimensionStr, out int dimensions))
					Dimensions = dimensions;
				Shape = DndUtils.ToShape(sizeShape.Groups[2].Value);
				Kind |= TargetKind.Volume | TargetKind.Creatures;
				// Is there ever a time when there's a shape but no creatures? If so, consider changing the text in the target column of the Spells data to indicate that (and modify code here to detect/parse that).
			}
			else
			{
				Match countQualifiedCreatures = Regex.Match(trimmedPart, $"^(\\d+)\\s+(.+)$");
				if (countQualifiedCreatures.Success)
				{
					string creatureCountStr = countQualifiedCreatures.Groups[1].Value;
					if (int.TryParse(creatureCountStr, out int creatureCount))
						MaxCreatures = creatureCount;

					CreatureQualifications = countQualifiedCreatures.Groups[2].Value;
					Kind |= TargetKind.Creatures;
				}
				else
				{
					Match countCreatures = Regex.Match(trimmedPart, $"^(\\d+)$");
					if (int.TryParse(countCreatures.Groups[1].Value, out int creatureCount))
					{
						MaxCreatures = creatureCount;
						Kind |= TargetKind.Creatures;
					}
				}
			}


		}
		void Initialize(string message)
		{
			Kind = TargetKind.None;
			Shape = SpellTargetShape.None;
			string[] addedParts = message.Trim().Split('+');
			if (addedParts.Length > 0)
				foreach (string addedPart in addedParts)
					ProcessPart(addedPart);
		}
	}
}
