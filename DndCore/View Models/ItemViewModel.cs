using DndCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DndCore
{
	public class ItemViewModel : ListEntry
	{
		public DndTimeSpan equipTime = DndTimeSpan.Zero;
		public DndTimeSpan unequipTime = DndTimeSpan.Zero;
		public string description = string.Empty;
		public bool consumable = false;
		public int weight = 0;
		public double costValue = 0;
		public bool magic = false;
		public bool equipped = false;
		public bool silvered = false;
		public bool adamantine = false;
		public int count = 1;
		public int minStrengthToCarry = 0;
		public ObservableCollection<ModViewModel> mods = new ObservableCollection<ModViewModel>();
		public ObservableCollection<CurseBlessingDisease> cursesBlessingsDiseases = new ObservableCollection<CurseBlessingDisease>();
		public ObservableCollection<EffectTimeLines> events = new ObservableCollection<EffectTimeLines>();

		public ItemViewModel()
		{
			equipTime.TimeMeasure = TimeMeasure.actions;
			equipTime.Count = 0;
			unequipTime.TimeMeasure = TimeMeasure.actions;
			unequipTime.Count = 0;
		}

		public override void AfterLoad()
		{
			base.AfterLoad();
		}

		public ItemViewModel(string name)
		{
			Name = name;
		}

		public ItemV Item
		{
			get { return GetItem(); }
			set
			{
				SetFromItem(value);
			}
		}
		ItemV GetItem()
		{
			ItemV item = new ItemV();
			//if (conditions != null)
			//	mod.condition = (Conditions)conditions.Value;
			//else
			//	mod.condition = DndCore.Conditions.None;

			return item;
		}

		void SetFromItem(ItemV item)
		{
			//conditions.Value = item.condition;
		}
	}
}
