using System;
using GoogleHelper;
using System.Linq;
using System.Collections.Generic;
using Imaging;

namespace DHDM
{
	[SheetName("D&D Viewers")]
	[TabName("Viewers")]
	public class DndViewer: TrackPropertyChanges
	{
		public bool AlreadySeen { get; set; }
		DateTime lastChatMessage;
		DateTime lastCardPurchase;
		public const bool TestingSayAnything = false;
		string dieBackColor = "#ffffff";
		string chargesStr = null;
		string trailingEffects;
		int cardsPlayed;
		string userName;

		[Indexer]
		[Column]
		public string UserName
		{
			get => userName;
			set
			{
				if (userName == value)
					return;
				userName = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public DateTime LastCardPurchase
		{
			get => lastCardPurchase;
			set
			{
				if (lastCardPurchase == value)
					return;
				lastCardPurchase = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public DateTime LastChatMessage
		{
			get => lastChatMessage;
			set
			{
				if (lastChatMessage == value)
					return;
				lastChatMessage = value;
				OnPropertyChanged();
			}
		}
		


		[Column]
		public int CardsPlayed
		{
			get => cardsPlayed;
			set
			{
				if (cardsPlayed == value)
					return;
				cardsPlayed = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public string Charges  // Semicolon-separated list of key/value pairs - SayAnything=3; ...
		{
			get
			{
				if (chargesStr == null)
					chargesStr = GetChargeStr();
				return chargesStr;
			}
			set
			{
				if (chargesStr == value)
					return;
				chargesStr = value;
				AddCharges();
				OnPropertyChanged();
			}
		}
		void AddCharges()
		{
			if (string.IsNullOrWhiteSpace(chargesStr))
				return;
			string[] individualCharges = chargesStr.Split(';');
			foreach (string charge in individualCharges)
			{
				int equalsPos = charge.IndexOf('=');
				if (equalsPos > 0)
				{
					string[] sides = charge.Split('=');
					if (sides.Length == 2)
					{
						if (int.TryParse(sides[1], out int value))
							charges.Add(sides[0], value);
					}
				}
			}
		}

		string GetChargePart(string key, int value)
		{
			return $"{key}={value};";
		}

		string GetChargeStr()
		{
			string result = "";

			foreach (string key in charges.Keys)
				result += GetChargePart(key, charges[key]);

			return result.TrimEnd(';');
		}

		void RecalculateChargesStr()
		{
			Charges = GetChargeStr();
		}

		internal Dictionary<string, int> charges = new Dictionary<string, int>();

		/// <summary>
		/// Adds the specified charge and returns the total charge count for the specified chargeName.
		/// </summary>
		/// <param name="chargeName">The name of the charge to add.</param>
		/// <param name="chargeCount">The number of charges to add.</param>
		/// <returns>The total charge count for the specified chargeName.</returns>
		public int AddCharge(string chargeName, int chargeCount)
		{
			if (charges.ContainsKey(chargeName))
				charges[chargeName] += chargeCount;
			else
				charges.Add(chargeName, chargeCount);

			RecalculateChargesStr();
			return charges[chargeName];
		}

		public bool HasCharges(string chargeName, int chargeCount = 1)
		{
			if (!charges.ContainsKey(chargeName))
				return false;

			return charges[chargeName] >= chargeCount;
		}


		/// <summary>
		/// Uses the specified number of charges and returns the number of charges remaining. Does not validate if there are 
		/// sufficient charges available (will simply reduce charges to zero if insufficient charges are available), so call 
		/// HasCharges to verify the viewer has sufficient charges before calling UseCharge.
		/// </summary>
		/// <param name="chargeName">The name of the charge to use.</param>
		/// <param name="chargeCount">The number of charges to use (default is one charge if not specified).</param>
		/// <returns>The number of charges remaining.</returns>
		public int UseCharge(string chargeName, int chargeCount = 1)
		{
			if (!charges.ContainsKey(chargeName))
				return 0;

			if (charges[chargeName] <= chargeCount)
			{
				charges.Remove(chargeName);
				RecalculateChargesStr();
				return 0;
			}
			charges[chargeName] -= chargeCount;
			RecalculateChargesStr();
			return charges[chargeName];
		}



		[Column]
		public string TrailingEffects  // A semicolon-separated list of trailing effects behind the player's die. See the TrailingEffects tab of the DnD spreadsheet for a complete list....
		{
			get => trailingEffects;
			set
			{
				if (trailingEffects == value)
					return;
				trailingEffects = value;
				OnPropertyChanged();
			}
		}

		string dieTextColor;
		public string DieTextColor
		{
			get
			{
				if (dieTextColor == null)
					dieTextColor = GetDieTextColor(DieBackColor);
				return dieTextColor;
			}
		}

		public static string GetDieTextColor(string hexColor)
		{
			HueSatLight hueSatLight = new HueSatLight(hexColor);
			const string white = "#ffffff";
			const string black = "#000000";
			if (hueSatLight.IsDark)
				return white;
			else
				return black;
		}

		public bool HasAnyCharges()
		{
			return charges != null && charges.Count > 0;
		}

		[Column]
		public string DieBackColor
		{
			get => dieBackColor;
			set
			{
				if (dieBackColor == value)
					return;
				dieTextColor = null;
				dieBackColor = value;
				OnPropertyChanged();
			}
		}

		private const double MinCardsPlayedToHaveTopReputation = 100;

		/// <summary>
		/// Returns a number between 0 and 1, based on number of cards played.
		/// </summary>
		public double Reputation
		{
			get
			{
				return Math.Min(1.0, CardsPlayed / MinCardsPlayedToHaveTopReputation);
			}
		}
		

		public DndViewer(string userName)
		{
			UserName = userName;
		}

		public DndViewer()
		{

		}
	}
}
