using System;
using CommonCore;

namespace Imaging
{
	public class LiveFeedData
	{
		/// <summary>
		/// Which camera to use (Front, Right, etc.).
		/// </summary>
		public StudioCamera Camera { get; set; } = StudioCamera.Front;
		
		/// <summary>
		/// The scale of the tracked dots, 1 == 100%.
		/// </summary>
		public double Scale { get; set; }

		/// <summary>
		/// Rotation in degrees
		/// </summary>
		public double Rotation { get; set; }

		/// <summary>
		/// Whether the image is flipped left/right.
		/// </summary>
		public bool Flipped { get; set; }

		/// <summary>
		/// 0..1, where 0 is transparent and 1 is fully opaque.
		/// </summary>
		public double Opacity { get; set; }

		/// <summary>
		/// The center anchor point of the actor.
		/// </summary>
		public Point2d Origin { get; set; }

		public LiveFeedData()
		{

		}
	}
}
