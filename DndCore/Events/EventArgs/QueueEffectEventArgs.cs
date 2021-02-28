using System;
using System.Linq;

namespace DndCore
{
	public class QueueEffectEventArgs : EventArgs
	{
		public string CardEventName { get; private set; }

		public string CardUserName { get; set; }
		public object[] Args { get; set; }

		public QueueEffectEventArgs(string cardEventName, string cardUserName, params object[] data)
		{
			CardUserName = cardUserName;
			CardEventName = cardEventName;
			Args = data;
		}
		public QueueEffectEventArgs()
		{

		}
	}
}
