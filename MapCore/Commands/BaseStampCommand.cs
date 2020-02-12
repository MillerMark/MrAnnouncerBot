using System;
using System.Collections.Generic;
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
