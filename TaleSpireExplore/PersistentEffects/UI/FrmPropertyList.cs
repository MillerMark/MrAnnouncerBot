using System;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using TaleSpireExplore.Unmanaged;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleSpireCore;
using System.Diagnostics;
using LordAshes;

namespace TaleSpireExplore
{
	public partial class FrmPropertyList : Form, IValueChangedListener
	{
		System.Timers.Timer windowDragTimer;
		static FrmPersistentEffectPropertyEditor frmPersistentEffectPropertyEditor;
		List<EffectProperty> EffectProperties = new List<EffectProperty>();

		public FrmPropertyList()
		{
			InitializeComponent();
			windowDragTimer = new System.Timers.Timer();
			windowDragTimer.Interval = 20;
			windowDragTimer.Elapsed += WindowDragTimer_Elapsed;
			
			ValueEditors.Register(STR_PersistentEffectsEditorKey);
			foreach (IValueEditor valueEditor in ValueEditors.GetAll(STR_PersistentEffectsEditorKey))
			{
				Talespire.Log.Debug($"valueEditor ({valueEditor.GetType().Name}).Initialize(this);");
				valueEditor.Initialize(this);
			}

			if (frmPersistentEffectPropertyEditor == null)
			{
				Talespire.Log.Debug($"frmPersistentEffectPropertyEditor = new FrmPersistentEffectPropertyEditor();");
				frmPersistentEffectPropertyEditor = new FrmPersistentEffectPropertyEditor();
			}

			Talespire.Log.Debug($"ValueEditors.ValueChanged += ValueEditors_ValueChanged;");
			ValueEditors.ValueChanged += ValueEditors_ValueChanged;
		}

		private void ValueEditors_ValueChanged(object sender, ValueChangedEventArgs ea)
		{
			if (!(sender is AllValueEditors allValueEditors))
			{
				Talespire.Log.Error($"sender is NOT AllValueEditors!!!");
				return;
			}

			if (allValueEditors.Key != STR_PersistentEffectsEditorKey)
			{
				Talespire.Log.Error($"allValueEditors.Key != STR_PersistentEffectsEditorKey");
				return;
			}

			Talespire.Log.Debug($"ValueEditors_ValueChanged...");
			UpdateInstance(ea.Value, ea.CommittedChange);
		}

		private void UpdateInstance(object value, bool committedChange)
		{
			EffectProperty effectProperty = lstProperties.SelectedItem as EffectProperty;
			if (effectProperty != null)
			{
				// TODO: Store this change in the Mini.
				if (frmPersistentEffectPropertyEditor != null)
				{
					Control childAtPoint = frmPersistentEffectPropertyEditor.GetChildAtPoint(new Point(4, 4));
					if (childAtPoint is IValueEditor valueEditor)
					{
						BasePropertyChanger propertyChanger = valueEditor.GetPropertyChanger();
						if (propertyChanger != null)
						{
							propertyChanger.Name = effectProperty.Path;
							propertyChanger.ValueOverride = value;
							propertyChanger.ModifyProperty(Instance, true);

							if (committedChange)
							{
								IOldPersistentEffect persistentEffect = Mini.GetPersistentEffect();
								if (persistentEffect != null)
								{
									Talespire.Log.Warning($"Properties[{effectProperty.Path}] = {value}!!!");

									// TODO: Use the correct EffectProperties instead of Properties (based on the prefix of the selected control).
									// TODO: Change this to be indexed by the property NAME, not the path (to support multiple linked properties (to a single SmartProperty).
									persistentEffect.Properties[effectProperty.Path] = value.ToString();
									Talespire.Log.Debug($"Mini.SavePersistentEffect();");
									Mini.SavePersistentEffect(persistentEffect);
								}
								else
									Talespire.Log.Error($"persistentEffect is not found!!");
							}
							else
								Talespire.Log.Warning($"Not a committed change! Not saving my friend!");
						}
						else
							Talespire.Log.Error($"propertyChanger not found!!!");
					}
					else
						Talespire.Log.Error($"childAtPoint is not a value editor!");
				}
				else
					Talespire.Log.Error($"frmPersistentEffectPropertyEditor is null!");
			}
			else
				Talespire.Log.Error($"effectProperty is NULL!!!");
		}

		public void AddProperty(string name, Type type, string path)
		{
			EffectProperties.Add(new EffectProperty(name, type, path));
		}

		public void ClearProperties()
		{
			EffectProperties.Clear();
		}

