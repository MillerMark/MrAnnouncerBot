using System;
using System.Linq;

namespace DndCore
{
	public class FeatureEventArgs : EventArgs
	{

		public FeatureEventArgs(Feature feature)
		{
			Feature = feature;
		}

		public Feature Feature { get; private set; }
	}
}
