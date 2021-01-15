using System;
using System.Linq;

namespace Streamloots
{
	public class SetLootViewModel
	{
		public string _id { get; set; }
		public string alert { get; set; }
		public DateTime createdAt { get; set; }
		public Currency currency { get; set; }
		public bool deactivated { get; set; }
		public string giftAlert { get; set; }
		public string imageUrl { get; set; }
		public DateTime modifiedAt { get; set; }
		public string ownerId { get; set; }
		public float price { get; set; }
		public float quantity { get; set; }
		public string setId { get; set; }

		public SetLootViewModel()
		{

		}
	}
}
