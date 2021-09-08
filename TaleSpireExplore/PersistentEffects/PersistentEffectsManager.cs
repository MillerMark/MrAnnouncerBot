// CreatureManager.SetCreatureExplicitHideState(creatureGuid, hideState);
using System;
using System.Linq;
using Bounce.Unmanaged;
using UnityEngine;
using TaleSpireCore;

namespace TaleSpireExplore
{
	public static class PersistentEffectsManager
	{
		public static void LoadMenuMods()
		{
			AddCharacterMenu("Duplicate", "Duplicate.png", DuplicateEffectAtMenu);
			AddCharacterMenu("Properties...", "IconEdit.png", ShowPropertyEditor);
			AddCharacterMenu("Lock Rotation", "LockRotation.png", LockRotation, ShouldShowLockRotationMenuItem);
			AddCharacterMenu("Unlock Rotation", "UnlockRotation.png", UnlockRotation, ShouldShowUnlockRotationMenuItem);
			AddCharacterMenu("Hide Orb", "HideShowOrb.png", HideOrb, ShouldShowHideOrbMenuItem);
			AddCharacterMenu("Reveal Orb", "HideShowOrb.png", ShowOrb, ShouldShowRevealOrbMenuItem);

			RemoveCharacterMenu("Stats");
			RemoveCharacterMenu("HP");
			RemoveCharacterMenu("Status");
			RemoveCharacterMenu("Enable Torch");
			RemoveCharacterMenu("Disable Torch");
			RemoveCharacterMenu("Emotes");
			RemoveGmMenu("Set Size");
			RemoveGmMenu("Make Unique");
			RemoveGmMenu("Player Permission");
			RemoveCharacterMenu("Hide");
			RemoveCharacterMenu("Show");
		}

		static void DuplicateEffectAtMenu(MapMenuItem menuItem, object arg2)
		{
			if (menuItem != null)
				Talespire.Log.Warning($"menuItem.gameObject.name = {menuItem.gameObject.name}");

			CreatureBoardAsset creatureAtMenu = RadialUI.RadialUIPlugin.CreatureAtMenu;

			if (creatureAtMenu != null)
			{
				PersistentEffect persistentEffect = creatureAtMenu.GetPersistentEffect();
				Talespire.PersistentEffects.Duplicate(persistentEffect, creatureAtMenu.GetOnlyCreatureName());
			}
			Talespire.Log.Warning($"DuplicateEffectAtMenu: {menuItem}, {arg2}");
			Talespire.Log.Warning($"CreatureAtMenu: {RadialUI.RadialUIPlugin.CreatureAtMenu}");
			
		}

		static void ShowPropertyEditor(MapMenuItem menuItem, object arg2)
		{
			Talespire.Log.Warning($"ShowPropertyEditor: {menuItem}, {arg2}");
			Talespire.Log.Warning($"CreatureAtMenu: {RadialUI.RadialUIPlugin.CreatureAtMenu}");
		}

		static void SetRotationLock(bool value)
		{
			CreatureBoardAsset creatureAtMenu = RadialUI.RadialUIPlugin.CreatureAtMenu;
			if (creatureAtMenu == null)
			{
				Talespire.Log.Error($"SetRotationLock - creatureAtMenu == null");
				return;
			}

			PersistentEffect persistentEffect = creatureAtMenu.GetPersistentEffect();
			if (persistentEffect == null)
			{
				Talespire.Log.Error($"SetRotationLock - persistentEffect == null");
				return;
			}

			Talespire.PersistentEffects.SetRotationLocked(creatureAtMenu, value);
		}

		static void SetHidden(bool value)
		{
			CreatureBoardAsset creatureAtMenu = RadialUI.RadialUIPlugin.CreatureAtMenu;
			if (creatureAtMenu == null)
			{
				Talespire.Log.Error($"creatureAtMenu == null");
				return;
			}

			PersistentEffect persistentEffect = creatureAtMenu.GetPersistentEffect();
			if (persistentEffect == null)
			{
				Talespire.Log.Error($"persistentEffect == null");
				return;
			}

			Talespire.PersistentEffects.SetHidden(creatureAtMenu, value);
		}

		static void LockRotation(MapMenuItem menuItem, object arg2)
		{
			SetRotationLock(true);
		}

		static void HideOrb(MapMenuItem menuItem, object arg2)
		{
			SetHidden(true);
		}

		static void ShowOrb(MapMenuItem menuItem, object arg2)
		{
			SetHidden(false);
		}

		static void UnlockRotation(MapMenuItem menuItem, object arg2)
		{
			SetRotationLock(false);
		}

		private static Sprite LoadIcon(string iconFileName)
		{
			string iconFolder = Talespire.PersistentEffects.Folder + "Assets/";
			Texture2D texture2D = new Texture2D(32, 32);
			texture2D.LoadImage(System.IO.File.ReadAllBytes(iconFolder + iconFileName));
			Sprite icon = Sprite.Create(texture2D, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
			return icon;
		}

		private static void RemoveCharacterMenu(string menuText)
		{
			RadialUI.RadialUIPlugin.AddOnRemoveCharacter(Guid.NewGuid().ToString(), menuText, IsPersistentEffect);
		}

		private static void RemoveGmMenu(string menuText)
		{
			RadialUI.RadialUIPlugin.AddOnRemoveSubmenuGm(Guid.NewGuid().ToString(), menuText, IsPersistentEffect);
		}

		private static void AddCharacterMenu(string title, string iconFileName, Action<MapMenuItem, object> action, 
			Func<NGuid, NGuid, bool> availabilityCheck = null)
		{
			if (availabilityCheck == null)
				availabilityCheck = IsPersistentEffect;
			RadialUI.RadialUIPlugin.AddOnCharacter(Guid.NewGuid().ToString(),
							new MapMenu.ItemArgs
							{
								Action = action,
								Title = title,
								CloseMenuOnActivate = true,
								Icon = LoadIcon(iconFileName)
							}, availabilityCheck);
		}

		static bool IsPersistentEffect(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			return Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId);
		}

