using System;
using System.Linq;

namespace TaleSpireExplore
{
	public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs ea);
	public class ValueChangedEventArgs : EventArgs
	{
		public IValueEditor Editor { get; set; }
		public object Value { get; set; }
		public bool CommittedChange { get; set; }

		public ValueChangedEventArgs(IValueEditor editor, object value, bool committedChange)
		{
			CommittedChange = committedChange;
			Editor = editor;
			Value = value;
		}
		public ValueChangedEventArgs()
		{

		}
	}
}
