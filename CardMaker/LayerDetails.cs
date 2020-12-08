using System;
using DndCore;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CardMaker
{
	public class LayerDetails : INotifyPropertyChanged
	{
		public string Name
		{
			get => name;
			set
			{
				if (name == value)
					return;
				name = value;
				if (string.IsNullOrWhiteSpace(DisplayName))
					DisplayName = Name;
				OnPropertyChanged();
			}
		}

		bool isVisible = true;
		public bool IsVisible
		{
			get => isVisible;
			set
			{
				if (isVisible == value)
					return;
				isVisible = value;
				isHidden = !isVisible;
				OnPropertyChanged();
				OnPropertyChanged("IsHidden");
			}
		}

		bool isHidden;
		public bool IsHidden
		{
			get => isHidden;
			set
			{
				if (isHidden == value)
					return;
				isHidden = value;
				isVisible = !isHidden;
				OnPropertyChanged();
				OnPropertyChanged("IsVisible");
			}
		}

		bool isSelected;
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				if (isSelected == value)
					return;
				isSelected = value;
				OnPropertyChanged();
			}
		}

		public int Angle
		{
			get => angle;
			set
			{
				if (angle == value)
					return;
				angle = value;
				OnPropertyChanged();
			}
		}

		
		public double Hue
		{
			get => hue;
			set
			{
				if (hue == value)
					return;
				hue = value;
				OnPropertyChanged();
			}
		}

		
		public double Sat
		{
			get => sat;
			set
			{
				if (sat == value)
					return;
				sat = value;
				OnPropertyChanged();
			}
		}

		
		public double Light
		{
			get => light;
			set
			{
				if (light == value)
					return;
				light = value;
				OnPropertyChanged();
			}
		}

		
		public double Contrast
		{
			get => contrast;
			set
			{
				if (contrast == value)
					return;
				contrast = value;
				OnPropertyChanged();
			}
		}

		
		public double OffsetX
		{
			get => offsetX;
			set
			{
				if (offsetX == value)
					return;
				offsetX = value;
				OnPropertyChanged();
			}
		}
		public double OffsetY
		{
			get => offsetY;
			set
			{
				if (offsetY == value)
					return;
				offsetY = value;
				OnPropertyChanged();
			}
		}

		
		public double ScaleX
		{
			get => scaleX;
			set
			{
				if (scaleX == value)
					return;
				scaleX = value;
				OnPropertyChanged();
			}
		}

		
		public double ScaleY
		{
			get => scaleY;
			set
			{
				if (scaleY == value)
					return;
				scaleY = value;
				OnPropertyChanged();
			}
		}

		
		public string FileName
		{
			get => fileName;
			set
			{
				if (fileName == value)
					return;
				fileName = value;
				OnPropertyChanged();
			}
		}
		
		public string DisplayName
		{
			get => displayName;
			set
			{
				if (displayName == value)
					return;
				displayName = value;
				OnPropertyChanged();
			}
		}

		public LayerDetails()
		{

		}

		string displayName;
		string fileName;
		double scaleX = 1;
		double scaleY = 1;
		double offsetX;
		double offsetY;
		double contrast;
		double light;
		double sat;
		double hue;
		int angle;
		string name;
		public static PropertyInfo[] properties = null;

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		void AssignValue(string propertyName, string value)
		{
			if (properties == null)
				properties = typeof(LayerDetails).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			PropertyInfo propInfo = properties.FirstOrDefault(x => string.Compare(x.Name, propertyName, true) == 0);
			if (propInfo == null)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}

			propInfo.SetValueFromString(this, value);
		}

		public void AddAssignment(string assignment)
		{
			int equalsPos = assignment.IndexOf('=');
			if (equalsPos < 0)
			{
				System.Diagnostics.Debugger.Break();
				return;
			}
			string propertyName = assignment.EverythingBefore("=").Trim();
			string value = assignment.EverythingAfter("=").Trim();

			AssignValue(propertyName, value);
		}

		void GetPropertyAssignmentIfDifferent<T>(StringBuilder propertyChangeList, string propName, T value, T defaultValue = default)
		{
			if (value.Equals(defaultValue))
				return;

			if (propertyChangeList.Length > 0)
				propertyChangeList.Append(", ");

			propertyChangeList.Append($"{propName}={value}");
		}

		public string GetStr()
		{
			StringBuilder propertyChangeList = new StringBuilder();
			// TODO: Consider using reflection to iterate these public properties.
			GetPropertyAssignmentIfDifferent(propertyChangeList, "Angle", Angle);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "ScaleX", ScaleX, 1.0);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "ScaleY", ScaleY, 1.0);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "OffsetX", OffsetX);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "OffsetY", OffsetY);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "Hue", Hue);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "Light", Light);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "Sat", Sat);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "Contrast", Contrast);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "IsHidden", IsHidden, false);
			GetPropertyAssignmentIfDifferent(propertyChangeList, "IsSelected", IsSelected, false);
			if (propertyChangeList.Length == 0)
				return string.Empty;
			return $"{Name}({propertyChangeList})";
		}
	}
}
