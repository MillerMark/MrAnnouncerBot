using DndCore;
using System;
using System.Linq;

namespace DndUI
{
	public class EffectPlaceholderEntry : ListEntry
	{
		//private fields...
		PlaceholderType type;

		public PlaceholderType Type
		{
			get { return type; }
			set
			{
				if (type == value)
					return;
				type = value;
				OnPropertyChanged();
			}
		}
		
		public EffectPlaceholderEntry()
		{

		}
	}
}
