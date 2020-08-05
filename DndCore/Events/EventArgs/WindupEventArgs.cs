using System;
using System.Linq;

namespace DndCore
{
	public class WindupEventArgs : EventArgs
	{
		public WindupEventArgs(WindupDto windupDto)
		{
			WindupDto = windupDto;
		}

		public WindupDto WindupDto { get; set; }
	}
}
