using DndCore;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeLineControl;
using ioPath = System.IO.Path;

namespace DndUI
{
	/// <summary>
	/// Interaction logic for GroupEffectBuilder.xaml
	/// </summary>
	public partial class GroupEffectBuilder : UserControl
	{
		public static readonly RoutedEvent PropertyChangedEvent = EventManager.RegisterRoutedEvent("PropertyChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GroupEffectBuilder));

		public event RoutedEventHandler PropertyChanged
		{
			add { AddHandler(PropertyChangedEvent, value); }
			remove { RemoveHandler(PropertyChangedEvent, value); }
		}

		protected virtual void OnPropertyChanged()
		{
			RoutedEventArgs eventArgs = new RoutedEventArgs(PropertyChangedEvent);
			RaiseEvent(eventArgs);
		}
		
		static Brush groupBrush = new SolidColorBrush(Color.FromRgb(32, 53, 54));
		static Brush placeholderBrush = new SolidColorBrush(Color.FromRgb(156, 83, 47));
		static Brush soundEffectBrush = new SolidColorBrush(Color.FromRgb(20, 135, 82));
		static Brush emitterBrush = new SolidColorBrush(Color.FromRgb(150, 33, 33));
		static Brush animationBrush = new SolidColorBrush(Color.FromRgb(61, 39, 105));

		public GroupEffectBuilder()
		{
			InitializeComponent();
		}

		ObservableCollection<TimeLineEffect> entries;

		public ObservableCollection<TimeLineEffect> Entries
		{
			get
			{
				if (entries == null)
					entries = new ObservableCollection<TimeLineEffect>();
				return entries;
			}
			set
			{
				entries = value;
				tlEffects.ItemsSource = entries;
			}
		}

		bool loadingInternally;

		TimeSpan GetEffectDuration(EffectEntry effectEntry)
		{
			switch (effectEntry.EffectKind)
			{
				case EffectKind.Animation:
					return GetAnimationDuration(effectEntry.AnimationEffect.spriteName);
				case EffectKind.Emitter:
					return GetEmitterDuration(effectEntry.EmitterEffect); ;
				case EffectKind.SoundEffect:
					return GetSoundFileDuration(effectEntry.SoundEffect.soundFileName);
			}
			return TimeSpan.FromSeconds(1);
		}

		public TimeLineEffect AddEntry(TimeSpan start, TimeSpan duration, string name, EffectEntry data)
		{
			TimeLineEffect timeLineEntry = new TimeLineEffect() { Start = start, Duration = duration, Name = name, Effect = data, Index = Entries.Count };
			Entries.Add(timeLineEntry);
			return timeLineEntry;
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button))
				return;

			FrmPickOne frmPickOne = new FrmPickOne();
			Point screenPos = button.PointToScreen(new Point(0, 0));
			frmPickOne.Left = screenPos.X;
			frmPickOne.Top = screenPos.Y;
			frmPickOne.lbChoices.ItemsSource = EditableListBox.LoadEntriesFromFile<EffectEntry>("AllEffects.json");
			if (frmPickOne.ShowDialog() == true)
			{
				if (frmPickOne.SelectedEntry is EffectEntry effectEntry)
				{
					string entryName = effectEntry.Name;

					TimeLineEffect newEntry = AddEntry(TimeSpan.Zero, GetEffectDuration(effectEntry), entryName, effectEntry);
					newEntry.PropertyChanged += Entry_PropertyChanged;
					OnPropertyChanged();
				}
			}
		}

