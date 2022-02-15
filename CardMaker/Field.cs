using System;
using System.Linq;
using System.Collections.Generic;
using Streamloots;
using SheetsPersist;

namespace CardMaker
{
	[Document(Constants.DocumentName_DeckData)]
	[Sheet("Fields")]
	public class Field : BaseNameId
	{
		Card parentCard;
		public Card ParentCard
		{
			get => parentCard;
			set
			{
				if (parentCard == value)
					return;
				parentCard = value;
				OwningTracker = parentCard;
			}
		}

		FieldType type;
		bool required;
		string label;
		string parentCardId = string.Empty;

		[Column]
		public string CardId
		{
			get
			{
				return ParentCard != null ? ParentCard.ID : parentCardId;
			}
			set
			{
				if (parentCardId == value)
					return;
				parentCardId = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public FieldType Type
		{
			get => type;
			set
			{
				if (type == value)
					return;
				type = value;
				OnPropertyChanged();
			}
		}


		[Column]
		public string Label
		{
			get => label;
			set
			{
				if (label == value)
					return;
				label = value;
				OnPropertyChanged();
			}
		}

		[Column]
		public bool Required
		{
			get => required;
			set
			{
				if (required == value)
					return;
				required = value;
				OnPropertyChanged();
			}
		}


		public override string ToString()
		{
			return Name;
		}
		public RedeemFieldsViewModel ToRedeemFieldsViewModel()
		{
			RedeemFieldsViewModel redeemFieldsViewModel = new RedeemFieldsViewModel();
			redeemFieldsViewModel.label = Label;
			redeemFieldsViewModel.name = Name;
			redeemFieldsViewModel.required = Required;
			// TODO: Make sure these text strings are correct for Streamloots.
			switch (Type)
			{
				case FieldType.Text:
					redeemFieldsViewModel.type = "INPUT#TEXT";
					break;
				case FieldType.LongText:
					redeemFieldsViewModel.type = "TEXTAREA";
					break;
				case FieldType.Phone:
					redeemFieldsViewModel.type = "INPUT#PHONE";
					break;
			}
			
			return redeemFieldsViewModel;
		}

		public Field()
		{

		}

		public Field(Card parentCard)
		{
			CardId = parentCard.ID;
			CreateNewId();
			ParentCard = parentCard;
			if (ParentCard != null)
				ParentCard.AddField(this);
		}
	}
}
