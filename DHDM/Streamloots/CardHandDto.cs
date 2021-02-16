using System;
using System.Linq;
using System.Collections.Generic;
using Streamloots;

namespace DHDM
{
	public class CardHandDto : CardDto
	{
		public List<StreamlootsHand> Hands { get; set; }
		public CardHandDto()
		{

		}
	}
}
