using System;
using System.Linq;

namespace DHDM
{
	public class BufferedNumber
	{
		bool clearOnNextDigit;
		decimal digitsEntered = decimal.MinValue;
		decimal decimalMultiplier = 1m;
		DateTime lastDigitSetTime = DateTime.MinValue;

		public decimal GetValue()
		{
			return digitsEntered;
		}

		public void Reset()
		{
			clearOnNextDigit = false;
			digitsEntered = decimal.MinValue;
			decimalMultiplier = 1m;
			lastDigitSetTime = DateTime.MinValue;
		}

		public void AddDigit(string value)
		{
			if (lastDigitSetTime == DateTime.MinValue || (DateTime.Now - lastDigitSetTime).TotalSeconds > 12 || clearOnNextDigit)
				Reset();

			lastDigitSetTime = DateTime.Now;
			if (value == ".")
			{
				decimalMultiplier = 0.1m;
				if (digitsEntered == decimal.MinValue)
					digitsEntered = 0;
			}
			else
			{
				if (int.TryParse(value, out int asInt))
					if (digitsEntered == decimal.MinValue)
						digitsEntered = asInt;
					else
					{
						if (decimalMultiplier == 1m)
						{
							digitsEntered *= 10m;
							digitsEntered += asInt;
						}
						else
						{
							digitsEntered += asInt * decimalMultiplier;
							decimalMultiplier *= 0.1m;
						}
					}
			}
		}
		public void SetValue(int value)
		{
			digitsEntered = value;
		}
		public void ClearOnNextDigit()
		{
			clearOnNextDigit = true;
		}
	}
}
