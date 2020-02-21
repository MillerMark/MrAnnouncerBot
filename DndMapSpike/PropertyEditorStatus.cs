using System;
using System.Linq;

namespace DndMapSpike
{
	public class PropertyEditorStatus
	{
		public OpenClosedStatus OpenClosed { get; set; }
		public string LaunchButtonName { get; set; }

		public PropertyEditorStatus(string launchButtonName)
		{
			LaunchButtonName = launchButtonName;
			OpenClosed = OpenClosedStatus.Closed;
		}
	}
}

