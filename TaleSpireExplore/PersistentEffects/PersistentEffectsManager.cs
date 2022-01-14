// CreatureManager.SetCreatureExplicitHideState(creatureGuid, hideState);
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Bounce.Unmanaged;
using UnityEngine;
using TaleSpireCore;
using System.Runtime.CompilerServices;

namespace TaleSpireExplore
{
	public static class PersistentEffectsManager
	{
		const string STR_SmartPropertyPrefix = "$";
		static FrmPropertyList frmPropertyList;

		static void AddCharacterMenu(
			string title,
			string iconFileName,
			Action<MapMenuItem, object> action,
			Func<NGuid, NGuid, bool> availabilityCheck = null)
		{
			if (availabilityCheck == null)
				availabilityCheck = IsPersistentEffect;
			RadialUI.RadialUIPlugin.RegisterAddCharacter(new MapMenu.ItemArgs
			{
				Action = action,
				Title = title,
				CloseMenuOnActivate = true,
				Icon = LoadIcon(iconFileName)
			},
					availabilityCheck);
		}

		static void DuplicateEffectAtMenu(MapMenuItem menuItem, object arg2)
		{
			//if (menuItem != null)
			//	Talespire.Log.Warning($"menuItem.gameObject.name = {menuItem.gameObject.name}");

			CreatureBoardAsset creatureAtMenu = RadialUI.RadialUIPlugin.CreatureAtMenu;

			//Talespire.Log.Warning($"DuplicateEffectAtMenu: \"{menuItem}\", \"{arg2}\"");
			//Talespire.Log.Warning($"CreatureAtMenu: {creatureAtMenu.Creature.Name}");

			if (creatureAtMenu != null)
			{
				IOldPersistentEffect persistentEffect = creatureAtMenu.GetPersistentEffect();
				//string persistentEffectData = creatureAtMenu.GetPersistentEffectData();
				Talespire.PersistentEffects.Duplicate(persistentEffect, creatureAtMenu.GetOnlyCreatureName());
			}
		}

