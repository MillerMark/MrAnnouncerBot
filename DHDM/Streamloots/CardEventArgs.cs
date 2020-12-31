using System;
using System.Linq;

namespace DHDM
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
