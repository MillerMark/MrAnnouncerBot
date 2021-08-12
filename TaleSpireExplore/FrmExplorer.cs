using Newtonsoft.Json;
using UnityEngine;
using TaleSpireCore;
using TaleSpireExplore;
using System;
using System.Collections.Generic;
using Runtime.Scripts;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace TaleSpireExplore
{
	public partial class FrmExplorer : Form
	{
		const string STR_SpellTestId = "SpellTest";
		public FrmExplorer()
		{
			Talespire.Log.Debug($"FrmExplorer constructor...");
			InitializeComponent();
			RegisterEffects();
		}

		void LogEvent(string message)
		{
			try
			{
				tbxLog.Text += message + Environment.NewLine;
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		void LogCreatureAsset(CreatureBoardAsset creatureAsset)
		{
			tbxLog.Text += "--------------" + Environment.NewLine;
			tbxLog.Text += $"{TaleSpireUtils.GetName(creatureAsset)}: \n";
			tbxLog.Text += $"  BoardAssetId: {creatureAsset.BoardAssetId}\n";
			tbxLog.Text += $"  WorldId: {creatureAsset.WorldId}\n";
			tbxLog.Text += $"  Position: {creatureAsset.PlacedPosition}\n";
			tbxLog.Text += $"  Scale: {creatureAsset.CreatureScale}\n";
			if (creatureAsset.IsGrounded)
				tbxLog.Text += $"  IsGrounded: true\n";
			if (creatureAsset.FlyingIndicator != null && creatureAsset.FlyingIndicator.ElevationAmount != 0)
				tbxLog.Text += $"  Flying altitude: {creatureAsset.FlyingIndicator.ElevationAmount} feet\n";
			tbxLog.Text += $"  IsVisible: {creatureAsset.IsVisible}\n";
			tbxLog.Text += Environment.NewLine;
			//tbxLog.Text += $"  HookSpellCast: {creatureAsset.HookSpellCast}\n";
		}

		void HookEvents()
		{
			PhotonNetwork.OnEventCall += PhotonNetwork_OnEventCall;
			ActiveCameraManager.OnActiveCameraChanged += ActiveCameraManager_OnActiveCameraChanged;
			AtmosphereManager.OnAtmosphereDataChanged += AtmosphereManager_OnAtmosphereDataChanged;
			AtmosphereManager.OnEditStatusChanged += AtmosphereManager_OnEditStatusChanged;
			BackendSocketClient.OnClose += BackendSocketClient_OnClose;
			BackendSocketClient.OnLostConnection += BackendSocketClient_OnLostConnection;
			BackendSocketClient.OnOpen += BackendSocketClient_OnOpen;
			BackendSocketClient.OnStateChange += BackendSocketClient_OnStateChange;
			BackendSocketClient.OnStateWillTransition += BackendSocketClient_OnStateWillTransition;
			BoardSessionManager.OnBoardInfoChanged += BoardSessionManager_OnBoardInfoChanged;
			BoardSessionManager.OnBoardPresentationChanged += BoardSessionManager_OnBoardPresentationChanged;
			BoardSessionManager.OnClientListChange += BoardSessionManager_OnClientListChange;
			BoardSessionManager.OnClientSelectedCreatureChange += BoardSessionManager_OnClientSelectedCreatureChange;
			BoardSessionManager.OnCurrentBoardChanged += BoardSessionManager_OnCurrentBoardChanged;
			BoardSessionManager.OnLocalClientModeChange += BoardSessionManager_OnLocalClientModeChange;
			BoardSessionManager.OnStateChange += BoardSessionManager_OnStateChange;
			BoardSessionManager.OnStateWillTransition += BoardSessionManager_OnStateWillTransition;
			BoardBuilder.OnBuildModeToggle += BoardBuilder_OnBuildModeToggle;
			BuildPlane.OnBuildHeightChange += BuildPlane_OnBuildHeightChange;
			BuildPlane.OnBuildHeightPropChange += BuildPlane_OnBuildHeightPropChange;
			BuildPlane.OnEnabledChange += BuildPlane_OnEnabledChange;
			BoardToolManager.OnSwitchTool += BoardToolManager_OnSwitchTool;
			BuildingBoardTool.OnBuildHistoryChange += BuildingBoardTool_OnBuildHistoryChange;
			RulerBoardTool.OnCloseRulers += RulerBoardTool_OnCloseRulers;
			RulerBoardTool.OnRulerIndicatorChange += RulerBoardTool_OnRulerIndicatorChange;
			CameraController.OnCutBoxChange += CameraController_OnCutBoxChange;
			CampaignSessionManager.OnPlayerInfoUpdated += CampaignSessionManager_OnPlayerInfoUpdated;
			CampaignSessionManager.OnEnteringBuildSceneForCampaign += CampaignSessionManager_OnEnteringBuildSceneForCampaign;
			CampaignSessionManager.OnCampaignUpgradeOrDowngrade += CampaignSessionManager_OnCampaignUpgradeOrDowngrade;
			CampaignSessionManager.OnDistanceUnitsChanged += CampaignSessionManager_OnDistanceUnitsChanged;
			CampaignSessionManager.OnBoardListUpdated += CampaignSessionManager_OnBoardListUpdated;
			CampaignSessionManager.OnCampaignInfoUpdated += CampaignSessionManager_OnCampaignInfoUpdated;
			CampaignSessionManager.OnCampaignUniquesChanged += CampaignSessionManager_OnCampaignUniquesChanged;
			CampaignSessionManager.OnStateChange += CampaignSessionManager_OnStateChange;
			CampaignSessionManager.OnStateWillTransition += CampaignSessionManager_OnStateWillTransition;
			CampaignSessionManager.OnStatNamesChange += CampaignSessionManager_OnStatNamesChange;
			InGameCompass.OnCompassEnabledChanged += InGameCompass_OnCompassEnabledChanged;
			CreatureManager.CreatureOwnershipChanged += CreatureManager_CreatureOwnershipChanged;
			CreatureManager.OnLineOfSightUpdated += CreatureManager_OnLineOfSightUpdated;
			CreatureManager.OnRequiresSyncStatusChanged += CreatureManager_OnRequiresSyncStatusChanged;
			CutsceneManager.OnCutscenesChanged += CutsceneManager_OnCutscenesChanged;
			CutsceneManager.OnGMBlockChanged += CutsceneManager_OnGMBlockChanged;
			GUIManager.OnGUIManagerInitalized += GUIManager_OnGUIManagerInitialized;
			GameSettings.OnScreenResolutionChange += GameSettings_OnScreenResolutionChange;
			GameSettings.OnUIScaleChange += GameSettings_OnUIScaleChange;
			InitiativeManager.OnTurnQueueEditUpdate += InitiativeManager_OnTurnQueueEditUpdate;
			InitiativeManager.OnTurnQueueUpdate += InitiativeManager_OnTurnQueueUpdate;
			InitiativeManager.OnTurnSwitch += InitiativeManager_OnTurnSwitch;
			ControllerManager.OnBuildTogglePress += ControllerManager_OnBuildTogglePress;
			ControllerManager.OnEscapePress += ControllerManager_OnEscapePress;
			ControllerManager.OnToolHotKeyPush += ControllerManager_OnToolHotKeyPush;
			MouseManager.OnApplicationFocusChange += MouseManager_OnApplicationFocusChange;
			LocalClient.OnIsGmStatusChange += LocalClient_OnIsGmStatusChange;
			LocalPlayer.OnRightsUpdated += LocalPlayer_OnRightsUpdated;
			PartyManager.OnPartyGmSet += PartyManager_OnPartyGmSet;
			PhotoMode.OnPhotoModeChangeFromTo += PhotoMode_OnPhotoModeChangeFromTo;
			PhotoMode.OnPhotoModeToggle += PhotoMode_OnPhotoModeToggle;
			PublishedBoardManager.OnPublishedBoardListChanged += PublishedBoardManager_OnPublishedBoardListChanged;
			WaterPlane.BuildDisplayToggleEvent += WaterPlane_BuildDisplayToggleEvent;
			WaterPlane.EnterWaterEvent += WaterPlane_EnterWaterEvent;
			WaterPlane.WaterLevelChangeEvent += WaterPlane_WaterLevelChangeEvent;
			BoardNavigationManager.OnNorthChanged += BoardNavigationManager_OnNorthChanged;
			BoardWaterManager.OnChangeWaterState += BoardWaterManager_OnChangeWaterState;
			SingletonStateMBehaviour.OnStateChange += SingletonStateMBehaviour_OnStateChange;
			SingletonStateMBehaviour.OnStateWillTransition += SingletonStateMBehaviour_OnStateWillTransition;
		}

		void UnhookEvents()
		{
			PhotonNetwork.OnEventCall -= PhotonNetwork_OnEventCall;
			ActiveCameraManager.OnActiveCameraChanged -= ActiveCameraManager_OnActiveCameraChanged;
			AtmosphereManager.OnAtmosphereDataChanged -= AtmosphereManager_OnAtmosphereDataChanged;
			AtmosphereManager.OnEditStatusChanged -= AtmosphereManager_OnEditStatusChanged;
			BackendSocketClient.OnClose -= BackendSocketClient_OnClose;
			BackendSocketClient.OnLostConnection -= BackendSocketClient_OnLostConnection;
			BackendSocketClient.OnOpen -= BackendSocketClient_OnOpen;
			BackendSocketClient.OnStateChange -= BackendSocketClient_OnStateChange;
			BackendSocketClient.OnStateWillTransition -= BackendSocketClient_OnStateWillTransition;
			BoardSessionManager.OnBoardInfoChanged -= BoardSessionManager_OnBoardInfoChanged;
			BoardSessionManager.OnBoardPresentationChanged -= BoardSessionManager_OnBoardPresentationChanged;
			BoardSessionManager.OnClientListChange -= BoardSessionManager_OnClientListChange;
			BoardSessionManager.OnClientSelectedCreatureChange -= BoardSessionManager_OnClientSelectedCreatureChange;
			BoardSessionManager.OnCurrentBoardChanged -= BoardSessionManager_OnCurrentBoardChanged;
			BoardSessionManager.OnLocalClientModeChange -= BoardSessionManager_OnLocalClientModeChange;
			BoardSessionManager.OnStateChange -= BoardSessionManager_OnStateChange;
			BoardSessionManager.OnStateWillTransition -= BoardSessionManager_OnStateWillTransition;
			BoardBuilder.OnBuildModeToggle -= BoardBuilder_OnBuildModeToggle;
			BuildPlane.OnBuildHeightChange -= BuildPlane_OnBuildHeightChange;
			BuildPlane.OnBuildHeightPropChange -= BuildPlane_OnBuildHeightPropChange;
			BuildPlane.OnEnabledChange -= BuildPlane_OnEnabledChange;
			BoardToolManager.OnSwitchTool -= BoardToolManager_OnSwitchTool;
			BuildingBoardTool.OnBuildHistoryChange -= BuildingBoardTool_OnBuildHistoryChange;
			RulerBoardTool.OnCloseRulers -= RulerBoardTool_OnCloseRulers;
			RulerBoardTool.OnRulerIndicatorChange -= RulerBoardTool_OnRulerIndicatorChange;
			CameraController.OnCutBoxChange -= CameraController_OnCutBoxChange;
			CampaignSessionManager.OnPlayerInfoUpdated -= CampaignSessionManager_OnPlayerInfoUpdated;
			CampaignSessionManager.OnEnteringBuildSceneForCampaign -= CampaignSessionManager_OnEnteringBuildSceneForCampaign;
			CampaignSessionManager.OnCampaignUpgradeOrDowngrade -= CampaignSessionManager_OnCampaignUpgradeOrDowngrade;
			CampaignSessionManager.OnDistanceUnitsChanged -= CampaignSessionManager_OnDistanceUnitsChanged;
			CampaignSessionManager.OnBoardListUpdated -= CampaignSessionManager_OnBoardListUpdated;
			CampaignSessionManager.OnCampaignInfoUpdated -= CampaignSessionManager_OnCampaignInfoUpdated;
			CampaignSessionManager.OnCampaignUniquesChanged -= CampaignSessionManager_OnCampaignUniquesChanged;
			CampaignSessionManager.OnStateChange -= CampaignSessionManager_OnStateChange;
			CampaignSessionManager.OnStateWillTransition -= CampaignSessionManager_OnStateWillTransition;
			CampaignSessionManager.OnStatNamesChange -= CampaignSessionManager_OnStatNamesChange;
			InGameCompass.OnCompassEnabledChanged -= InGameCompass_OnCompassEnabledChanged;
			CreatureManager.CreatureOwnershipChanged -= CreatureManager_CreatureOwnershipChanged;
			CreatureManager.OnLineOfSightUpdated -= CreatureManager_OnLineOfSightUpdated;
			CreatureManager.OnRequiresSyncStatusChanged -= CreatureManager_OnRequiresSyncStatusChanged;
			CutsceneManager.OnCutscenesChanged -= CutsceneManager_OnCutscenesChanged;
			CutsceneManager.OnGMBlockChanged -= CutsceneManager_OnGMBlockChanged;
			GUIManager.OnGUIManagerInitalized -= GUIManager_OnGUIManagerInitialized;
			GameSettings.OnScreenResolutionChange -= GameSettings_OnScreenResolutionChange;
			GameSettings.OnUIScaleChange -= GameSettings_OnUIScaleChange;
			InitiativeManager.OnTurnQueueEditUpdate -= InitiativeManager_OnTurnQueueEditUpdate;
			InitiativeManager.OnTurnQueueUpdate -= InitiativeManager_OnTurnQueueUpdate;
			InitiativeManager.OnTurnSwitch -= InitiativeManager_OnTurnSwitch;
			ControllerManager.OnBuildTogglePress -= ControllerManager_OnBuildTogglePress;
			ControllerManager.OnEscapePress -= ControllerManager_OnEscapePress;
			ControllerManager.OnToolHotKeyPush -= ControllerManager_OnToolHotKeyPush;
			MouseManager.OnApplicationFocusChange -= MouseManager_OnApplicationFocusChange;
			LocalClient.OnIsGmStatusChange -= LocalClient_OnIsGmStatusChange;
			LocalPlayer.OnRightsUpdated -= LocalPlayer_OnRightsUpdated;
			PartyManager.OnPartyGmSet -= PartyManager_OnPartyGmSet;
			PhotoMode.OnPhotoModeChangeFromTo -= PhotoMode_OnPhotoModeChangeFromTo;
			PhotoMode.OnPhotoModeToggle -= PhotoMode_OnPhotoModeToggle;
			PublishedBoardManager.OnPublishedBoardListChanged -= PublishedBoardManager_OnPublishedBoardListChanged;
			WaterPlane.BuildDisplayToggleEvent -= WaterPlane_BuildDisplayToggleEvent;
			WaterPlane.EnterWaterEvent -= WaterPlane_EnterWaterEvent;
			WaterPlane.WaterLevelChangeEvent -= WaterPlane_WaterLevelChangeEvent;
			BoardNavigationManager.OnNorthChanged -= BoardNavigationManager_OnNorthChanged;
			BoardWaterManager.OnChangeWaterState -= BoardWaterManager_OnChangeWaterState;
			SingletonStateMBehaviour.OnStateChange -= SingletonStateMBehaviour_OnStateChange;
			SingletonStateMBehaviour.OnStateWillTransition -= SingletonStateMBehaviour_OnStateWillTransition;
		}

		private void SingletonStateMBehaviour_OnStateWillTransition(SingletonStateMBehaviour.State arg1, SingletonStateMBehaviour.State arg2)
		{
			LogEvent($"SingletonStateMBehaviour.OnStateWillTransition(arg1: {arg1}, arg2: {arg2})");
		}

		private void SingletonStateMBehaviour_OnStateChange(SingletonStateMBehaviour.State obj)
		{
			LogEvent($"SingletonStateMBehaviour.OnStateChange(obj: {obj})");
		}

		private void BoardWaterManager_OnChangeWaterState(DataModel.WaterState obj)
		{
			LogEvent($"BoardWaterManager.OnChangeWaterState(obj: {obj})");
		}

		private void BoardNavigationManager_OnNorthChanged(float obj)
		{
			LogEvent($"BoardNavigationManager.OnNorthChanged(obj: {obj})");
		}

		private void WaterPlane_WaterLevelChangeEvent(float obj)
		{
			LogEvent($"WaterPlane.WaterLevelChangeEvent(obj: {obj})");
		}

		private void WaterPlane_EnterWaterEvent(bool obj)
		{
			LogEvent($"WaterPlane.EnterWaterEvent(obj: {obj})");
		}

		private void WaterPlane_BuildDisplayToggleEvent(bool obj)
		{
			LogEvent($"WaterPlane.BuildDisplayToggleEvent(obj: {obj})");
		}

		private void PublishedBoardManager_OnPublishedBoardListChanged()
		{
			LogEvent($"PublishedBoardManager.OnPublishedBoardListChanged()");
		}

		private void PhotoMode_OnPhotoModeToggle(bool obj)
		{
			LogEvent($"PhotoMode.OnPhotoModeToggle(obj: {obj})");
		}

		private void PhotoMode_OnPhotoModeChangeFromTo(PhotoMode.CameraControlStyle arg1, PhotoMode.CameraControlStyle arg2)
		{
			LogEvent($"PhotoMode.OnPhotoModeChangeFromTo(arg1: {arg1}, arg2: {arg2})");
		}

		private void PartyManager_OnPartyGmSet(ClientGuid obj)
		{
			LogEvent($"PartyManager.OnPartyGmSet(obj: {obj})");
		}

		private void LocalPlayer_OnRightsUpdated()
		{
			LogEvent($"LocalPlayer.OnRightsUpdated()");
		}

		private void LocalClient_OnIsGmStatusChange(bool obj)
		{
			LogEvent($"LocalClient.OnIsGmStatusChange(obj: {obj})");
		}

		private void MouseManager_OnApplicationFocusChange(bool obj)
		{
			LogEvent($"MouseManager.OnApplicationFocusChange(obj: {obj})");
		}

		private void ControllerManager_OnToolHotKeyPush(int obj)
		{
			LogEvent($"ControllerManager.OnToolHotKeyPush(obj: {obj})");
		}

		private void ControllerManager_OnEscapePress()
		{
			LogEvent($"ControllerManager.OnEscapePress()");
		}

		private void ControllerManager_OnBuildTogglePress()
		{
			LogEvent($"ControllerManager.OnBuildTogglePress()");
		}

		private void InitiativeManager_OnTurnSwitch(CreatureGuid obj)
		{
			LogEvent($"InitiativeManager.OnTurnSwitch(obj: {obj})");
		}

		private void InitiativeManager_OnTurnQueueUpdate(List<CreatureGuid> obj)
		{
			LogEvent($"InitiativeManager.OnTurnQueueUpdate(obj: {obj})");
		}

		private void InitiativeManager_OnTurnQueueEditUpdate(List<CreatureGuid> obj)
		{
			LogEvent($"InitiativeManager.OnTurnQueueEditUpdate(obj: {obj})");
		}

		private void GameSettings_OnUIScaleChange(float obj)
		{
			LogEvent($"GameSettings.OnUIScaleChange(obj: {obj})");
		}

		private void GameSettings_OnScreenResolutionChange(int arg1, int arg2)
		{
			LogEvent($"GameSettings.OnScreenResolutionChange(arg1: {arg1}, arg2: {arg2})");
		}

		private void GUIManager_OnGUIManagerInitialized()
		{
			LogEvent($"GUIManager.OnGUIManagerInitalized()");
		}

		private void CutsceneManager_OnGMBlockChanged(DataModel.CinematicBlock arg1, bool arg2)
		{
			LogEvent($"CutsceneManager.OnGMBlockChanged(arg1: {arg1}, arg2: {arg2})");
		}

		private void CutsceneManager_OnCutscenesChanged()
		{
			LogEvent($"CutsceneManager.OnCutscenesChanged()");
		}

		private void CreatureManager_OnRequiresSyncStatusChanged(bool obj)
		{
			LogEvent($"CreatureManager.OnRequiresSyncStatusChanged(obj: {obj})");
		}

		private void CreatureManager_OnLineOfSightUpdated(CreatureGuid arg1, LineOfSightManager.LineOfSightResult arg2)
		{
			if (CreaturePresenter.TryGetAsset(arg1, out CreatureBoardAsset creatureAsset))
				LogCreatureAsset(creatureAsset);
			LogEvent($"CreatureManager.OnLineOfSightUpdated(arg1: {arg1}, arg2: {arg2})");
		}

		private void CreatureManager_CreatureOwnershipChanged()
		{
			LogEvent($"CreatureManager.CreatureOwnershipChanged()");
		}

		private void InGameCompass_OnCompassEnabledChanged(bool obj)
		{
			LogEvent($"InGameCompass.OnCompassEnabledChanged(obj: {obj})");
		}

		private void CampaignSessionManager_OnStatNamesChange(string[] obj)
		{
			if (obj != null)
				LogEvent($"CampaignSessionManager.OnStatNamesChange(\"{string.Join(", ", obj.ToArray())}\")");
		}

		private void CampaignSessionManager_OnStateWillTransition(PhotonSimpleSingletonStateMBehaviour<CampaignSessionManager>.State arg1, PhotonSimpleSingletonStateMBehaviour<CampaignSessionManager>.State arg2)
		{
			LogEvent($"CampaignSessionManager.OnStateWillTransition(\"{arg1.Name}\", \"{arg2.Name}\")");
		}

		private void CampaignSessionManager_OnStateChange(PhotonSimpleSingletonStateMBehaviour<CampaignSessionManager>.State obj)
		{
			LogEvent($"CampaignSessionManager.OnStateChange(\"{obj.Name}\")");
		}

		private void CampaignSessionManager_OnCampaignUniquesChanged()
		{
			LogEvent($"CampaignSessionManager.OnCampaignUniquesChanged()");
		}

		private void CampaignSessionManager_OnCampaignInfoUpdated(CampaignInfo obj)
		{
			LogEvent($"CampaignSessionManager.OnCampaignInfoUpdated(obj: {obj})");
		}

		private void CampaignSessionManager_OnBoardListUpdated(CampaignSessionManager.BoardSummaries obj)
		{
			LogEvent($"CampaignSessionManager.OnBoardListUpdated(obj: {obj})");
		}

		private void CampaignSessionManager_OnDistanceUnitsChanged(DistanceUnit[] obj)
		{
			LogEvent($"CampaignSessionManager.OnDistanceUnitsChanged(obj: {obj})");
		}

		private void CampaignSessionManager_OnCampaignUpgradeOrDowngrade()
		{
			LogEvent($"CampaignSessionManager.OnCampaignUpgradeOrDowngrade()");
		}

		private void CampaignSessionManager_OnEnteringBuildSceneForCampaign(CampaignGuid obj)
		{
			LogEvent($"CampaignSessionManager.OnEnteringBuildSceneForCampaign(obj: {obj})");
		}

		private void CampaignSessionManager_OnPlayerInfoUpdated()
		{
			LogEvent($"CampaignSessionManager.OnPlayerInfoUpdated()");
		}

		private void CameraController_OnCutBoxChange(bool obj)
		{
			LogEvent($"CameraController.OnCutBoxChange(obj: {obj})");
		}

		private void RulerBoardTool_OnRulerIndicatorChange(int obj)
		{
			LogEvent($"RulerBoardTool.OnRulerIndicatorChange(obj: {obj})");
		}

		private void RulerBoardTool_OnCloseRulers()
		{
			LogEvent($"RulerBoardTool.OnCloseRulers()");
		}

		private void BuildingBoardTool_OnBuildHistoryChange(AssetDb.DbGroup obj)
		{
			LogEvent($"BuildingBoardTool.OnBuildHistoryChange(obj: {obj})");
		}

		private void BoardToolManager_OnSwitchTool(BoardTool obj)
		{
			LogEvent($"BoardToolManager.OnSwitchTool(obj: {obj})");
		}

		private void BuildPlane_OnEnabledChange(bool obj)
		{
			LogEvent($"BuildPlane.OnEnabledChange(obj: {obj})");
		}

		private void BuildPlane_OnBuildHeightPropChange(float obj)
		{
			LogEvent($"BuildPlane.OnBuildHeightPropChange(obj: {obj})");
		}

		private void BuildPlane_OnBuildHeightChange(float obj)
		{
			LogEvent($"BuildPlane.OnBuildHeightChange(obj: {obj})");
		}

		private void BoardBuilder_OnBuildModeToggle(bool obj)
		{
			LogEvent($"BoardBuilder.OnBuildModeToggle(obj: {obj})");
		}

		private void BoardSessionManager_OnStateWillTransition(PhotonSimpleSingletonStateMBehaviour<BoardSessionManager>.State arg1, PhotonSimpleSingletonStateMBehaviour<BoardSessionManager>.State arg2)
		{
			LogEvent($"BoardSessionManager.OnStateWillTransition(\"{arg1.Name}\", \"{arg2.Name}\")");
		}

		private void BoardSessionManager_OnStateChange(PhotonSimpleSingletonStateMBehaviour<BoardSessionManager>.State obj)
		{
			LogEvent($"BoardSessionManager.OnStateChange(\"{obj.Name}\")");
		}

		private void BoardSessionManager_OnLocalClientModeChange(ClientMode obj)
		{
			LogEvent($"BoardSessionManager.OnLocalClientModeChange(obj: {obj})");
		}

		private void BoardSessionManager_OnCurrentBoardChanged()
		{
			LogEvent($"BoardSessionManager.OnCurrentBoardChanged()");
		}

		private void BoardSessionManager_OnClientSelectedCreatureChange(ClientGuid arg1, CreatureGuid arg2)
		{
			LogEvent($"BoardSessionManager.OnClientSelectedCreatureChange(arg1: {arg1}, arg2: {arg2})");
		}

		private void BoardSessionManager_OnClientListChange()
		{
			LogEvent($"BoardSessionManager.OnClientListChange()");
		}

		private void BoardSessionManager_OnBoardPresentationChanged()
		{
			LogEvent($"BoardSessionManager.OnBoardPresentationChanged()");
		}

		private void BoardSessionManager_OnBoardInfoChanged(BoardInfo obj)
		{
			LogEvent($"BoardSessionManager.OnBoardInfoChanged(obj: {obj})");
		}

		private void BackendSocketClient_OnStateWillTransition(SingletonStateMBehaviour<BackendSocketClient>.State arg1, SingletonStateMBehaviour<BackendSocketClient>.State arg2)
		{
			LogEvent($"BackendSocketClient.OnStateWillTransition(arg1: {arg1}, arg2: {arg2})");
		}

		private void BackendSocketClient_OnStateChange(SingletonStateMBehaviour<BackendSocketClient>.State obj)
		{
			LogEvent($"BackendSocketClient.OnStateChange(obj: {obj})");
		}

		private void BackendSocketClient_OnOpen()
		{
			LogEvent($"BackendSocketClient.OnOpen()");
		}

		private void BackendSocketClient_OnLostConnection()
		{
			LogEvent($"BackendSocketClient.OnLostConnection()");
		}

		private void BackendSocketClient_OnClose()
		{
			LogEvent($"BackendSocketClient.OnClose()");
		}

		private void AtmosphereManager_OnEditStatusChanged(bool obj)
		{
			LogEvent($"AtmosphereManager.OnEditStatusChanged(obj: {obj})");
		}

		private void AtmosphereManager_OnAtmosphereDataChanged(TaleSpire.Atmosphere.AtmosphereReferenceData obj)
		{
			LogEvent($"AtmosphereManager.OnAtmosphereDataChanged(obj: {obj})");
		}

		private void ActiveCameraManager_OnActiveCameraChanged()
		{
			LogEvent($"ActiveCameraManager.OnActiveCameraChanged()");
		}

		private void PhotonNetwork_OnEventCall(byte eventCode, object content, int senderId)
		{
			LogEvent($"PhotonNetwork.OnEventCall(eventCode: {eventCode}, content: {content}, senderId: {senderId})");
		}

		void LoadCreatures()
		{
			try
			{
				CreatureBoardAsset[] allCreatureAssets = Talespire.Minis.GetAll();
				tbxScratch.Text = "";
				if (allCreatureAssets == null)
					return;
				foreach (CreatureBoardAsset creatureAsset in allCreatureAssets)
				{
					string uniqueIdStr = "";
					if (creatureAsset.Creature.IsUnique)
						uniqueIdStr = " UniqueId: {creatureAsset.Creature.UniqueId},";

					tbxScratch.Text += $"{TaleSpireUtils.GetName(creatureAsset)}: {creatureAsset.PlacedPosition},{uniqueIdStr} WorldId: {creatureAsset.WorldId}, BoardAssetId: {creatureAsset.BoardAssetId}\n"; // , Scale: {creatureAsset.CreatureScale}
				}
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception!");
			}
		}
		void LoadResources()
		{
			try
			{
				UnityEngine.Object[] resources = Resources.LoadAll("");
				tbxScratch.Text = "";
				foreach (UnityEngine.Object unityObj in resources)
					tbxScratch.Text += $"{unityObj.name}\n  Type: {unityObj.GetType().FullName}\n  ID: {unityObj.GetInstanceID()}\n\n";
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Exception!");
			}
		}
		private void btnLoadClasses_Click(object sender, EventArgs e)
		{
			LoadCreatures();
			//LoadResources();
			//SendBouncyApiToScratch()
		}

		void SendBouncyApiToScratch()
		{
			string GetSimpleType(string typeStr)
			{
				if (typeStr == "System.Boolean")
					return "bool";
				if (typeStr == "System.Void")
					return "void";
				if (typeStr == "System.Int32")
					return "int";
				if (typeStr == "System.Single")
					return "float";
				if (typeStr == "System.Decimal")
					return "decimal";
				return typeStr;
			}

			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					if (assembly.FullName.StartsWith("Bouncy"))
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine($"{assembly.GetName()}");
						Type[] types = assembly.GetTypes();
						foreach (Type type in types)
						{
							string namespaceStr = "";
							if (!string.IsNullOrEmpty(type.Namespace))
								namespaceStr = $"{type.Namespace}.";

							if (type.IsPublic)
								stringBuilder.AppendLine($"  public {namespaceStr}{type.FullName}");
							else
								stringBuilder.AppendLine($"  internal {namespaceStr}{type.FullName}");
							MemberInfo[] members = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
							foreach (MemberInfo memberInfo in members)
							{
								if (memberInfo.Name.StartsWith(".") || memberInfo.Name.StartsWith("<>"))
									continue;
								string suffix = "";
								string prefix = "";
								if (memberInfo is MethodInfo methodInfo)
								{
									prefix = GetSimpleType(methodInfo.ReturnType.ToString());
									ParameterInfo[] parameters = methodInfo.GetParameters();
									suffix = "(";
									foreach (ParameterInfo parameterInfo in parameters)
									{
										suffix += $"{GetSimpleType(parameterInfo.ParameterType.ToString())} {parameterInfo.Name}, ";
									}
									suffix = suffix.TrimEnd(' ', ',');
									suffix += ")";
								}
								else if (memberInfo is PropertyInfo propInfo)
								{
									prefix = GetSimpleType(propInfo.PropertyType.ToString());
									suffix = " { get; set; }";
								}
								else if (memberInfo is FieldInfo fieldInfo)
								{
									prefix = GetSimpleType(fieldInfo.FieldType.ToString());
								}
								else if (memberInfo is EventInfo eventInfo)
								{
									prefix = $"event {eventInfo.EventHandlerType}";
								}
								else
								{
									suffix = $" ({memberInfo.MemberType.ToString().ToLower()})";
								}

								if (prefix != "")
									prefix = prefix + " ";

								stringBuilder.AppendLine($"    {prefix}{memberInfo.Name}{suffix};");
							}
						}
					}
				}
				tbxScratch.Text = stringBuilder.ToString();
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		private void btnSetTime_Click(object sender, EventArgs e)
		{
			if (float.TryParse(txtTime.Text, out float normalizedTime))
				AtmosphereManager.SetTimeOfDay(normalizedTime);
		}

		private void btnSetSun_Click(object sender, EventArgs e)
		{
			if (float.TryParse(txtSunDirection.Text, out float sunDirection))
				AtmosphereManager.SetSunDirection(sunDirection);
		}

		private void FrmExplorer_FormClosing(object sender, FormClosingEventArgs e)
		{
			UnhookEvents();
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			tbxLog.Text = "";

		}


		private void btnEffects_Click(object sender, EventArgs e)
		{
			try
			{
				tbxScratch.Text += Talespire.Effects.GetList();
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		private void btnShowEffect_Click(object sender, EventArgs e)
		{
			string effectName = txtEffectName.Text.Trim();

			float x = 15f;
			float y = 22.5f;
			float z = 95.4f;
			Vector3 location = new Vector3(x, y, z);
			try
			{
				EffectManager.PlayEffect(effectName, location);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				//while (ex != null)
				//{
				//	MessageBox.Show(ex.Message, "Exception!");
				//	MessageBox.Show(ex.StackTrace, "StackTrace!");
				//	ex = ex.InnerException;
				//}
			}

		}

		void AddScratchLine(string str)
		{
			tbxScratch.Text += str + Environment.NewLine;
		}

		private void btnShowRelationEffect_Click(object sender, EventArgs e)
		{
			float x = 15f;
			float y = 22.5f;
			float z = 95.4f;
			try
			{
				string effectName = txtEffectName.Text.Trim();
				// We have to jump through some hoops here to set the position (transforms can only be created inside a GameObject).
				GameObject origin = new GameObject();
				GameObject target = new GameObject();
				origin.transform.position = new Vector3(x, y, z);
				target.transform.position = new Vector3(x + 4, y, z + 3);
				VisualEffect visualEffect = Talespire.Effects.GetVisual(effectName);

				if (visualEffect != null)
				{
					try
					{
						AnimationCurve animationCurve = ReflectionHelper.GetNonPublicField<AnimationCurve>(visualEffect, "animCurve");
						if (animationCurve != null)
						{
							Talespire.Log.Debug($"animationCurve.length: {animationCurve.length}");
							Talespire.Log.Debug($"animationCurve.postWrapMode: {animationCurve.postWrapMode}");
							Talespire.Log.Debug($"animationCurve.preWrapMode: {animationCurve.preWrapMode}");
							for (int i = 0; i < animationCurve.keys.Length; i++)
							{
								Keyframe keyframe = animationCurve.keys[i];
								Talespire.Log.Debug($"{i}: time: {keyframe.time}, value: {keyframe.value}, inTangent: {keyframe.inTangent}, outTangent: {keyframe.outTangent}, inWeight: {keyframe.inWeight}, outWeight: {keyframe.outWeight}, tangentMode: {keyframe.tangentMode}");
							}
							Talespire.Log.Debug($"");
						}
						else
							Talespire.Log.Error($"Animation Curve not found!");
					}
					catch (Exception ex)
					{
						Talespire.Log.Exception(ex);
					}

					if (visualEffect is VFXMissile missile)
					{
						//ParticleSystem drizzle = ReflectionHelper.GetNonPublicField<ParticleSystem>(typeof(VFXMissile), missile, "drizzle");
						//if (drizzle == null)
						//	tbxScratch.Text += $"drizzle was NOT found!" + Environment.NewLine;
						//else
						//{
						//	tbxScratch.Text += $"drizzle was found!" + Environment.NewLine;
						//	AddScratchLine($"  proceduralSimulationSupported: {drizzle.proceduralSimulationSupported}");
						//	AddScratchLine($"  collision: {drizzle.collision}");
						//	AddScratchLine($"  colorBySpeed: {drizzle.colorBySpeed}");
						//	AddScratchLine($"  colorOverLifetime: {drizzle.colorOverLifetime}");
						//	AddScratchLine($"  customData: {drizzle.customData}");
						//	AddScratchLine($"  emission.burstCount: {drizzle.emission.burstCount}");
						//	AddScratchLine($"  emission.emission.enabled: {drizzle.emission.enabled}");
						//	AddScratchLine($"  emission.emission.rateOverDistance: {drizzle.emission.rateOverDistance}");
						//	AddScratchLine($"  emission.emission.rateOverDistanceMultiplier: {drizzle.emission.rateOverDistanceMultiplier}");
						//	AddScratchLine($"  emission.emission.rateOverTime: {drizzle.emission.rateOverTime}");
						//	AddScratchLine($"  emission.emission.rateOverTimeMultiplier: {drizzle.emission.rateOverTimeMultiplier}");
						//	AddScratchLine($"  externalForces: {drizzle.externalForces}");
						//	AddScratchLine($"  forceOverLifetime: {drizzle.forceOverLifetime}");
						//	AddScratchLine($"  inheritVelocity: {drizzle.inheritVelocity}");
						//	AddScratchLine($"  isEmitting: {drizzle.isEmitting}");
						//	AddScratchLine($"  isPaused: {drizzle.isPaused}");
						//	AddScratchLine($"  isStopped: {drizzle.isStopped}");
						//	AddScratchLine($"  lifetimeByEmitterSpeed: {drizzle.lifetimeByEmitterSpeed}");
						//	AddScratchLine($"  lights: {drizzle.lights}");
						//	AddScratchLine($"  limitVelocityOverLifetime: {drizzle.limitVelocityOverLifetime}");

						//	AddScratchLine($"  main.duration: {drizzle.main.duration}");
						//	AddScratchLine($"  main.gravityModifier: {drizzle.main.gravityModifier}");
						//	AddScratchLine($"  main.loop: {drizzle.main.loop}");


						//	// None of these tries work to change the color:
						//	ParticleSystem.ColorOverLifetimeModule col = drizzle.colorOverLifetime;
						//	col.enabled = true;

						//	Gradient grad = new Gradient();
						//	grad.SetKeys(new GradientColorKey[] { new GradientColorKey(UnityEngine.Color.blue, 0.0f), new GradientColorKey(UnityEngine.Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
						//	col.color = grad;

						//	ParticleSystem.ColorBySpeedModule colorBySpeed = drizzle.colorBySpeed;
						//	colorBySpeed.enabled = true;
						//	colorBySpeed.color = grad;

						//	ParticleSystemRenderer renderer = drizzle.GetComponent<ParticleSystemRenderer>();
						//	if (renderer != null)
						//		renderer.material.color = UnityEngine.Color.blue;
						//}
					}
					tbxScratch.Text += $"PlayFromOriginToTarget!" + Environment.NewLine;
					visualEffect.PlayFromOriginToTarget(origin.transform, target.transform);
				}
				else
					EffectManager.PlayRelationEffect(effectName, origin.transform, target.transform);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				//while (ex != null)
				//{
				//	MessageBox.Show(ex.Message, $"{ex.GetType().Name} Exception!");
				//	MessageBox.Show(ex.StackTrace, "StackTrace!");
				//	ex = ex.InnerException;
				//}
			}
		}

		private void chkListenToEvents_CheckedChanged(object sender, EventArgs e)
		{
			if (chkListenToEvents.Checked)
				HookEvents();
			else
				UnhookEvents();
		}

		private void btnGetFlashlight_Click(object sender, EventArgs e)
		{
			//LocalClient.SelectedCreatureId

		}

		System.Timers.Timer flashlightTimer;
		System.Timers.Timer rotationTimer;
		void StartFlashlightTimer()
		{
			if (flashlightTimer == null)
			{
				lblCreateAt.Text = "(tracking)";
				flashlightTimer = new System.Timers.Timer();
				flashlightTimer.Interval = 100;
				flashlightTimer.Elapsed += FlashlightTimer_Elapsed;
				flashlightTimer.Start();
			}
		}
		void StartRotationTimer()
		{
			if (rotationTimer == null)
			{
				lblRotationStatus.Text = "(tracking)";
				rotationTimer = new System.Timers.Timer();
				rotationTimer.Interval = 100;
				rotationTimer.Elapsed += RotationTimer_Elapsed;
				rotationTimer.Start();
			}
		}

		private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
		{
			Vector2 difference = vec2 - vec1;
			float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
			return Vector2.Angle(Vector2.right, difference) * sign;
		}

		private void RotationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				UpdateMiniRotation();
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
			}
		}

		private void FlashlightTimer_Elapsed(object sender, EventArgs e)
		{
			string mouseState;
			if (Input.GetMouseButton(0))
				mouseState = " [x";
			else
				mouseState = " [";
			lblCreateAt.Text = Talespire.Flashlight.GetPositionStr() + mouseState;
		}

		void StopFlashlightTimer()
		{
			lblCreateAt.Text = "(not tracking)";
			if (flashlightTimer == null)
				return;
			flashlightTimer.Stop();
			flashlightTimer = null;
		}

		void StopRotationTimer()
		{
			lblRotationStatus.Text = "(not tracking)";
			if (rotationTimer == null)
				return;
			rotationTimer.Stop();
			rotationTimer = null;
		}

		private void chkTrackFlashlight_CheckedChanged(object sender, EventArgs e)
		{
			if (chkTrackFlashlight.Checked)
				StartFlashlightTimer();
			else
				StopFlashlightTimer();
		}

		private void btnGetRuler_Click(object sender, EventArgs e)
		{
			UnityEngine.Object[] lineRulers = Talespire.Components.GetAll<LineRulerIndicator>();

			if (lineRulers == null)
			{
				tbxScratch.Text += $"No LineRulerIndicators found.\n";
				return;
			}


			tbxScratch.Text += $"{lineRulers.Length} LineRulerIndicators found!\n";

			for (int j = 0; j < 11; j++)
				Talespire.Instances.Delete($"RulerFlames{j}");

			for (int i = 0; i < lineRulers.Length; i++)
			{
				LineRulerIndicator lineRulerIndicator = lineRulers[i] as LineRulerIndicator;
				
				if (lineRulerIndicator != null)
				{
					List<Transform> _handles = ReflectionHelper.GetNonPublicField<List<Transform>>(lineRulerIndicator, "_handles");
					if (_handles != null)
					{
						tbxScratch.Text += $"{_handles.Count} _handles found!\n";
						for (int j = 0; j < _handles.Count; j++)
							AddEffect($"RulerFlames{j}", "MediumFire", new Vector3(_handles[j].position.x, _handles[j].position.y, _handles[j].position.z));
					}
					else
						tbxScratch.Text += $"_handles NOT found!\n";
				}
			}
		}

		private void btnSpectatorMode_Click(object sender, EventArgs e)
		{
			Talespire.Modes.SwitchToSpectator();
		}

		private void btnPlayer_Click(object sender, EventArgs e)
		{
			Talespire.Modes.SwitchToPlayer();
		}

		private void btnFlashlightOff_Click(object sender, EventArgs e)
		{
			Talespire.Target.Off();
			Talespire.Target.SetInteractiveTargetingMode(InteractiveTargetingMode.Sphere);
		}

		private void btnFlashlightOn_Click(object sender, EventArgs e)
		{
			chkTrackFlashlight.Checked = true;
			try
			{
				Talespire.Target.SetInteractiveTargetingMode(InteractiveTargetingMode.Creatures);
				Talespire.Target.On(40);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception calling method!");
			}
		}

		private void btnAddEffects_Click(object sender, EventArgs e)
		{
			VFXMissile magicMissile = Talespire.Effects.GetVisual("MagicMissile") as VFXMissile;
			VFXMissile spellMissile = VFXMissile.Instantiate(magicMissile);
			GameObject particleSysPrefab = Talespire.Prefabs.Get("SampleParticleSys");

			if (particleSysPrefab != null)
			{
				ParticleSystem particleSystem = particleSysPrefab.GetComponentInChildren<ParticleSystem>();
				if (particleSystem != null)
				{
					Talespire.Log.Debug("setting drizzle...");
					ReflectionHelper.SetNonPublicFieldValue(spellMissile, "drizzle", particleSystem);
					Talespire.Log.Debug("setting impact...");
					ReflectionHelper.SetNonPublicFieldValue(spellMissile, "impact", particleSystem);
				}
			}


			//ParticleSystem drizzle = ReflectionHelper.GetNonPublicFieldValue<ParticleSystem>(spellMissile, "drizzle");
			//if (drizzle != null)
			//{
			//	TaleSpireExplorePlugin.LogWarning("Test Log Warning - about to show Drizzle MB...");
			//	MessageBox.Show("drizzle found!");
			//	TaleSpireExplorePlugin.LogError("Test Log Error - this is only a test...");
			//	ParticleSystem.MainModule main = drizzle.main;
			//	
			//	ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient(UnityEngine.Color.blue);
			//	minMaxGradient.mode = ParticleSystemGradientMode.Color;
			//	main.startColor = minMaxGradient;
			//	main.startSize = 5;
			//	ParticleSystem.EmissionModule emission = drizzle.emission;

			//	ParticleSystem.ColorOverLifetimeModule colorOverLifetime = drizzle.colorOverLifetime;

			//	ParticleSystem.MinMaxGradient colorOverLifetimeGradient = new ParticleSystem.MinMaxGradient(UnityEngine.Color.blue, UnityEngine.Color.red);
			//	colorOverLifetime.color = colorOverLifetimeGradient;
			//	colorOverLifetime.enabled = true;
			//	TaleSpireExplorePlugin.LogInfo(ReflectionHelper.GetAllProperties(drizzle.main));

			//	//Shader shader = Shader.Find("Transparent/Diffuse");
			//	//if (shader != null)
			//	//{
			//	//	MessageBox.Show(ReflectionHelper.GetAllProperties(shader), "shader");
			//	//	Material material = new Material(shader);
			//	//	MessageBox.Show(ReflectionHelper.GetAllProperties(material), "material");
			//	//	material.color = UnityEngine.Color.blue;
			//	//	// TODO: use this material in the renderer?
			//	//}


			//	//MessageBox.Show(ReflectionHelper.GetAllProperties(emission), "emission");
			//	emission.rateOverTime = 50;
			//}

			const string spellEffectName = "Spell Missile!";
			spellMissile.name = spellEffectName;
			Talespire.Effects.AddNew(spellEffectName, spellMissile);

			//tbxScratch.Text += Environment.NewLine + "MagicMissile:" + Environment.NewLine;
			//tbxScratch.Text += ReflectionHelper.GetAllProperties(magicMissile);
			//tbxScratch.Text += Environment.NewLine + Environment.NewLine;
			//tbxScratch.Text += Environment.NewLine + "spellMissile: (clone)" + Environment.NewLine;
			//tbxScratch.Text += Environment.NewLine + ReflectionHelper.GetAllProperties(spellMissile);
		}

		private void btnParticleSystemOn_Click(object sender, EventArgs e)
		{
			string spellEffectName = "Fire";
			if (!string.IsNullOrWhiteSpace(lastSelectedPrefab))
				spellEffectName = lastSelectedPrefab;
			Vector3 vector = Talespire.Convert.ToVector3(tbxTargetLocation.Text);
			AddEffect(STR_SpellTestId, spellEffectName, vector);
		}

		private static GameObject AddEffect(string instanceId, string spellEffectName, Vector3? newPosition = null)
		{
			try
			{
				Talespire.Instances.Delete(instanceId);
				GameObject effect = Talespire.Prefabs.Clone(spellEffectName, instanceId);

				Talespire.Property.Modify(effect, "<FX_LifeTime>.LifeTime", 0, false);

				Talespire.Log.Debug($"Back from Modify");
				if (effect == null)
				{
					Talespire.Log.Debug($"Talespire.Prefabs.Clone(\"{spellEffectName}\") returned null.");
					return null;
				}

				Talespire.Log.Debug($"prefab: {effect.name}");
				if (newPosition.HasValue)
					effect.transform.position = newPosition.Value;
				return effect;
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				return null;
			}
		}

		private void btnParticleSystemOff_Click(object sender, EventArgs e)
		{
			Talespire.Instances.Delete(STR_SpellTestId);
			//Talespire.Property.Modify(effect, "<FX_LifeTime>.DieNow", true);
		}

		string lastSelectedPrefab;
		private void cmbPrefabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			lastSelectedPrefab = cmbPrefabs.SelectedItem as string;
			if (string.IsNullOrWhiteSpace(lastSelectedPrefab))
				return;
			Vector3 vector = Talespire.Convert.ToVector3(tbxTargetLocation.Text);
			AddEffect(STR_SpellTestId, lastSelectedPrefab, vector);
		}

		private void cmbPrefabs_DropDown(object sender, EventArgs e)
		{
			if (cmbPrefabs.Items.Count == 0)
				foreach (string item in Talespire.Prefabs.AllNames.OrderBy(x => x).ToList())
					cmbPrefabs.Items.Add(item);
		}

		// TODO: Replace these IDs with the equivalent world IDs:
		public const string JanusId = "aba6b475-a026-48dd-9722-c8d7049e2566";
		public const string MerkinId = "35400cec-9539-424f-b185-00569d4850c4"; // "b9529862-8d73-4662-ab13-cf9232b1ccf9";

		/* 
		 Merkin: (11.5, 22.5, 95.9), UniqueId: {creatureAsset.Creature.UniqueId}, WorldId: 35400cec-9539-424f-b185-00569d4850c4, BoardAssetId: b9529862-8d73-4662-ab13-cf9232b1ccf9
L'il Cutie: (11.3, 22.5, 98.4), UniqueId: {creatureAsset.Creature.UniqueId}, WorldId: 4c795076-7bdc-489c-afff-4112d12f1138, BoardAssetId: 537d21df-3cd7-4873-bd75-3913621e484c
		 */
		public const string CutieId = "4c795076-7bdc-489c-afff-4112d12f1138"; // "537d21df-3cd7-4873-bd75-3913621e484c";

		const string MediumFireEffectId = "MediumFireEffectId";

		private void btnAttackJanus_Click(object sender, EventArgs e)
		{

			try
			{
				CreatureBoardAsset[] allCreatureAssets = Talespire.Minis.GetAll();

				string effectName = "MediumFire";
				if (lastSelectedPrefab != null)
					effectName = lastSelectedPrefab;


				GameObject fireBall = AddEffect(MediumFireEffectId, effectName);
				Translator translator = fireBall.AddComponent<Translator>();

				CharacterPosition janusPosition = Talespire.Minis.GetPosition(JanusId);
				CharacterPosition merkinPosition = Talespire.Minis.GetPosition(MerkinId);

				if (janusPosition == null)
				{
					Talespire.Log.Error("janusPosition is null!");
					return;
				}

				if (merkinPosition == null)
				{
					Talespire.Log.Error("merkinPosition is null!");
					return;
				}

				fireBall.transform.position = merkinPosition.Position.GetVector3();
				translator.StartPosition = merkinPosition.Position.GetVector3();
				translator.StopPosition = janusPosition.Position.GetVector3();
				translator.TravelTime = 0.8f;
				translator.easing = EasingOption.EaseInQuint;
				translator.StartTravel();
				//AddEffect(MerkinId, "MediumFire", merkinPosition.Position.GetVector3());
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
			}
		}

		void ApplyEffects(GameObject effect)
		{
			string effectNameToMatch = effect.name;
			if (effectNameToMatch.Contains("("))
			{
				effectNameToMatch = effectNameToMatch.Substring(0, effectNameToMatch.IndexOf("("));
			}

			Talespire.Log.Debug($"effectNameToMatch = {effectNameToMatch}");
		}

		Dictionary<string, CompositeEffect> registeredEffects = new Dictionary<string, CompositeEffect>();

		void RegisterEffects()
		{
			CompositeEffect result = CreateCustomFireEffect();

			string serializedObject = JsonConvert.SerializeObject(result);
			Talespire.Log.Debug($"JSON'd effect: \"{serializedObject}\"");
			registeredEffects.Add("TestFireEffect", result);
		}

		private static CompositeEffect CreateCustomFireEffect()
		{
			CompositeEffect result = new CompositeEffect();
			result.PrefabToCreate = "MediumFire";
			result.AddProperty(new ChangeVector3("<Transform>.localScale", "3, 3, 3"));
			result.AddProperty(new ChangeMinMaxGradient("<ParticleSystem>.main.startColor", "#0026ff -> #00ffdd"));
			return result;
		}

		private void btnClearAttack_Click(object sender, EventArgs e)
		{
			Talespire.Instances.Delete(JanusId);
			Talespire.Instances.Delete(MediumFireEffectId);
			Talespire.Instances.Delete(MerkinId);
		}

		CompositeEffect GetCompositeEffect(string key)
		{
			if (registeredEffects.ContainsKey(key))
				return registeredEffects[key];
			return null;
		}

		const string testInstanceId = "TestEffectsClick";

		private void btnTestEffects_Click(object sender, EventArgs e)
		{
			Talespire.GameObjects.InvalidateFound();
			CompositeEffect compositeEffect = GetCompositeEffect("TestFireEffect");
			if (compositeEffect == null)
			{
				Talespire.Log.Error("TestFireEffect not found!");
				return;
			}

			Talespire.Log.Warning("TestFireEffect found!");

			try
			{
				Talespire.Instances.Delete(testInstanceId);

				CharacterPosition janusPosition = Talespire.Minis.GetPosition(JanusId);
				CharacterPosition merkinPosition = Talespire.Minis.GetPosition(MerkinId);
				compositeEffect.CreateOrFindSafe(testInstanceId, merkinPosition, janusPosition);
				tbxScratch.Text = JsonConvert.SerializeObject(compositeEffect);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		private void btnGetActiveGameObjects_Click(object sender, EventArgs e)
		{
			Talespire.Material.Refresh();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("All Materials: ");
			foreach (string name in Talespire.Material.GetAllNames())
				stringBuilder.AppendLine($"  {name}");

			tbxScratch.Text = stringBuilder.ToString();
		}

		private void Deserialize(string text)
		{
			try
			{
				Talespire.GameObjects.InvalidateFound();
				Talespire.Instances.Delete(testInstanceId);
				CompositeEffect compositeEffect = JsonConvert.DeserializeObject<CompositeEffect>(text);
				compositeEffect.RebuildPropertiesAfterLoad();
				CharacterPosition janusPosition = Talespire.Minis.GetPosition(JanusId);
				CharacterPosition merkinPosition = Talespire.Minis.GetPosition(MerkinId);
				compositeEffect.CreateOrFindSafe(testInstanceId, merkinPosition, janusPosition);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		FrmEffectEditor frmEffectEditor;
		private void btnEditPrefabs_Click(object sender, EventArgs e)
		{
			try
			{
				if (frmEffectEditor == null)
				{
					frmEffectEditor = new FrmEffectEditor();
					frmEffectEditor.FormClosed += FrmEffectEditor_FormClosed;
				}
				frmEffectEditor.Show(this);
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex);
				MessageBox.Show(ex.Message, "Exception");
			}
		}

		private void FrmEffectEditor_FormClosed(object sender, FormClosedEventArgs e)
		{
			frmEffectEditor = null;
		}

		bool lastRed;

		private void btnTestSetIndicatorColor_Click(object sender, EventArgs e)
		{
			if ((ModifierKeys & Keys.Shift) == Keys.Shift)
				Talespire.Minis.IndicatorTurnOff(MerkinId);
			else if (lastRed)
				Talespire.Minis.IndicatorChangeColor(MerkinId, UnityEngine.Color.blue, 2);
			else
				Talespire.Minis.IndicatorChangeColor(MerkinId, UnityEngine.Color.red);

			lastRed = !lastRed;
		}

		private void btnSetScale_Click(object sender, EventArgs e)
		{
			if (float.TryParse(txtScale.Text, out float result))
				Talespire.Minis.SetCreatureScale(MerkinId, result);
		}

		void GenerateAnimationCurveCode(string name, AnimationCurve curve)
		{
			if (curve == null)
			{
				Talespire.Log.Error($"{name} curve is null!");
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"{name} = new AnimationCurve();");
			stringBuilder.AppendLine($"{name}.preWrapMode = WrapMode.{curve.preWrapMode};");
			stringBuilder.AppendLine($"{name}.postWrapMode = WrapMode.{curve.postWrapMode};");
			foreach (Keyframe key in curve.keys)
				stringBuilder.AppendLine($"{name}.AddKey(new Keyframe({key.time}f, {key.value}f, {key.inTangent}f, {key.outTangent}f, {key.inWeight}f, {key.outWeight}f));");

			stringBuilder.AppendLine();
			tbxScratch.Text += stringBuilder.ToString();
		}

		private void btnTest1_Click(object sender, EventArgs e)
		{

		}

		private void btnTest2_Click(object sender, EventArgs e)
		{
			//Talespire.Minis.TryCreateCrocodile(new Vector3(15f, 22.5f, 95.4f));
			//Talespire.Board.InstantiateCreature(tbxAssetId.Text, new Vector3(15f, 22.5f, 95.4f));
		}

		private void btnSetCameraPosition_Click(object sender, EventArgs e)
		{
			Talespire.Camera.SetPosition(tbxCameraPosition.Text);
		}

		private void btnSetCameraHeight_Click(object sender, EventArgs e)
		{
			Talespire.Camera.SetCameraHeight(tbxCameraHeight.Text);
		}

		private void chkTrackAngle_CheckedChanged(object sender, EventArgs e)
		{
			if (chkTrackAngle.Checked)
				StartRotationTimer();
			else
				StopRotationTimer();
		}

		private void btnAngle_Click(object sender, EventArgs e)
		{
			UpdateMiniRotation();
		}

		private void UpdateMiniRotation()
		{
			CreatureBoardAsset merkinCreatureBoardAsset = Talespire.Minis.GetCreatureBoardAsset(MerkinId);
			if (merkinCreatureBoardAsset == null)
			{
				Talespire.Log.Error($"merkinCreatureBoardAsset is null!");
				return;
			}

			Talespire.Log.ChangeOnly("merkinCreatureBoardAsset.Creature.transform.forward", $"{merkinCreatureBoardAsset.Creature.transform.forward}");

			CreatureBoardAsset cutieCreatureBoardAsset = Talespire.Minis.GetCreatureBoardAsset(CutieId);
			
			Vector3 cutiePosition = cutieCreatureBoardAsset.transform.position;
			
			float angleBetweenVectors = AngleBetweenVector2(merkinCreatureBoardAsset.Creature.transform.forward, cutiePosition - merkinCreatureBoardAsset.transform.position);

			float rotationDegrees = merkinCreatureBoardAsset.GetRotationDegrees();

			Talespire.Log.ChangeOnly("Merkin's rotationDegrees", $"{rotationDegrees}");

			Talespire.Log.ChangeOnly("angleBetweenVectors", $"{angleBetweenVectors}");

			lblRotationStatus.Text = $"{angleBetweenVectors:F}";
		}

		private void btnSet1_Click(object sender, EventArgs e)
		{
			CreatureBoardAsset selected = Talespire.Minis.GetSelected();

			CreatureBoardAsset cutie = Talespire.Minis.GetCreatureBoardAsset(CutieId);
			if (selected != null)
				cutie.RotateTowards(selected.transform.position);
			else
				Talespire.Log.Error($"No selected creature!");
		}

		private void btnSet2_Click(object sender, EventArgs e)
		{
			
		}

		private void btnSet3_Click(object sender, EventArgs e)
		{
			
		}

		private void btnSkyScraper_Click(object sender, EventArgs e)
		{
			tbxTargetLocation.Text = "15, 22.5, 95.4";
		}

		private void btnBattleZone_Click(object sender, EventArgs e)
		{
			tbxTargetLocation.Text = "-1,0.2,0";
		}

		private void btnGhostToggle_Click(object sender, EventArgs e)
		{
			Talespire.Minis.SetVisibility(tbxGhostId.Text);
		}

		private void btnCamera2_Click(object sender, EventArgs e)
		{
			Talespire.Camera.SetPosition("0, 12, 0");
		}

		private void btnCamera1_Click(object sender, EventArgs e)
		{
			Talespire.Camera.SetPosition("15, 25, 95.4");
		}

		private void btnReloadSpellEffects_Click(object sender, EventArgs e)
		{
			TaleSpireExplorePlugin.LoadKnownEffects();
		}
	}
}