		static void HideOrb(MapMenuItem menuItem, object arg2)
		{
			SetHidden(true);
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

		static bool IsPersistentEffect(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			return Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId);
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

		static Sprite LoadIcon(string iconFileName)
		{
			string iconFolder = $"{Talespire.PersistentEffects.Folder}Assets/";
			Texture2D texture2D = new Texture2D(32, 32);
			texture2D.LoadImage(File.ReadAllBytes($"{iconFolder}{iconFileName}"));
			Sprite icon = Sprite.Create(texture2D, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
			return icon;
		}

		static void LockRotation(MapMenuItem menuItem, object arg2)
		{
			SetRotationLock(true);
		}
		public static bool SuppressPersistentEffectUI;
		static void Minis_MiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			if (SuppressPersistentEffectUI)
				return;

			if (ea.Mini != null && ea.Mini.IsPersistentEffect())
				ShowPersistentEffectUI(ea.Mini);
			else
				HidePersistentEffectUI();
		}

		static void ModifyProperty(PersistentEffectEventArgs ea, string propertyPath, string prefix = null, string propertyKey = null, bool logDetails = false)
		{
			if (logDetails)
				Talespire.Log.Debug($"ModifyProperty({propertyPath})");
			if (propertyKey == null)
				propertyKey = propertyPath;

			GameObject childNodeToModify = ea.AttachedNode.GetChildNodeStartingWith(prefix);
			if (childNodeToModify == null)
			{
				Talespire.Log.Error($"childNodeToModify == null");
				childNodeToModify = ea.AttachedNode;
			}
			PropertyModDetails propertyModDetails = BasePropertyChanger.GetPropertyModDetails(childNodeToModify, propertyPath, logDetails);
			if (propertyModDetails == null)
			{
				Talespire.Log.Error($"propertyModDetails == null for property {propertyPath}");
				return;
			}
			if (logDetails)
				Talespire.Log.Debug($"propertyModDetails.GetPropertyType() = {propertyModDetails.GetPropertyType()}");
			BasePropertyChanger propertyChanger = PropertyChangerManager.GetPropertyChanger(propertyModDetails.GetPropertyType());

			if (propertyChanger != null)
			{
				propertyChanger.FullPropertyPath = propertyPath;

				if (ea.PersistentEffect is SuperPersistentEffect superPersistentEffect && prefix != null)
				{
					if (logDetails)
						Talespire.Log.Warning($"prefix = \"{prefix}\"");

					string numberStr;
					if (prefix.StartsWith("0") && prefix.Length > 1)
						numberStr = prefix.Remove(0, 1);
					else
						numberStr = prefix;

					if (!int.TryParse(numberStr, out int index))
					{
						Talespire.Log.Error($"Unable to parse \"{numberStr}\" into a number.");
						index = 0;
					}

					if (index >= 0 && index < superPersistentEffect.EffectProperties.Count)
					{
						EffectProperties effectProperties = superPersistentEffect.EffectProperties[index];
						ChangeProperty(effectProperties.Properties, propertyKey, propertyPath, propertyModDetails, propertyChanger, logDetails);
					}
					else
						Talespire.Log.Error($"index ({index}) out of range. Unable to modify property.");
				}
				else
				{
					ChangeProperty(ea.PersistentEffect.Properties, propertyKey, propertyPath, propertyModDetails, propertyChanger, logDetails);
				}
			}
		}

		private static void ChangeProperty(Dictionary<string, string> properties, string propertyKey, string propertyPath, PropertyModDetails propertyModDetails, BasePropertyChanger propertyChanger, bool logDetails)
		{
			propertyChanger.Value = properties[propertyKey];

			if (logDetails)
				Talespire.Log.Warning($"Setting {propertyPath} to \"{propertyChanger.GetValue()}\"");

			propertyModDetails.SetValue(propertyChanger);
		}

		static void PersistentEffects_PersistentEffectInitialized(object sender, PersistentEffectEventArgs ea)
		{
			// Guards are needed when we duplicate...
			if (Guard.IsNull(ea, nameof(ea))) return;
			if (Guard.IsNull(ea.PersistentEffect, nameof(ea.PersistentEffect))) return;
			if (Guard.IsNull(ea.CreatureAsset, nameof(ea.CreatureAsset))) return;
			if (Guard.IsNull(ea.AttachedNode, nameof(ea.AttachedNode))) return;
			//Talespire.Log.Debug($"All guarded data is good");

			Talespire.PersistentEffects.SetSpinLockVisible(ea.CreatureAsset, ea.PersistentEffect.RotationIsLocked && ea.CreatureAsset.IsVisible);
			ea.PersistentEffect.Initialize(ea.CreatureAsset);

			if (ea.PersistentEffect is SuperPersistentEffect superPersistentEffect)
			{
				for (int i = 0; i < superPersistentEffect.EffectProperties.Count; i++)
				{
					string prefix = i.ToString().PadLeft(2, '0');
					GameObject childNode = ea.AttachedNode.GetChildNodeStartingWith(prefix);
					CompositeEffect originalCompositeEffect = null;

					if (childNode == null)
					{
						Talespire.Log.Error($"childNode == null - unable to find child node starting with {prefix}");
						Talespire.Log.Hierarchy(ea.AttachedNode);
					}
					else
						originalCompositeEffect = CompositeEffect.GetFromGameObject(childNode);

					if (originalCompositeEffect == null)
						Talespire.Log.Warning($"Did not find a CompositeEffect for \"{childNode}\"!!!");

					EffectProperties effectProperties = superPersistentEffect.EffectProperties[i];

					foreach (string propertyPath in effectProperties.Properties.Keys)
					{

						if (propertyPath.StartsWith(STR_SmartPropertyPrefix))  // SmartProperty
						{
							if (originalCompositeEffect == null)
							{
								//Talespire.Log.Error($"DID NOT FIND a CompositeEffect for {childNode}!!! Unable to get smart properties from it!");
								break;
							}
							string smartPropertyName = propertyPath.Substring(1);
							SmartProperty smartProperty = originalCompositeEffect.SmartProperties?.FirstOrDefault(x => x.Name == smartPropertyName);
							if (smartProperty == null)
							{
								Talespire.Log.Error($"smartProperty {smartPropertyName} NOT found!!!");
							}
							else
							{
								foreach (string smartPropertyPath in smartProperty.PropertyPaths)
								{
									string propertyKey = propertyPath;
									//Talespire.Log.Debug($"ModifyProperty(ea, {smartPropertyPath}, {prefix}, {propertyKey});");
									ModifyProperty(ea, smartPropertyPath, prefix, propertyKey, true);
								}
							}
						}
						else
						{
							//Talespire.Log.Debug($"ModifyProperty(ea, {propertyPath}, {prefix});");
							ModifyProperty(ea, propertyPath, prefix, null, true);
						}
					}
				}
			}
			else
				foreach (string propertyPath in ea.PersistentEffect.Properties.Keys)
					ModifyProperty(ea, propertyPath, null, null, true);
		}

		static void RemoveCharacterMenu(string menuText)
		{
			RadialUI.RadialUIPlugin.RegisterRemoveCharacter(menuText, IsPersistentEffect);
		}

		static void RemoveGmMenu(string menuText)
		{
			RadialUI.RadialUIPlugin.RegisterRemoveSubmenuGm(menuText, IsPersistentEffect);
		}

		static void SetHidden(bool value)
		{
			CreatureBoardAsset creatureAtMenu = RadialUI.RadialUIPlugin.CreatureAtMenu;
			if (creatureAtMenu == null)
			{
				Talespire.Log.Error($"creatureAtMenu == null");
				return;
			}

			IOldPersistentEffect persistentEffect = creatureAtMenu.GetPersistentEffect();
			if (persistentEffect == null)
			{
				Talespire.Log.Error($"persistentEffect == null");
				return;
			}

			Talespire.PersistentEffects.SetHidden(creatureAtMenu, value);
		}

		static void SetRotationLock(bool value)
		{
			CreatureBoardAsset creatureAtMenu = RadialUI.RadialUIPlugin.CreatureAtMenu;
			if (creatureAtMenu == null)
			{
				Talespire.Log.Error($"SetRotationLock - creatureAtMenu == null");
				return;
			}

			IOldPersistentEffect persistentEffect = creatureAtMenu.GetPersistentEffect();
			if (persistentEffect == null)
			{
				Talespire.Log.Error($"SetRotationLock - persistentEffect == null");
				return;
			}

			Talespire.PersistentEffects.SetRotationLocked(creatureAtMenu, value);
		}

		static bool ShouldShowHideOrbMenuItem(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			if (!Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId))
				return false;
			return !Talespire.PersistentEffects.IsPersistentEffectHidden(contextCharacterId);
		}

