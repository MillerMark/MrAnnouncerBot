using System;
using System.Linq;

namespace DndCore
{
	public class QueueEffectEventArgs : EventArgs
	{
		public string cardEventName { get; private set; }

		public object[] Args { get; set; }

		public QueueEffectEventArgs(string cardEventName, params object[] data)
		{
			this.cardEventName = cardEventName;
			Args = data;
		}
		public QueueEffectEventArgs()
		{

		}
	}
}
