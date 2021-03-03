using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public static class DigitManager
	{
		public static event DigitChangedEventHandler DigitChanged;
		static Dictionary<string, BufferedNumber> allNumbers = new Dictionary<string, BufferedNumber>();
		
		public static void OnDigitChanged(object sender, DigitChangedEventArgs ea)
		{
			DigitChanged?.Invoke(sender, ea);
		}

		static void CreateIfNeeded(string keyword)
		{
			if (allNumbers.ContainsKey(keyword))
				return;

			allNumbers.Add(keyword, new BufferedNumber());
		}

		public static void AddDigit(string keyword, string digit)
		{
			if (!Debouncer.IsGood($"{keyword}.{digit}"))
				return;
			CreateIfNeeded(keyword);
			allNumbers[keyword].AddDigit(digit);
			TriggerNumberChanged(keyword);
		}

		private static void TriggerNumberChanged(string keyword)
		{
			OnDigitChanged(allNumbers[keyword], new DigitChangedEventArgs(keyword, allNumbers[keyword].GetValue()));
		}

		public static decimal GetValue(string keyword)
		{
			if (string.IsNullOrEmpty(keyword))
				return decimal.MinValue;

			CreateIfNeeded(keyword);
			return allNumbers[keyword].GetValue();
		}

		public static void ResetValue(string keyword)
		{
			CreateIfNeeded(keyword);
			allNumbers[keyword].Reset();
			TriggerNumberChanged(keyword);
		}
		public static void ClearOnNextDigit(string keyword)
		{
			CreateIfNeeded(keyword);
			allNumbers[keyword].ClearOnNextDigit();
		}
		public static void SetValue(string keyword, int value)
		{
			CreateIfNeeded(keyword);
			allNumbers[keyword].SetValue(value);
			TriggerNumberChanged(keyword);
		}
	}
}
