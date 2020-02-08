using System;
using System.Linq;

namespace MapCore
{
	public abstract class BaseStampCommand : BaseCommand
	{
		public BaseStampCommand()
		{
			WorksOnStamps = true;
		}
	}
}
