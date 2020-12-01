using System;
using System.Linq;
using GoogleHelper;

namespace CardMaker
{
	[SheetName(Constants.SheetName_DeckData)]
	[TabName("Fields")]
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

		public Field()
		{

		}

		public Field(Card parentCard)
		{
			CreateNewId();
			ParentCard = parentCard;
			if (ParentCard != null)
				ParentCard.AddField(this);
		}
	}
}
