using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	public static class AllActionShortcuts
	{
		static List<PlayerActionShortcut> shortcuts;

		public static void Invalidate()
		{
			shortcuts = null;
		}

		static void ProcessDtos(List<PlayerActionShortcutDto> playerDtos)
		{
			shortcuts = new List<PlayerActionShortcut>();

			shortcuts.Clear();

			int index = 0;
			List<PlayerActionShortcut> lastShortcuts = null;
			while (index <= playerDtos.Count - 1)
			{
				PlayerActionShortcutDto thisShortcutDto = playerDtos[index];

				if (string.IsNullOrWhiteSpace(thisShortcutDto.player)) // appending to last entry...
				{
					foreach (PlayerActionShortcut shortcut in lastShortcuts)
					{
						Character player = AllPlayers.GetFromId(shortcut.PlayerId);

						bool isWindup = shortcut.Spell != null && !shortcut.Spell.Duration.HasValue() && shortcut.Spell.MustRollDiceToCast();
						shortcut.AddEffect(thisShortcutDto, shortcut.lastPrefix, player, shortcut.SpellSlotLevel, isWindup);
					}
				}
				else
				{
					lastShortcuts = PlayerActionShortcut.From(thisShortcutDto);
					foreach (PlayerActionShortcut shortcut in lastShortcuts)
					{
						shortcuts.Add(shortcut);
					}
				}
				index++;
			}
		}
		public static void LoadData()
		{
			ProcessDtos(LoadData(Folders.InCoreData("DnD - Shortcuts.csv")));
			AddPlayerShortcuts();
		}
		static void AddSpellShortcutsFor(Character player, KnownSpell knownSpell)
		{
			List<PlayerActionShortcut> lastShortcuts = null;
			List<ItemEffect> spellEffects = AllSpellEffects.GetAll(knownSpell.SpellName).OrderBy(x => x.index).ToList();
			if (spellEffects.Count == 0)
			{
				lastShortcuts = PlayerActionShortcut.FromItemSpellEffect(knownSpell.SpellName, null, player);
			}
			else
				for (int i = 0; i < spellEffects.Count; i++)
				{
					ItemEffect itemEffect = spellEffects[i];
					if (i == 0)
						lastShortcuts = PlayerActionShortcut.FromItemSpellEffect(knownSpell.SpellName, itemEffect, player);
					else
						foreach (PlayerActionShortcut playerActionShortcut in lastShortcuts)
						{
							playerActionShortcut.Windups.Add(WindupDto.FromItemEffect(itemEffect, playerActionShortcut.DisplayText));
						}
				}
			if (lastShortcuts != null)
				AllShortcuts.AddRange(lastShortcuts);
		}

		static void AddWeaponShortcutsFor(Character player, CarriedWeapon carriedWeapon)
		{
			List<PlayerActionShortcut> lastShortcuts = null;

			List<ItemEffect> weaponEffects = AllWeaponEffects.GetAll(carriedWeapon.Name).OrderBy(x => x.index).ToList();
			if (!weaponEffects.Any())
				if (carriedWeapon.Weapon != null)
					weaponEffects = AllWeaponEffects.GetAll(carriedWeapon.Weapon.Name).OrderBy(x => x.index).ToList(); ;
			if (weaponEffects.Count == 0)
			{
				lastShortcuts = PlayerActionShortcut.FromWeapon(carriedWeapon, null, player);
			}
			else
				for (int i = 0; i < weaponEffects.Count; i++)
				{
					ItemEffect itemEffect = weaponEffects[i];
					if (i == 0)
					{
						lastShortcuts = PlayerActionShortcut.FromWeapon(carriedWeapon, itemEffect, player);
					}
					else
						foreach (PlayerActionShortcut playerActionShortcut in lastShortcuts)
						{
							playerActionShortcut.Windups.Add(WindupDto.FromItemEffect(itemEffect, PlayerActionShortcut.WeaponWindupPrefix + playerActionShortcut.DisplayText));
						}
				}

			if (lastShortcuts != null)
				AllShortcuts.AddRange(lastShortcuts);
		}

		static void AddFeatureShortcutsFor(Character player, Feature feature)
		{
			PlayerActionShortcut shortcut = PlayerActionShortcut.FromFeature(feature, player, ActivationShortcutKind.Activate);
			AllShortcuts.Add(shortcut);

			shortcut = PlayerActionShortcut.FromFeature(feature, player, ActivationShortcutKind.Deactivate);
			if (shortcut != null)
				AllShortcuts.Add(shortcut);
		}

		public static void AddShortcutsFor(Character player)
		{
			if (!player.playingNow)
				return;

			foreach (KnownSpell knownSpell in player.KnownSpells)
				AddSpellShortcutsFor(player, knownSpell);

			foreach (CarriedWeapon carriedWeapon in player.CarriedWeapons)
				AddWeaponShortcutsFor(player, carriedWeapon);

			AddFeatureShortcutsFor(player);
		}

		static void AddFeatureShortcutsFor(Character player)
		{
			if (player.features == null)
				return;
			List<AssignedFeature> featuresRequiringActivation = player.features.Where(x => x.Feature.RequiresPlayerActivation).ToList();
			foreach (AssignedFeature assignedFeature in featuresRequiringActivation)
			{
				AddFeatureShortcutsFor(player, assignedFeature.Feature);
			}
		}

		static void AddPlayerShortcuts()
		{
			foreach (Character player in AllPlayers.Players)
			{
				AddShortcutsFor(player);
			}
		}
		
		public static List<PlayerActionShortcutDto> LoadData(string dataFile)
		{
			return GoogleSheets.Get<PlayerActionShortcutDto>(dataFile);
		}

		public static List<PlayerActionShortcut> Get(int playerId)
		{
			return AllShortcuts.Where(x => x.PlayerId == playerId).ToList();
		}

		public static List<PlayerActionShortcut> Get(int playerId, TurnPart part)
		{
			return AllShortcuts.Where(x => x.PlayerId == playerId).Where(x => x.Part == part).ToList();
		}

		public static List<PlayerActionShortcut> Get(int playerId, string nameStart)
		{
			string lowerName = nameStart.ToLower();
			return AllShortcuts.Where(x => x.PlayerId == playerId).Where(x => x.DisplayText.ToLower().StartsWith(lowerName)).ToList();
		}

		public static List<PlayerActionShortcut> AllShortcuts {
			get
			{
				if (shortcuts == null)
					LoadData();
				return shortcuts;
			}
			private set => shortcuts = value; }
	}
}

