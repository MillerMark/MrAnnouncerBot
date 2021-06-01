﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace TaleSpireExplore
{
	public class AllValueEditors: IValueChangedListener
	{
		public event ValueChangedEventHandler ValueChanged;

		protected virtual void OnValueChanged(object sender, ValueChangedEventArgs ea)
		{
			ValueChanged?.Invoke(sender, ea);
		}

		Dictionary<Type, IValueEditor> editors = new Dictionary<Type, IValueEditor>();

		public Dictionary<Type, IValueEditor> Editors { get => editors; }

		public UserControl Register(IValueEditor valueEditor)
		{
			Type key = valueEditor.GetValueType();
			if (editors.ContainsKey(key))
			{
				MessageBox.Show($"Value editor for this type {key.FullName} already registered...", "Error");
				return null;
			}

			editors.Add(key, valueEditor);

			valueEditor.Initialize(this);
			return valueEditor as UserControl;
		}

		public IValueEditor Get(Type key)
		{
			if (editors.ContainsKey(key))
				return editors[key];
			return null;
		}

		public void ValueHasChanged(IValueEditor editor, object value)
		{
			TaleSpireCore.Talespire.Log.Debug("value changed...");
			OnValueChanged(this, new ValueChangedEventArgs(editor, value));
		}
	}
}