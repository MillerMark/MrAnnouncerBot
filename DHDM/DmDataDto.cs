//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public class DmDataDto
	{
		public DmEvent Event { get; set; }
		public DmDataDto(DmEvent @event)
		{
			Event = @event;
		}
		public DmDataDto()
		{

		}
	}
}
