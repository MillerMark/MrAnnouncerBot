using System;

namespace Imaging
{
	public class IntermediateResults
	{
		/// <summary>
		/// A red circle marks the left side of a camera tracker.
		/// </summary>
		public CountTotals Red { get; set; } = new CountTotals();

		/// <summary>
		/// A blue circle marks the right side of a camera tracker.
		/// </summary>
		public CountTotals Blue { get; set; } = new CountTotals();
		
		/// <summary>
		/// A green circle marks the top of a Camera1 tracker.
		/// </summary>
		public CountTotals Green { get; set; } = new CountTotals();
		
		/// <summary>
		/// A yellow circle marks the top of a Camera2 tracker.
		/// </summary>
		public CountTotals Yellow { get; set; } = new CountTotals();

		/// <summary>
		/// A cyan circle marks the top of a Camera3 tracker.
		/// </summary>
		public CountTotals Cyan { get; set; } = new CountTotals();


		/// <summary>
		/// A magenta circle marks the top of a Camera4 tracker.
		/// </summary>
		public CountTotals Magenta { get; set; } = new CountTotals();

		public byte GreatestOpacity { get; set; }
	}
}
