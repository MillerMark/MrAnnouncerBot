using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;

namespace DndCore
{
    public static class AllWeaponEffects
    {
        public static void Invalidate()
        {
            weaponEffects = null;
            weaponEffectsByName = null;
        }

        static void LoadData()
        {
            weaponEffects = new List<ItemEffect>();
            List<ItemEffectDto> weaponEffectDtos = CsvToSheetsHelper.Get<ItemEffectDto>(Folders.InCoreData("DnD - WeaponEffects.csv"));
            foreach (ItemEffectDto itemEffect in weaponEffectDtos)
            {
                weaponEffects.Add(ItemEffect.From(itemEffect));
            }
            weaponEffectsByName = weaponEffects
                .GroupBy(e => e.name)
                .ToDictionary(g => g.Key, g => g.OrderBy(e => e.index).ToList());
        }

        static List<ItemEffect> weaponEffects = new List<ItemEffect>();
        static Dictionary<string, List<ItemEffect>> weaponEffectsByName;

        public static List<ItemEffect> WeaponEffects
        {
            get
            {
                if (weaponEffects == null)
                    LoadData();
                return weaponEffects;
            }
            private set
            {
                weaponEffects = value;
            }
        }

        public static List<ItemEffect> GetAll(string weaponName)
        {
            if (string.IsNullOrEmpty(weaponName))
                return new List<ItemEffect>();
            if (weaponEffects == null)
                LoadData();
            if (weaponEffectsByName != null && weaponEffectsByName.TryGetValue(weaponName, out List<ItemEffect> cached))
                return cached;
            return new List<ItemEffect>();
        }
    }
}
