using DndCore;
using System;
using System.Linq;

namespace DndUI
{
	public class SavingThrowViewModel : ViewModelBase
	{
		int success;
		RadioEnumList ability;

		public RadioEnumList Ability
		{
			get { return ability; }
			set
			{
				if (ability == value)
					return;
				ability = value;
				OnPropertyChanged();
			}
		}

		
		public int Success
		{
			get { return success; }
			set
			{
				if (success == value)
					return;
				success = value;
				OnPropertyChanged();
			}
		}
		

		public SavingThrowViewModel()
		{
			ability = new RadioEnumList(typeof(Ability), "Ability");
		}
	}
}
