using System;
using System.Linq;
using GoogleHelper;

namespace CardMaker
{
	public class BaseNameId : TrackPropertyChanges
	{
		string name;
		Guid iD;

		[Indexer]
		[Column]
		public string ID
		{
			get => iD.ToString();
			set
			{
				if (iD != null && iD.ToString() == value)
					return;
				if (string.IsNullOrWhiteSpace(value))
					iD = Guid.NewGuid();
				else
					iD = Guid.Parse(value);
			}
		}

		[Column]
		public string Name
		{
			get => name;
			set
			{
				if (name == value)
					return;
				name = value;
				OnPropertyChanged();
			}
		}

		public void CreateNewId()
		{
			iD = Guid.NewGuid();
		}

		public BaseNameId()
		{

		}
	}
}
