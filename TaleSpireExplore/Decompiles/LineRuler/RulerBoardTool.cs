using System;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	public class RulerBoardTool : BoardTool
	{
		// Fields
		private const float DOUBLE_CLICK_TIME = 0.2f;
		private SimpleStateMachine<BaseState, State> _states = new SimpleStateMachine<BaseState, State>();
		private Ruler _ruler;
		private int _lastIndicatorIndex;
		[SerializeField]
		private Texture[] _rulerIcons;

		// Events
		public static event Action OnCloseRulers;

		public static event Action<int> OnRulerIndicatorChange;

		// Methods
		protected void Awake()
		{
			_states.AddState(State.Idle, new IdleState(this));
			_states.AddState(State.SettingHandles, new SettingsHandles(this));
			_states.AddState(State.AdjustingHandles, new AdjustingHandles(this));
		}

		public override void Back()
		{
			Cleanup();
			SwitchToOwnDefaultTool();
		}

		public override void Begin()
		{
			CampaignSessionManager.OnDistanceUnitsChanged -= new Action<DistanceUnit[]>(OnDistanceUnitsChanged);
			CampaignSessionManager.OnDistanceUnitsChanged += new Action<DistanceUnit[]>(OnDistanceUnitsChanged);
			UI_HelpBar.DisplayHelpCollection(HelpBarCollection.RulerHelp, 2);
			Cleanup();
		}

		public override void CallUpdate()
		{
			base.CallUpdate();
			_states.Update();
			if (_ruler != null)
			{
				_lastIndicatorIndex = _ruler.CurrentIndicatorIndex;
			}
		}

		public override bool CheckForInGameGUI() =>
				((State)_states.CurrentState) == State.AdjustingHandles;

		private void Cleanup()
		{
			EnsureNoRuler();
			_states.SwitchState(State.Idle);
		}

		public override void End()
		{
			if (OnCloseRulers == null)
			{
				Action onCloseRulers = OnCloseRulers;
			}
			else
			{
				OnCloseRulers();
			}
			Cleanup();
			base.End();
			UI_HelpBar.ClearPrioritySlot(2);
		}

		private void EnsureNoRuler()
		{
			if (_ruler != null)
			{
				_ruler.Dispose();
				_ruler = null;
			}
		}

		private void HideUI()
		{
		}

		public override void OnCameraClick(CameraController.CameraClickEvent click)
		{
			base.OnCameraClick(click);
			if (((click.position.x > 0f) && (click.position.y > 0f)) && ((click.position.x <= Screen.currentResolution.width) && (click.position.y <= Screen.currentResolution.height)))
			{
				_states.GetState().OnCameraClick(click);
			}
		}

		private void OnDistanceUnitsChanged(DistanceUnit[] _)
		{
			if (_ruler == null)
			{
				Ruler local1 = _ruler;
			}
			else
			{
				_ruler.OnDistanceUnitsChanged();
			}
		}

		public void ResetState()
		{
			Cleanup();
			_states.SwitchState(State.SettingHandles);
		}

		public void SetRulerMode(int index)
		{
			if (_ruler == null)
			{
				Ruler local1 = _ruler;
			}
			else
			{
				_ruler.SetMode(index);
			}
			if (OnRulerIndicatorChange == null)
			{
				Action<int> onRulerIndicatorChange = OnRulerIndicatorChange;
			}
			else
			{
				OnRulerIndicatorChange(index);
			}
		}

		public override bool ShouldRightClickGoBack() => _ruler == null;

		private void ShowUI()
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
		}

		// Properties
		public static int CurrentIndicatorIndex
		{
			get
			{
				RulerBoardTool tool = SingletonBehaviour<BoardToolManager>.Instance.GetTool<RulerBoardTool>();
				return ((tool == null) ? 0 : tool._lastIndicatorIndex);
			}
		}

		public static bool IsActive =>
				BoardToolManager.CurrentTool is RulerBoardTool;

		// Nested Types
		private class AdjustingHandles : RulerBoardTool.BaseState
		{
			// Fields
			private int _currentHoveredGizmo;
			private int _activeGizmo;

			// Methods
			public AdjustingHandles(RulerBoardTool owner) : base(RulerBoardTool.State.AdjustingHandles, owner)
			{
				_currentHoveredGizmo = -1;
				_activeGizmo = -1;
			}

			public override void Begin()
			{
				_currentHoveredGizmo = -1;
				_activeGizmo = -1;
			}

			public override void OnCameraClick(CameraController.CameraClickEvent click)
			{
				if (click.interactionType == CameraController.ClickEventInteraction.TAP)
				{
					if (click.buttonID == 1)
					{
						base._owner.Back();
					}
				}
				else if (click.interactionType == CameraController.ClickEventInteraction.WHILE_DOWN)
				{
					if ((click.buttonID == 0) && ((_activeGizmo == -1) && ((click.buttonID == 0) && (_currentHoveredGizmo != -1))))
					{
						_activeGizmo = _currentHoveredGizmo;
						_currentHoveredGizmo = -1;
						base._owner.ShowUI();
					}
				}
				else if ((_activeGizmo != -1) && (click.buttonID == 0))
				{
					if (base.Ruler.GimzoAllowsReturnToEditting(_activeGizmo))
					{
						base.Ruler.NextHandle();
						base._owner._states.SwitchState(RulerBoardTool.State.SettingHandles);
					}
					else
					{
						base._owner.HideUI();
						_activeGizmo = -1;
					}
				}
			}

			public override void Update()
			{
				if (_activeGizmo == -1)
				{
					_currentHoveredGizmo = base.Ruler.HandleGizmoHover(CameraController.GetCameraRay());
				}
				else
				{
					base.Ruler.UpdateGizmo(_activeGizmo);
				}
			}
		}

		private abstract class BaseState : SimpleState<RulerBoardTool>
		{
			// Methods
			public BaseState(RulerBoardTool.State stateEnum, RulerBoardTool owner) : base(stateEnum.ToString(), owner)
			{
			}

			public virtual void OnCameraClick(CameraController.CameraClickEvent click)
			{
			}

			// Properties
			protected Ruler Ruler
			{
				get =>
						base._owner._ruler;
				set =>
						base._owner._ruler = value;
			}
		}

		public enum ClickResult
		{
			Cancel,
			Ignore,
			Finish
		}

		private class IdleState : RulerBoardTool.BaseState
		{
			// Methods
			public IdleState(RulerBoardTool owner) : base(RulerBoardTool.State.Idle, owner)
			{
			}

			public override void Begin()
			{
				base._owner.EnsureNoRuler();
			}
		}

		private class SettingsHandles : RulerBoardTool.BaseState
		{
			// Methods
			public SettingsHandles(RulerBoardTool owner) : base(RulerBoardTool.State.SettingHandles, owner)
			{
			}

			public override void Begin()
			{
				if (base.Ruler == null)
				{
					base.Ruler = Ruler.Spawn(RulerBoardTool.CurrentIndicatorIndex, true);
				}
				base._owner.ShowUI();
			}

			public override void End()
			{
				base._owner.HideUI();
			}

			public override void OnCameraClick(CameraController.CameraClickEvent click)
			{
				if (click.interactionType == CameraController.ClickEventInteraction.TAP)
				{
					switch (base.Ruler.OnClick(click.buttonID))
					{
						case RulerBoardTool.ClickResult.Cancel:
							base._owner.Back();
							return;

						case RulerBoardTool.ClickResult.Ignore:
							break;

						case RulerBoardTool.ClickResult.Finish:
							base._owner._states.SwitchState(RulerBoardTool.State.AdjustingHandles);
							break;

						default:
							return;
					}
				}
			}

			public override void Update()
			{
				if (base.Ruler != null)
				{
					base.Ruler.UpdateActiveHandle();
				}
			}
		}

		public enum State
		{
			Idle,
			SettingHandles,
			AdjustingHandles
		}
	}
}