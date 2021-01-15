using System;
using System.Linq;

namespace Streamloots
{
	public class CardEventArgs : EventArgs
	{
		public CardDto CardDto { get; private set; }
		public CardEventArgs(CardDto cardDto)
		{
			CardDto = cardDto;
		}
		public CardEventArgs()
		{

		}
	}
}
