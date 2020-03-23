using System;
using System.Linq;

namespace MapCore
{
	public interface IStampProperties : IItemProperties, IModifiableColor, IArrangeable, IScalable
	{
		string Name { get; set; }

		bool Collectible { get; set; }

		bool Hideable { get; set; }

		double MinStrengthToMove { get; set; }

		Cover Cover { get; set; }

		StampAltitude Altitude { get; set; }

		double Weight { get; set; }

		/// <summary>
		/// In Gold Pieces
		/// </summary>
		double Value { get; set; }

		void ResetImage();
	}
}
