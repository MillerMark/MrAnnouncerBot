using Newtonsoft.Json;
using UnityEngine;
using TaleSpireCore;
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

namespace TaleSpireExplore
{
	public partial class FrmExplorer : Form
	{
		public FrmExplorer()
		{
			InitializeComponent();
		}

		void LogEvent(string message)
		{
			try
			{
				tbxLog.Text += message + Environment.NewLine;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
			}
		}

		void LogCreatureAsset(CreatureBoardAsset creatureAsset)
		{
			tbxLog.Text += "--------------" + Environment.NewLine;
			tbxLog.Text += $"{TaleSpireUtils.GetName(creatureAsset)}: \n";
			tbxLog.Text += $"  BoardAssetId: {creatureAsset.BoardAssetId}\n";
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
			LogEvent($"CampaignSessionManager.OnStatNamesChange(obj: {obj})");
		}

		private void CampaignSessionManager_OnStateWillTransition(PhotonSimpleSingletonStateMBehaviour<CampaignSessionManager>.State arg1, PhotonSimpleSingletonStateMBehaviour<CampaignSessionManager>.State arg2)
		{
			LogEvent($"CampaignSessionManager.OnStateWillTransition(arg1: {arg1}, arg2: {arg2})");
		}

		private void CampaignSessionManager_OnStateChange(PhotonSimpleSingletonStateMBehaviour<CampaignSessionManager>.State obj)
		{
			LogEvent($"CampaignSessionManager.OnStateChange(obj: {obj})");
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
			LogEvent($"BoardSessionManager.OnStateWillTransition(arg1: {arg1}, arg2: {arg2})");
		}

