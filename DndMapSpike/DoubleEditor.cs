using System;
using System.Linq;

namespace DndMapSpike
{
	public class DoubleEditor : TextEditor
	{
		protected override void ConditionText(ref string text)
		{
			if (double.TryParse(text, out double result))
				text = TrimDecimalPoints(result).ToString();
			else
				text = null;
		}

		public new double? Value
		{
			get
			{
				if (double.TryParse(base.Value, out double result))
					return result;
				return null;
			}
			set
			{
				if (value.HasValue)
					base.Value = TrimDecimalPoints(value.Value).ToString();
				else
					base.Value = null;
			}
		}

		private double TrimDecimalPoints(double value)
		{
			return Math.Round(value, DecimalPoints);
		}

		public int DecimalPoints { get; set; }

		public DoubleEditor(int decimalPoints)
		{
			DecimalPoints = decimalPoints;
		}

		public override void Initialize(PropertyValueData propertyValueData)
		{
			base.Initialize(propertyValueData);
			DecimalPoints = propertyValueData.NumDecimalPlaces;
		}
	}
}

