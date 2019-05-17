using System;
using System.Linq;
using DndCore.Enums;
using DndCore.ViewModels.Support;

namespace DndCore.ViewModels
{
	public class SavingThrowViewModel : ViewModelBase
	{
		RadioEnumList ability;
		int success;


		public SavingThrowViewModel()
		{
			ability = new RadioEnumList(typeof(Ability), nameof(Ability));
		}

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
	}
}