		private void Entry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Duration")
			{
				if (sender is TimeLineEntry entry && entry.Data is EffectEntry effect)
				{
					if (effect.EffectKind == EffectKind.Emitter)
					{
						double totalSeconds = entry.Duration.TotalSeconds;
						if (entry.Duration == System.Threading.Timeout.InfiniteTimeSpan)
							effect.EmitterEffect.maxTotalParticles = double.PositiveInfinity;
						else
							effect.EmitterEffect.maxTotalParticles = Math.Round(effect.EmitterEffect.particlesPerSecond * totalSeconds);

						effectBuilder.LoadFromItem(effect);
					}
				}
			}
			OnPropertyChanged();
		}

		private string GetEntryName()
		{
			return "Effect" + Entries.Count;
		}

		private void BtnAddNewSoundEffect_Click(object sender, RoutedEventArgs e)
		{
			CreateNewEffect(EffectKind.SoundEffect);
			effectBuilder.PickSoundFile();
			//effectBuilder.EffectEntry
		}

		private void BtnAddNewEmitter_Click(object sender, RoutedEventArgs e)
		{
			CreateNewEffect(EffectKind.Emitter);
		}

		private void CreateNewEffect(EffectKind effectKind)
		{
			EffectEntry newEffect = new EffectEntry(effectKind, GetEntryName());
			TimeLineEntry entry = AddEntry(TimeSpan.Zero, TimeSpan.FromSeconds(1), GetEntryName(), newEffect);

			entry.PropertyChanged += Entry_PropertyChanged;

			tlEffects.SelectedItem = entry;
			SetSelectedColorByEffectType(effectKind);

			loadingInternally = true;
			try
			{
				effectBuilder.EffectEntry = newEffect;
			}
			finally
			{
				loadingInternally = false;
			}
			OnPropertyChanged();
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			tlEffects.DeleteSelected();
		}

		private void BtnAddNewAnimation_Click(object sender, RoutedEventArgs e)
		{
			CreateNewEffect(EffectKind.Animation);
		}

		TimeSpan GetSoundFileDuration(string soundFileName)
		{
			TimeSpan defaultDuration = TimeSpan.FromSeconds(1);
			if (string.IsNullOrWhiteSpace(soundFileName))
				return defaultDuration;

			string fullPath = ioPath.Combine(EffectBuilder.SoundFolder, soundFileName);

			WaveStream fileReader = null;

			string fileExtension = ioPath.GetExtension(fullPath);

			if (fileExtension == ".mp3")
				fileReader = new Mp3FileReader(fullPath);

			else if (fileExtension == ".wav")
				fileReader = new WaveFileReader(fullPath);

			if (fileReader == null)
				return defaultDuration;

			return fileReader.TotalTime;
		}

		void AdjustSelectedEntryDuration(TimeSpan duration, bool lockTimeline, string newName)
		{
			if (tlEffects.SelectedItem is TimeLineEntry timeLineEntry)
			{
				timeLineEntry.Duration = duration;
				timeLineEntry.DurationLocked = lockTimeline;
				if (newName != null)
					timeLineEntry.Name = newName;
			}
		}

		TimeSpan GetAnimationDuration(string name)
		{
			double fps30 = 33; // ms
												 //double fps20 = 50; // ms
			double fps15 = 67; // ms
			if (name == "SparkShower")
				return TimeSpan.FromMilliseconds(fps30 * 63);
			if (name == "Poof")
				return TimeSpan.FromMilliseconds(fps30 * 67);
			if (name == "DenseSmoke")
				return TimeSpan.FromMilliseconds(fps30 * 115);
			if (name == "EmbersLarge")
				return TimeSpan.FromMilliseconds(fps30 * 93);
			if (name == "EmbersMedium")
				return TimeSpan.FromMilliseconds(fps30 * 91);
			if (name == "FireBall")
				return TimeSpan.FromMilliseconds(fps30 * 88);
			if (name == "Stars")
				return System.Threading.Timeout.InfiniteTimeSpan;
			if (name == "Restrained")
				return System.Threading.Timeout.InfiniteTimeSpan;
			if (name == "Heart")
				return System.Threading.Timeout.InfiniteTimeSpan;
			if (name == "Fumes")
				return System.Threading.Timeout.InfiniteTimeSpan;
			if (name == "BloodGush")
				return TimeSpan.FromMilliseconds(fps30 * 77);
			if (name == "BloodLarger")
				return TimeSpan.FromMilliseconds(fps15 * 50);
			if (name == "BloodLarge")
				return TimeSpan.FromMilliseconds(fps15 * 50);
			if (name == "BloodMedium")
				return TimeSpan.FromMilliseconds(fps15 * 50);
			if (name == "BloodSmall")
				return TimeSpan.FromMilliseconds(fps15 * 50);
			if (name == "BloodSmaller")
				return TimeSpan.FromMilliseconds(fps15 * 50);
			if (name == "BloodSmallest")
				return TimeSpan.FromMilliseconds(fps15 * 50);
			return TimeSpan.Zero;
		}

		TimeSpan GetEmitterDuration(EmitterEffect emitterEffect)
		{
			if (emitterEffect.maxTotalParticles == double.PositiveInfinity)
				return System.Threading.Timeout.InfiniteTimeSpan;
			return TimeSpan.FromSeconds(emitterEffect.maxTotalParticles / emitterEffect.particlesPerSecond);
		}

		void SetSelectedColorByEffectType(EffectKind effectKind)
		{
			if (tlEffects.SelectedItem is TimeLineEntry timeLineEntry)
			{
				switch (effectKind)
				{
					case EffectKind.Animation:
						timeLineEntry.Fill = animationBrush;
						break;
					case EffectKind.Emitter:
						timeLineEntry.Fill = emitterBrush;
						break;
					case EffectKind.SoundEffect:
						timeLineEntry.Fill = soundEffectBrush;
						break;
					case EffectKind.GroupEffect:
						timeLineEntry.Fill = groupBrush;
						break;
				}

			}
		}

		private void EffectBuilder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (loadingInternally)
				return;
			if (!(sender is EffectBuilder effectBuilder))
				return;

			EffectEntry effectEntry = effectBuilder.EffectEntry;
			if (effectEntry == null)
				return;
			SetSelectedColorByEffectType(effectEntry.EffectKind);
			if (effectBuilder.EffectKind == EffectKind.SoundEffect)
			{
				string soundFileName = effectEntry.SoundEffect.soundFileName;
				AdjustSelectedEntryDuration(GetSoundFileDuration(soundFileName), true, soundFileName);
			}
			else if (effectBuilder.EffectKind == EffectKind.Animation)
			{
				AnimationEffect animationEffect = effectEntry.AnimationEffect;
				TimeSpan duration = GetAnimationDuration(animationEffect.spriteName);
				if (animationEffect.name != "")
				{
					AdjustSelectedEntryDuration(duration, false, animationEffect.name);
				}
				else
				{
					AdjustSelectedEntryDuration(duration, false, animationEffect.spriteName);
				}
			}
			else if (effectBuilder.EffectKind == EffectKind.Emitter)
			{
				EmitterEffect emitterEffect = effectEntry.EmitterEffect;
				TimeSpan duration = GetEmitterDuration(emitterEffect);
				AdjustSelectedEntryDuration(duration, false, null);
			}

			OnPropertyChanged();
		}

		private void TlEffects_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			loadingInternally = true;
			try
			{
				if (sender is TimeLine timeLine && timeLine.SelectedItem is TimeLineEntry entry && entry.Data is EffectEntry effect)
				{
					effectBuilder.Visibility = Visibility.Visible;
					effectBuilder.EffectEntry = effect;
				}
				else
					effectBuilder.Visibility = Visibility.Hidden;
			}
			finally
			{
				loadingInternally = false;
			}
		}

		private void BtnTestGroupEffect_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BtnAddNewPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			EffectEntry newPlaceholder = new EffectEntry(EffectKind.Animation, "Placeholder");
			TimeLineEntry entry = AddEntry(TimeSpan.Zero, TimeSpan.FromSeconds(1), "Placeholder", newPlaceholder);
			entry.Fill = placeholderBrush;
			entry.PropertyChanged += Entry_PropertyChanged;
			tlEffects.SelectedItem = entry;

			effectBuilder.Visibility = Visibility.Hidden;
			OnPropertyChanged();
		}

		private void TlEffects_TimeLineChanged(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged();
		}
	}
}