		static bool ShouldShowLockRotationMenuItem(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			if (!Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId))
				return false;
			return !Talespire.PersistentEffects.IsPersistentEffectRotationLocked(contextCharacterId);
		}
			
		static bool ShouldShowUnlockRotationMenuItem(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			if (!Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId))
				return false;
			return Talespire.PersistentEffects.IsPersistentEffectRotationLocked(contextCharacterId);
		}
		static bool ShouldShowHideOrbMenuItem(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			if (!Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId))
				return false;
			return !Talespire.PersistentEffects.IsPersistentEffectHidden(contextCharacterId);
		}

		static bool ShouldShowRevealOrbMenuItem(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			if (!Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId))
				return false;
			return Talespire.PersistentEffects.IsPersistentEffectHidden(contextCharacterId);
		}

		static bool IsPersistentEffect(string menuText, string miniId, string targetId)
		{
			//CreatureBoardAsset creatureBoardAsset = Talespire.Minis.GetCreatureBoardAsset(miniId);
			//if (Talespire.PersistentEffects.IsPersistentEffect(miniId))
			//{
			//	if (creatureBoardAsset != null)
			//		Talespire.Log.Warning($"{creatureBoardAsset.Creature.Name}) is persistent!!!)");
			//	else
			//		Talespire.Log.Warning($"IsPersistentEffect(miniId = {miniId}) is True!!!)");
			//	return true;
			//}
			//else
			//{
			//	if (creatureBoardAsset != null)
			//		Talespire.Log.Warning($"{creatureBoardAsset.Creature.Name}) is persistent!!!)");
			//	else
			//		Talespire.Log.Warning($"{miniId} is NOT a persistent effect!!!)");
			//	return false;
			//}
			return Talespire.PersistentEffects.IsPersistentEffect(miniId);
		}
		public static void Initialize()
		{
			LoadMenuMods();
			Talespire.PersistentEffects.PersistentEffectInitialized += PersistentEffects_PersistentEffectInitialized;
			Talespire.Minis.MiniSelected += Minis_MiniSelected;
		}

		static FrmPropertyList frmPropertyList;
		static void ShowPersistentEffectUI(CreatureBoardAsset mini)
		{
			Talespire.Log.Debug($"ShowPersistentEffectUI...");
			if (frmPropertyList != null)
				HidePersistentEffectUI();
			
			frmPropertyList = new FrmPropertyList();
			frmPropertyList.Instance = mini.GetAttachedGameObject();
			frmPropertyList.Mini = mini;
			frmPropertyList.ClearProperties();
			frmPropertyList.AddProperty("Position", typeof(Vector3), "<Transform>.localPosition");
			frmPropertyList.AddProperty("Rotation", typeof(Vector3), "<Transform>.localEulerAngles");
			frmPropertyList.AddProperty("Scale", typeof(Vector3), "<Transform>.localScale");

			PersistentEffect persistentEffect = mini.GetPersistentEffect();
			// TODO: Use persistentEffect to fill in the rest of the properties we want change.


			frmPropertyList.Show();
		}

		static void HidePersistentEffectUI()
		{
			if (frmPropertyList != null)
			{
				frmPropertyList.PrepForClose();
				frmPropertyList.Close();
				frmPropertyList = null;
			}
		}

		private static void Minis_MiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			Talespire.Log.Warning($"Minis_MiniSelected");
			if (ea.Mini != null && ea.Mini.IsPersistentEffect())
				ShowPersistentEffectUI(ea.Mini);
			else
				HidePersistentEffectUI();
		}

		private static void PersistentEffects_PersistentEffectInitialized(object sender, PersistentEffectEventArgs ea)
		{
			Talespire.PersistentEffects.SetSpinLockVisible(ea.CreatureAsset, ea.PersistentEffect.RotationLocked && ea.CreatureAsset.IsVisible);
			ea.PersistentEffect.Initialize(ea.CreatureAsset);
			foreach (string propertyPath in ea.PersistentEffect.Properties.Keys)
			{
				PropertyModDetails propertyModDetails = BasePropertyChanger.GetPropertyModDetails(ea.AttachedNode, propertyPath, true);
				if (propertyModDetails != null)
					Talespire.Log.Error($"propertyModDetails != null");

				BasePropertyChanger propertyChanger = PropertyChangerManager.GetPropertyChanger(propertyModDetails.GetPropertyType());
				propertyChanger.Name = propertyPath;
				propertyChanger.Value = ea.PersistentEffect.Properties[propertyPath];
				Talespire.Log.Warning($"Setting {propertyPath} to {propertyChanger.Value}");
				if (propertyChanger != null)
				{
					Talespire.Log.Warning($"propertyModDetails.SetValue(propertyChanger) !!! Hope this works!!!");
					propertyModDetails.SetValue(propertyChanger);
				}
			}

		}
	}
}
