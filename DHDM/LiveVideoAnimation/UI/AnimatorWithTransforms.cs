using System;
using System.Linq;
using System.Collections.Generic;
using Imaging;

namespace DHDM
{
	public class AnimatorWithTransforms
	{
		public List<ObsTransformEdit> ObsTransformEdits { get; set; } = new List<ObsTransformEdit>();
		public LiveFeedAnimator LiveFeedAnimator { get; set; }
		public string MovementFileName { get; set; }
		public AnimatorWithTransforms()
		{

		}
	}
}
