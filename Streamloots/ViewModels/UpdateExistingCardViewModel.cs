using System;
using System.Linq;
using System.Collections.Generic;

namespace Streamloots
{
	public class UpdateExistingCardViewModel : SetCardWithImageViewModel
	{
		public string _id { get; set; }
		public bool craftable { get; set; } = false;

		public UpdateExistingCardViewModel()
		{

		}

		public void SetOrderFrom(List<SetCardViewModel> existingCards)
		{
			SetCardViewModel existingCard = existingCards.FirstOrDefault(x => x._id == _id);
			if (existingCard != null)
				order = existingCard.order;
		}
	}
}
