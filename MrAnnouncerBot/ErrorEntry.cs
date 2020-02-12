using System;
using System.Linq;

namespace MrAnnouncerBot
{
	public class ErrorEntry : Entry
	{
		public Exception Exception { get; set; }
		public ErrorEntry()
		{

		}
	}
}
