using System;
using System.Linq;
using System.Collections.Generic;

namespace Streamloots
{
	public class SetCardUpdateViewModel
	{
		public string actionType { get; set; }
		public bool autocomplete { get; set; }
		public string description { get; set; }
		public string descriptionDetailed { get; set; }
		public int? dropLimit { get; set; }
		public bool dropLimited { get; set; }
		public bool fragmented { get; set; }
		public int? fragments { get; set; }
		public string name { get; set; }
		public bool obtainable { get; set; }
		public int order { get; set; }
		public string rarity { get; set; }
		public float rarityCardProbability { get; set; }
		public bool redeemable { get; set; }
		public List<RedeemFieldsViewModel> redeemFields { get; set; }

		public RedemptionLimit redemptionLimit { get; set; } = new RedemptionLimit();

		public string redemptionSuccessMessage { get; set; }
		public List<RewardFieldsViewModel> rewardFields { get; set; }



		public SetCardUpdateViewModel()
		{

		}

		public SetCardUpdateViewModel(SetCardViewModel prototype)
		{
			actionType = prototype.actionType;
			autocomplete = prototype.autocomplete;
			description = prototype.description;
			descriptionDetailed = prototype.descriptionDetailed;
			dropLimited = prototype.dropLimited;
			dropLimit = prototype.dropLimit;
			fragmented = prototype.fragmented;
			fragments = prototype.fragments;
			name = prototype.name;
			obtainable = prototype.obtainable;
			order = prototype.order;
			rarity = prototype.rarity;
			rarityCardProbability = prototype.rarityCardProbability;
			redeemable = prototype.redeemable;
			redeemFields = prototype.redeemFields;
			redemptionLimit = prototype.redemptionLimit;
			redemptionSuccessMessage = prototype.redemptionSuccessMessage;
			rewardFields = prototype.rewardFields;
		}

		public virtual void SelfValidate()
		{
			if (order <= 0)
				order = 1;
			if (rarityCardProbability <= 0)
				rarityCardProbability = 1;
			if (string.IsNullOrWhiteSpace(redemptionSuccessMessage))
				redemptionSuccessMessage = null;
			if (string.IsNullOrWhiteSpace(descriptionDetailed))
				descriptionDetailed = null;
		}
	}
}