		static bool ShouldShowLockRotationMenuItem(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			if (!Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId))
				return false;
			return !Talespire.PersistentEffects.IsPersistentEffectRotationLocked(contextCharacterId);
		}

		static bool ShouldShowRevealOrbMenuItem(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			if (!Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId))
				return false;
			return Talespire.PersistentEffects.IsPersistentEffectHidden(contextCharacterId);
		}

		static bool ShouldShowUnlockRotationMenuItem(NGuid selectedCharacterId, NGuid contextCharacterId)
		{
			if (!Talespire.PersistentEffects.IsPersistentEffect(contextCharacterId))
				return false;
			return Talespire.PersistentEffects.IsPersistentEffectRotationLocked(contextCharacterId);
		}

		static void ShowOrb(MapMenuItem menuItem, object arg2)
		{
			SetHidden(false);
		}

		static void ShowPersistentEffectUI(CreatureBoardAsset mini)
		{
			if (mini == null)
			{
				Talespire.Log.Error($"mini is NULL! - exiting!");
				return;
			}

			if (frmPropertyList != null)
				HidePersistentEffectUI();

			frmPropertyList = new FrmPropertyList();
			GameObject parentForAttachedObjects = mini.GetAttachedParentGameObject();
			if (parentForAttachedObjects == null)
			{
				Talespire.Log.Error($"parentForAttachedObjects is NULL! - exiting!");
				return;
			}

			IEnumerable<Transform> children = parentForAttachedObjects.transform.Children();
			if (children == null)
			{
				Talespire.Log.Error($"children is NULL! - exiting!");
				return;
			}

			// TODO: Add support for multiple child nodes (UI to select the attached effect).
			frmPropertyList.Instance = parentForAttachedObjects.GetChildNodeStartingWith("00");

			CompositeEffect originalCompositeEffect = null;
			foreach (Transform child in children)
			{
				originalCompositeEffect = CompositeEffect.GetFromGameObject(child.gameObject);
				if (originalCompositeEffect != null)
				{
					frmPropertyList.Instance = child.gameObject;
					break;
				}
			}

			frmPropertyList.Mini = mini;
			frmPropertyList.ClearProperties();

			if (originalCompositeEffect?.SuppressPosition != true)
				frmPropertyList.AddProperty("Position", typeof(Vector3), "<Transform>.localPosition");

			if (originalCompositeEffect?.SuppressRotation != true)
				frmPropertyList.AddProperty("Rotation", typeof(Vector3), "<Transform>.localEulerAngles");

			if (originalCompositeEffect?.SuppressScale != true)
				frmPropertyList.AddProperty("Scale", typeof(Vector3), "<Transform>.localScale");

			if (originalCompositeEffect == null)
				Talespire.Log.Warning($"We DID NOT FIND the CompositeEffect!!!");
			else
			{
				if (originalCompositeEffect.SmartProperties != null)  // Add SmartProperties (which we can get from the Composite).
					foreach (SmartProperty smartProperty in originalCompositeEffect.SmartProperties)
					{
						Type type = TypeLookup.GetType(smartProperty.Type);
						if (type == null)
							if (smartProperty.Type == "Enum")
							{
								Talespire.Log.Error($"Enum types not supported yet! Fix this!!!");
								continue;
							}
							else
							{
								Talespire.Log.Error($"type == null. Unable to add SmartProperty!!!");
								continue;
							}

						frmPropertyList.AddProperty(STR_SmartPropertyPrefix + smartProperty.Name, type, string.Join(";", smartProperty.PropertyPaths.ToArray()));
					}

				SuperPersistentEffect superPersistentEffect = mini.GetPersistentEffect() as SuperPersistentEffect;

				if (superPersistentEffect != null)
				{
					//Talespire.Log.Debug($"Looking for scripts...");
					foreach (string key in superPersistentEffect.ScriptData.Keys)
					{
						//Talespire.Log.Warning($"  ScriptData[{key}] == \"{superPersistentEffect.ScriptData[key]}\"");
						Type scriptType = KnownScripts.GetType(key);
						if (scriptType == null)
						{
							Talespire.Log.Error($"scriptType is null!!!");
							continue;
						}

						frmPropertyList.AddProperty($"<{scriptType.Name}>", scriptType, $"<{scriptType.FullName}>");
					}
				}
			}

			frmPropertyList.Show();
		}

		static void ShowPropertyEditor(MapMenuItem menuItem, object arg2)
		{
			Talespire.Log.Warning($"ShowPropertyEditor: {menuItem}, {arg2}");
			Talespire.Log.Warning($"CreatureAtMenu: {RadialUI.RadialUIPlugin.CreatureAtMenu}");
		}

		static void UnlockRotation(MapMenuItem menuItem, object arg2)
		{
			SetRotationLock(false);
		}

		public static void Initialize()
		{
			LoadMenuMods();
			Talespire.PersistentEffects.PersistentEffectInitialized += PersistentEffects_PersistentEffectInitialized;
			//BoardSessionManager.OnClientSelectedCreatureChange += BoardSessionManager_OnClientSelectedCreatureChange;
			Talespire.Minis.NewMiniSelected += Minis_NewMiniSelected;
		}

		private static void Minis_NewMiniSelected(object sender, CreatureBoardAssetEventArgs ea)
		{
			Minis_MiniSelected(sender, ea);
		}

		//private static void BoardSessionManager_OnClientSelectedCreatureChange(ClientGuid arg1, CreatureGuid arg2)
		//{
		//	CreatureBoardAsset mini = Talespire.Minis.GetCreatureBoardAsset(arg2.ToString());
		//	Talespire.Log.Warning($"ClientSelectedCreatureChange: {mini.GetOnlyCreatureName()}!");
		//	
		//	Minis_MiniSelected(null, new CreatureBoardAssetEventArgs() { Mini = mini });
		//}

		public static void LoadMenuMods()
		{
			AddCharacterMenu("Duplicate", "Duplicate.png", DuplicateEffectAtMenu);
			AddCharacterMenu("Properties...", "IconEdit.png", ShowPropertyEditor);
			AddCharacterMenu("Lock Rotation", "LockRotation.png", LockRotation, ShouldShowLockRotationMenuItem);
			AddCharacterMenu("Unlock Rotation", "UnlockRotation.png", UnlockRotation, ShouldShowUnlockRotationMenuItem);
			AddCharacterMenu("Hide Base", "HideShowOrb.png", HideOrb, ShouldShowHideOrbMenuItem);
			AddCharacterMenu("Reveal Base", "HideShowOrb.png", ShowOrb, ShouldShowRevealOrbMenuItem);

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
	}
}
