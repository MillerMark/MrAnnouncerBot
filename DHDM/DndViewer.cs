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
		string dieBackColor = "#ffffff";
		string charges;
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
			get => charges;
			set
			{
				if (charges == value)
					return;
				charges = value;
				OnPropertyChanged();
			}
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