		void SetHeightAndWidth()
		{
			const int topBottomMargin = 12;
			const int leftRightMargin = 12;
			int height = lstProperties.Items.Count * lstProperties.ItemHeight + topBottomMargin;
			int maxWidthSoFar = 0;
			foreach (EffectProperty effectProperty in EffectProperties)
			{
				Size measureText = TextRenderer.MeasureText(effectProperty.Name, lstProperties.Font);
				if (measureText.Width > maxWidthSoFar)
					maxWidthSoFar = measureText.Width;
			}
			Height = height;
			lstProperties.Height = height;
			Width = maxWidthSoFar + leftRightMargin;
		}

		public void ShowPropertyList()
		{
			foreach (EffectProperty effectProperty in EffectProperties)
				lstProperties.Items.Add(effectProperty);

			lstProperties.SelectedIndex = 0;

			SetHeightAndWidth();
			SetLocation(WindowHelper.GetTaleSpireTopRight());
			windowDragTimer.Start();
		}

		const int INT_PropertyEditorWidth = 330;
		const int INT_PropertyEditorMargin = 8;
		const string STR_PersistentEffectsEditorKey = "PersistentEffectsEditor";
		Point lastLocation;

		private void SetLocation(Point topRightOfTaleSpire)
		{
			if (lastLocation == topRightOfTaleSpire)
				return;
			Location = new Point(topRightOfTaleSpire.X - (INT_PropertyEditorWidth + INT_PropertyEditorMargin) - Width, topRightOfTaleSpire.Y + WindowHelper.TaleSpireTitleBarHeight);
			frmPersistentEffectPropertyEditor.SetLocation(topRightOfTaleSpire);
			lastLocation = topRightOfTaleSpire;
		}

		private void lstProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				EffectProperty effectProperty = lstProperties.SelectedItem as EffectProperty;
				if (effectProperty != null)
				{
					Talespire.Log.Warning($"{effectProperty.Name} selected!");
					WindowHelper.FocusTaleSpire();
				}

				frmPersistentEffectPropertyEditor.Controls.Clear();

				UserControl valueEditor = ValueEditors.Get(STR_PersistentEffectsEditorKey, effectProperty.Type) as UserControl;
				if (valueEditor != null)
				{
					// TODO: Set the control's state based on the actual value.
					frmPersistentEffectPropertyEditor.Controls.Add(valueEditor);
					frmPersistentEffectPropertyEditor.Height = valueEditor.Height + 8;

					Talespire.Log.Debug($"Instance: {Instance}");
					if (valueEditor is IValueEditor iValueEditor)
					{
						iValueEditor.EditingProperty(effectProperty.Name);
						PropertyModDetails propertyModDetails = BasePropertyChanger.GetPropertyModDetails(Instance, effectProperty.Path);
						Talespire.Log.Warning($"iValueEditor.SetValue(propertyModDetails.GetValue());");
						iValueEditor.SetValue(propertyModDetails.GetValue());
					}
					else
						Talespire.Log.Error($"valueEditor is NOT an IValueEditor!!!");
				}
				else
					Talespire.Log.Error($"value editor NOT found!!!");

				Talespire.Log.Warning($"frmPersistentEffectPropertyEditor.Show();");
				frmPersistentEffectPropertyEditor.Show();
			}
			catch (Exception ex)
			{
				Talespire.Log.Exception(ex, nameof(lstProperties_SelectedIndexChanged));
			}
		}

		private void FrmPropertyList_Load(object sender, EventArgs e)
		{
			ShowPropertyList();
			WindowHelper.FocusTaleSpire();
		}

		public void ValueHasChanged(IValueEditor editor, object value, bool committedChange = false)
		{
			Talespire.Log.Debug($"ValueHasChanged...");
			UpdateInstance(value, committedChange);
		}

		public void PrepForClose()
		{
			windowDragTimer.Stop();
			windowDragTimer.Elapsed -= WindowDragTimer_Elapsed;
			windowDragTimer = null;
			ValueEditors.ValueChanged -= ValueEditors_ValueChanged;
			ValueEditors.Clean(STR_PersistentEffectsEditorKey);
			frmPersistentEffectPropertyEditor?.Close();
			frmPersistentEffectPropertyEditor = null;
		}

		public GameObject Instance { get; set; }
		public CreatureBoardAsset Mini { get; set; }

		private void WindowDragTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			SetLocation(WindowHelper.GetTaleSpireTopRight());
		}
	}
}