		private void BoardSessionManager_OnStateChange(PhotonSimpleSingletonStateMBehaviour<BoardSessionManager>.State obj)
		{
			LogEvent($"BoardSessionManager.OnStateChange(obj: {obj})");
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
				IReadOnlyList<CreatureBoardAsset> allCreatureAssets = CreaturePresenter.AllCreatureAssets;
				tbxScratch.Text = "";
				if (allCreatureAssets == null)
					return;
				foreach (CreatureBoardAsset creatureAsset in allCreatureAssets)
				{
					tbxScratch.Text += $"{TaleSpireUtils.GetName(creatureAsset)}: {creatureAsset.PlacedPosition}, ID: {creatureAsset.BoardAssetId}, Scale: {creatureAsset.CreatureScale}\n";
				}
			}
			catch (Exception ex)
			{
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


		public EffectManager EffectManagerInstance => SingletonBehaviour<EffectManager>.Instance;

		private void btnEffects_Click(object sender, EventArgs e)
		{
			try
			{
				EffectManager effectManager = EffectManagerInstance;
				FieldInfo field = typeof(EffectManager).GetField("_effects", BindingFlags.NonPublic | BindingFlags.Instance);
				object value = field.GetValue(effectManager);
				if (value is List<EffectManager.Effect> effects)
				{
					tbxScratch.Text += $"Known Effects ({effects.Count}):" + Environment.NewLine;

					foreach (EffectManager.Effect effect in effects)
						tbxScratch.Text += "  " + effect._effectName + Environment.NewLine;
				}
				else
				{
					tbxScratch.Text += $"value is not a List<EffectManager.Effect>! It is a {value.GetType()}" + Environment.NewLine;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception!");
				while (ex.InnerException != null)
				{
					ex = ex.InnerException;
					MessageBox.Show(ex.Message, "Inner Exception!");
				}
			}
		}

		EffectManager.Effect GetEffect(string effectName)
		{
			effectName = effectName.Trim();
			EffectManager effectManager = EffectManagerInstance;
			FieldInfo field = typeof(EffectManager).GetField("_effects", BindingFlags.NonPublic | BindingFlags.Instance);
			object value = field.GetValue(effectManager);
			if (value is List<EffectManager.Effect> effects)
				foreach (EffectManager.Effect effect in effects)
					if (effect._effectName == effectName)
						return effect;
			return null;
		}

		VisualEffect GetVisualEffect(string effectName)
		{
			EffectManager.Effect effect = GetEffect(effectName);
			if (effect == null)
				return null;
			return effect.GetEffect();
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
				while (ex != null)
				{
					MessageBox.Show(ex.Message, "Exception!");
					MessageBox.Show(ex.StackTrace, "StackTrace!");
					ex = ex.InnerException;
				}
			}

		}

		T GetNonPublicField<T>(Type type, object instance, string fieldName) where T: class
		{
			FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (field == null)
				return default(T);
			return field.GetValue(instance) as T;
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
				VisualEffect visualEffect = GetVisualEffect(effectName);
				if (visualEffect != null)
				{
					if (visualEffect is VFXMissile missile)
					{
						ParticleSystem drizzle = GetNonPublicField<ParticleSystem>(typeof(VFXMissile), missile, "drizzle");
						if (drizzle == null)
							tbxScratch.Text += $"drizzle was NOT found!" + Environment.NewLine;
						else
						{
							tbxScratch.Text += $"drizzle was found!" + Environment.NewLine;
							AddScratchLine($"  proceduralSimulationSupported: {drizzle.proceduralSimulationSupported}");
							AddScratchLine($"  collision: {drizzle.collision}");
							AddScratchLine($"  colorBySpeed: {drizzle.colorBySpeed}");
							AddScratchLine($"  colorOverLifetime: {drizzle.colorOverLifetime}");
							AddScratchLine($"  customData: {drizzle.customData}");
							AddScratchLine($"  emission.burstCount: {drizzle.emission.burstCount}");
							AddScratchLine($"  emission.emission.enabled: {drizzle.emission.enabled}");
							AddScratchLine($"  emission.emission.rateOverDistance: {drizzle.emission.rateOverDistance}");
							AddScratchLine($"  emission.emission.rateOverDistanceMultiplier: {drizzle.emission.rateOverDistanceMultiplier}");
							AddScratchLine($"  emission.emission.rateOverTime: {drizzle.emission.rateOverTime}");
							AddScratchLine($"  emission.emission.rateOverTimeMultiplier: {drizzle.emission.rateOverTimeMultiplier}");
							AddScratchLine($"  externalForces: {drizzle.externalForces}");
							AddScratchLine($"  forceOverLifetime: {drizzle.forceOverLifetime}");
							AddScratchLine($"  inheritVelocity: {drizzle.inheritVelocity}");
							AddScratchLine($"  isEmitting: {drizzle.isEmitting}");
							AddScratchLine($"  isPaused: {drizzle.isPaused}");
							AddScratchLine($"  isStopped: {drizzle.isStopped}");
							AddScratchLine($"  lifetimeByEmitterSpeed: {drizzle.lifetimeByEmitterSpeed}");
							AddScratchLine($"  lights: {drizzle.lights}");
							AddScratchLine($"  limitVelocityOverLifetime: {drizzle.limitVelocityOverLifetime}");

							AddScratchLine($"  main.duration: {drizzle.main.duration}");
							AddScratchLine($"  main.gravityModifier: {drizzle.main.gravityModifier}");
							AddScratchLine($"  main.loop: {drizzle.main.loop}");




							ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient(UnityEngine.Color.blue);
							minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;
							ParticleSystem.MainModule main = drizzle.main;
							main.startColor = minMaxGradient;
							main.startSize = 2;
							ParticleSystem.EmissionModule emission = drizzle.emission;
							emission.rateOverTime = 50;



							// None of these tries work to change the color:
							ParticleSystem.ColorOverLifetimeModule col = drizzle.colorOverLifetime;
							col.enabled = true;

							Gradient grad = new Gradient();
							grad.SetKeys(new GradientColorKey[] { new GradientColorKey(UnityEngine.Color.blue, 0.0f), new GradientColorKey(UnityEngine.Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
							col.color = grad;

							ParticleSystem.ColorBySpeedModule colorBySpeed = drizzle.colorBySpeed;
							colorBySpeed.enabled = true;
							colorBySpeed.color = grad;

							ParticleSystemRenderer renderer = drizzle.GetComponent<ParticleSystemRenderer>();
							if (renderer != null)
								renderer.material.color = UnityEngine.Color.blue;
						}
					}
					tbxScratch.Text += $"PlayFromOriginToTarget!" + Environment.NewLine;
					visualEffect.PlayFromOriginToTarget(origin.transform, target.transform);
				}
				else
					EffectManager.PlayRelationEffect(effectName, origin.transform, target.transform);
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					MessageBox.Show(ex.Message, $"{ex.GetType().Name} Exception!");
					MessageBox.Show(ex.StackTrace, "StackTrace!");
					ex = ex.InnerException;
				}
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
		void StartFlashlightTimer()
		{
			if (flashlightTimer == null)
			{
				lblFlashlightStatus.Text = "(tracking)";
				flashlightTimer = new System.Timers.Timer();
				flashlightTimer.Interval = 100;
				flashlightTimer.Elapsed += FlashlightTimer_Tick;
				flashlightTimer.Start();
			}
		}

		private void FlashlightTimer_Tick(object sender, EventArgs e)
		{
			lblFlashlightStatus.Text = TaleSpireHelper.GetFlashlightPositionStr();
		}

		void StopFlashlightTimer()
		{
			lblFlashlightStatus.Text = "(not tracking)";
			if (flashlightTimer == null)
				return;
			flashlightTimer.Stop();
			flashlightTimer = null;
		}

		private void chkTrackFlashlight_CheckedChanged(object sender, EventArgs e)
		{
			if (chkTrackFlashlight.Checked)
				StartFlashlightTimer();
			else
				StopFlashlightTimer();
		}

		private void btnBoom_Click(object sender, EventArgs e)
		{
			float x = 15f;
			float y = 22.5f;
			float z = 95.4f;

			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = new Vector3(x, y, z);
			sphere.transform.localScale = new Vector3(10, 10, 10);
		}

		private void btnSpectatorMode_Click(object sender, EventArgs e)
		{
			TaleSpireHelper.SwitchToSpectatorMode();
		}

		private void btnPlayer_Click(object sender, EventArgs e)
		{
			TaleSpireHelper.SwitchToPlayerMode();
		}

		private void btnFlashlightOff_Click(object sender, EventArgs e)
		{
			TaleSpireHelper.TurnFlashlightOff();
		}

		private void btnFlashlightOn_Click(object sender, EventArgs e)
		{
			TaleSpireHelper.TurnFlashlightOn();
			try
			{
				FlashLight flashlight = TaleSpireHelper.GetFlashlight();

				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				// Unity units - 1 = 1 tile = 5 feet
				sphere.transform.localScale = new Vector3(4, 4, 4);
				Renderer renderer = sphere.GetComponent<Renderer>();

				// TODO: Figure transparency.
				// http://gyanendushekhar.com/2021/02/08/using-transparent-material-in-unity-3d/
				Material material = renderer.material;

				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt("_ZWrite", 0);
				material.DisableKeyword("_ALPHATEST_ON");
				material.DisableKeyword("_ALPHABLEND_ON");
				material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				material.color = new UnityEngine.Color(0, 0, 1, 0.2f);

				sphere.transform.parent = flashlight.gameObject.transform;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception calling method!");
			}
		}
	}
}
