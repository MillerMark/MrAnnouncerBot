using System;

namespace DHDM
{
	public class RenderFrameEventArgs : EventArgs
	{
		public double Duration { get; set; }
		public bool ShouldStop { get; set; }

		public RenderFrameEventArgs()
		{

		}
	}
}