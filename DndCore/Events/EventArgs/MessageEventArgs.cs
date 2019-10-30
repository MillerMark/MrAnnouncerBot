using System;
using System.Linq;

namespace DndCore
{
	public class MessageEventArgs : EventArgs
	{

		public MessageEventArgs(string message)
		{
			Message = message;
		}

		public string Message { get; private set; }
	}
}
