using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GoogleHelper
{
	public class TrackPropertyChanges : INotifyPropertyChanged, ITrackPropertyChanges
	{
		public TrackPropertyChanges OwningTracker;
		public event PropertyChangedEventHandler PropertyChanged;
		public List<string> ChangedProperties { get; set; }
		bool isDirty;
		public bool IsDirty
		{
			get => isDirty;
			set
			{
				if (isDirty == value)
					return;
				isDirty = value;
				if (!isDirty)
					ChangedProperties = null;

				OnPropertyChanged();

				if (isDirty && OwningTracker != null)
					OwningTracker.IsDirty = true;
			}
		}

		void AddChangedProperty(string propertyName)
		{
			if (ChangedProperties == null)
				ChangedProperties = new List<string>();
			if (ChangedProperties.IndexOf(propertyName) < 0)
				ChangedProperties.Add(propertyName);
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (propertyName != "IsDirty")
			{
				AddChangedProperty(propertyName);
				IsDirty = true;
			}
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public TrackPropertyChanges()
		{

		}
	}
}
