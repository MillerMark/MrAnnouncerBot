using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace DndCore
{
	public class ItemViewModel : ListEntry
	{
		public bool adamantine = false;
		public bool consumable = false;
		public double costValue = 0;
		public int count = 1;
		public ObservableCollection<CurseBlessingDisease> cursesBlessingsDiseases = new ObservableCollection<CurseBlessingDisease>();
		public string description = string.Empty;
		public bool equipped = false;
		public DndTimeSpan equipTime = DndTimeSpan.Zero;
		public ObservableCollection<EffectTimeLines> events = new ObservableCollection<EffectTimeLines>();
		public bool magic = false;
		public int minStrengthToCarry = 0;
		public ObservableCollection<ModViewModel> mods = new ObservableCollection<ModViewModel>();
		public bool silvered = false;
		public DndTimeSpan unequipTime = DndTimeSpan.Zero;
		public int weight = 0;

		public ItemViewModel()
		{
			equipTime.TimeMeasure = TimeMeasure.actions;
			equipTime.Count = 0;
			unequipTime.TimeMeasure = TimeMeasure.actions;
			unequipTime.Count = 0;
		}

		public ItemViewModel(string name)
		{
			StandardName = name;
		}

		public override void AfterLoad()
		{
			base.AfterLoad();
		}

		public ModViewModel FindMod(string name)
		{
			foreach (ModViewModel modViewModel in mods)
			{
				if (modViewModel.StandardName == name)
					return modViewModel;
			}
			return null;
		}
	}
}
