using System;
using System.Linq;

namespace DndCore
{
	public class Coins
	{
		int numGold;
		public int NumGold
		{
			get
			{
				return numGold;
			}
			set
			{
				if (numGold == value)
					return;
				totalGold = double.NaN;
				numGold = value;
			}
		}
		int numSilver;
		public int NumSilver
		{
			get
			{
				return numSilver;
			}
			set
			{
				if (numSilver == value)
					return;
				totalGold = double.NaN;
				numSilver = value;
			}
		}
		int numCopper;
		public int NumCopper
		{
			get
			{
				return numCopper;
			}
			set
			{
				if (numCopper == value)
					return;
				totalGold = double.NaN;
				numCopper = value;
			}
		}
		int numElectrum;
		public int NumElectrum
		{
			get
			{
				return numElectrum;
			}
			set
			{
				if (numElectrum == value)
					return;
				totalGold = double.NaN;
				numElectrum = value;
			}
		}
		int numPlatinum;
		public int NumPlatinum
		{
			get
			{
				return numPlatinum;
			}
			set
			{
				if (numPlatinum == value)
					return;
				totalGold = double.NaN;
				numPlatinum = value;
			}
		}

		double totalGold = double.NaN;
		public static double ConvertToGold(double numPlatinum, double numGold, double numElectrum, double numSilver, double numCopper)
		{
			double totalGold = numPlatinum * 10.0 + numGold + numElectrum / 2.0 + numSilver / 10.0 + numCopper / 100.0;
			return totalGold;
		}
		public double TotalGold
		{
			get
			{
				if (double.IsNaN(totalGold))
					totalGold = ConvertToGold(NumPlatinum, NumGold, NumElectrum, NumSilver, NumCopper);
				return totalGold;
			}
			set
			{
				if (totalGold == value)
					return;
				totalGold = value;
				SetFromGold(totalGold);
			}
		}
		
		public Coins()
		{

		}

		public void SetFromText(string text)
		{
			if (!double.TryParse(text, out double result))
				return;

			SetFromGold(result);
		}

		public void SetFromGold(double totalGold)
		{
			int multiplier = 1;
			if (totalGold < 0)
			{
				totalGold = Math.Abs(totalGold);
				multiplier = -1;
			}

			int numGold = (int)Math.Floor(totalGold);
			int numPlatinum = numGold / 10;
			totalGold -= numGold;

			if (numPlatinum > 0)
				numGold -= numPlatinum * 10;

			totalGold *= 10;

			int numSilver = (int)Math.Floor(totalGold);
			int numElectrum = numSilver / 5;
			totalGold -= numSilver;

			if (numElectrum > 0)
				numSilver -= numElectrum * 5;

			totalGold *= 10;

			this.numCopper = multiplier * (int)Math.Floor(totalGold);
			this.numSilver = multiplier * numSilver;
			this.numElectrum = multiplier * numElectrum;
			this.numGold = multiplier * numGold;
			this.numPlatinum = multiplier * numPlatinum;
		}
	}
}
