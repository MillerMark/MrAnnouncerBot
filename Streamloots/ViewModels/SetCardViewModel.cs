using System;
using System.Linq;
using System.Collections.Generic;
using Streamloots;
using Newtonsoft.Json;

namespace Streamloots
{
	public class SetCardViewModel: SetCardWithImageViewModel
	{
		public string _id { get; set; }
		public DateTime activatedAt { get; set; }
		public bool archived { get; set; }
		public string backgroundUrl { get; set; }
		public DateTime createdAt { get; set; }
		public bool craftable { get; set; }
		public float craftingCost { get; set; }
		public bool deactivated { get; set; }
		public DateTime deactivatedAt { get; set; }
		public int dropLimitRemaining { get; set; }
		public DateTime firstActivatedAt { get; set; }
		public FileViewModel imageFile { get; set; }
		public DateTime modifiedAt { get; set; }
		public string normalizedName { get; set; }
		public string rarityImageUrl { get; set; }
		public string setId { get; set; }
		public CardStatus status { get; set; }


		public SetCardViewModel()
		{

		}
	}
}
