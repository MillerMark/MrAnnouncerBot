using System;
using DndCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace DHDM
{
	/// <summary>
	/// Interaction logic for EffectBuilder.xaml
	/// </summary>
	public partial class EffectBuilder : UserControl, INotifyPropertyChanged
	{
		//private fields...

		const string STR_EffectKind = "EffectKind";
		public EffectKind EffectKind
		{
			// IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
			get
			{
				return (EffectKind)GetValue(EffectKindProperty);
			}
			set
			{
				SetValue(EffectKindProperty, value);
			}
		}
		public string ScreenPosCoordinates
		{
			get
			{
				return txtCoordinates.Text;
			}
			set
			{
				if (value == txtCoordinates.Text)
					return;
				txtCoordinates.Text = value;
				OnPropertyChanged();
			}
		}

		public static readonly DependencyProperty EffectKindProperty = DependencyProperty.Register(STR_EffectKind, typeof(EffectKind), typeof(EffectBuilder), new FrameworkPropertyMetadata(EffectKind.Animation, new PropertyChangedCallback(OnEffectKindChanged)));

		public TargetType TargetType
		{
			get
			{
				return GetVisualEffectTarget();
			}
			set
			{
				TargetType activeTargetType = GetVisualEffectTarget();
				if (value == activeTargetType)
					return;
				if (value == TargetType.ActiveEnemy)
					rbActiveEnemy.IsChecked = true;
				else if (value == TargetType.ActivePlayer)
					rbActivePlayer.IsChecked = true;
				else if (value == TargetType.ScreenPosition)
					rbScreenPos.IsChecked = true;
				else
					rbScrollPos.IsChecked = true;

				OnPropertyChanged();
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		private static void OnEffectKindChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			EffectBuilder effectBuilder = o as EffectBuilder;
			if (effectBuilder != null)
				effectBuilder.OnEffectKindChanged((EffectKind)e.OldValue, (EffectKind)e.NewValue);
		}

		protected virtual void OnEffectKindChanged(EffectKind oldValue, EffectKind newValue)
		{
			EffectKind activeEffect = GetActiveEffect();
			if (newValue == activeEffect)
				return;
			if (newValue == EffectKind.Animation)
				rbAnimation.IsChecked = true;
			else if (newValue == EffectKind.Emitter)
				rbEmitter.IsChecked = true;
			else
				rbSoundEffect.IsChecked = true;
		}

		protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private EffectKind GetActiveEffect()
		{
			if (rbAnimation.IsChecked ?? false)
				return EffectKind.Animation;
			if (rbSoundEffect.IsChecked ?? false)
				return EffectKind.SoundEffect;
			return EffectKind.Emitter;
		}

		public EffectBuilder()
		{
			InitializeComponent();
		}

		private void RbAnimation_Checked(object sender, RoutedEventArgs e)
		{
			if (spTargetOptions != null)
				spTargetOptions.Visibility = Visibility.Visible;
			if (spEmitterOptions != null)
				spEmitterOptions.Visibility = Visibility.Collapsed;
			if (spAnimationOptions != null)
				spAnimationOptions.Visibility = Visibility.Visible;
			if (spSoundOptions != null)
				spSoundOptions.Visibility = Visibility.Collapsed;
			SettingsChanged();
			OnPropertyChanged(STR_EffectKind);
		}

		private void RbEmitter_Checked(object sender, RoutedEventArgs e)
		{
			spTargetOptions.Visibility = Visibility.Visible;
			spEmitterOptions.Visibility = Visibility.Visible;
			spAnimationOptions.Visibility = Visibility.Collapsed;
			spSoundOptions.Visibility = Visibility.Collapsed;
			SettingsChanged();
			OnPropertyChanged(STR_EffectKind);
		}

		private void RbSoundEffect_Checked(object sender, RoutedEventArgs e)
		{
			spTargetOptions.Visibility = Visibility.Collapsed;
			spEmitterOptions.Visibility = Visibility.Collapsed;
			spAnimationOptions.Visibility = Visibility.Collapsed;
			spSoundOptions.Visibility = Visibility.Visible;
			tbxSoundFileName.Focus();
			SettingsChanged();
			OnPropertyChanged(STR_EffectKind);
		}

		void SettingsChanged()
		{

		}
		private void RbActivePlayer_Checked(object sender, RoutedEventArgs e)
		{
			if (spScrollTarget != null)
				spScrollTarget.Visibility = Visibility.Collapsed;
			SettingsChanged();
			OnPropertyChanged("TargetType");
		}

		private void RbActiveEnemy_Checked(object sender, RoutedEventArgs e)
		{
			spScrollTarget.Visibility = Visibility.Collapsed;
			SettingsChanged();
			OnPropertyChanged("TargetType");
		}

		private void RbScrollPos_Checked(object sender, RoutedEventArgs e)
		{
			spScrollTarget.Visibility = Visibility.Visible;
			SettingsChanged();
			OnPropertyChanged("TargetType");
		}

		// TODO: Add an event handler for txtCoordinates mouse down to check RbScreenPos if not checked.
		private void RbScreenPos_Checked(object sender, RoutedEventArgs e)
		{
			spScrollTarget.Visibility = Visibility.Collapsed;
			SettingsChanged();
			OnPropertyChanged("TargetType");
		}

		private void Any_TextChanged(object sender = null, TextChangedEventArgs e = null)
		{
			SettingsChanged();
			OnPropertyChanged("AnyText");
		}

		private void RbnPageMain_Checked(object sender, RoutedEventArgs e)
		{
			if (cmbMainItems != null)
				cmbMainItems.Visibility = Visibility.Visible;
			if (cmbSkillItems != null)
				cmbSkillItems.Visibility = Visibility.Collapsed;
			if (cmbEquipmentItems != null)
				cmbEquipmentItems.Visibility = Visibility.Collapsed;
			SettingsChanged();
		}

		private void RbnPageSkills_Checked(object sender, RoutedEventArgs e)
		{
			cmbMainItems.Visibility = Visibility.Collapsed;
			cmbSkillItems.Visibility = Visibility.Visible;
			cmbEquipmentItems.Visibility = Visibility.Collapsed;
			SettingsChanged();
		}

		private void RbnPageEquipment_Checked(object sender, RoutedEventArgs e)
		{
			cmbMainItems.Visibility = Visibility.Collapsed;
			cmbSkillItems.Visibility = Visibility.Collapsed;
			cmbEquipmentItems.Visibility = Visibility.Visible;
			SettingsChanged();
		}

		private void AnyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			OnPropertyChanged("AnyCombo");
			SettingsChanged();
		}

		private void RbEmitterRound_Checked(object sender, RoutedEventArgs e)
		{
			if (spCircularOptions != null)
				spCircularOptions.Visibility = Visibility.Visible;
			if (spRectangularOptions != null)
				spRectangularOptions.Visibility = Visibility.Collapsed;
			OnPropertyChanged("EmitterKind");
			SettingsChanged();
		}

		private void RbEmitterRectangular_Checked(object sender, RoutedEventArgs e)
		{
			if (spCircularOptions != null)
				spCircularOptions.Visibility = Visibility.Collapsed;
			if (spRectangularOptions != null)
				spRectangularOptions.Visibility = Visibility.Visible;
			OnPropertyChanged("EmitterKind");
			SettingsChanged();
		}

		private void AnyNumEdit_Changed(object sender = null, RoutedEventArgs e = null)
		{
			OnPropertyChanged("AnyNumEdit");
			SettingsChanged();
		}

		private void CkbRenderOldestParticlesLast_Checked(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("RenderOldestParticlesLast");
			SettingsChanged();
		}

		private void CkbRenderOldestParticlesLast_Unchecked(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("RenderOldestParticlesLast");
			SettingsChanged();
		}

		private void AnyTargetValue_Changed(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("AnyTargetValue");
			SettingsChanged();
		}

		private void NedEmitterGravity_TextChanged(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("EmitterGravity");
			if (spEmitterGravityCenter == null)
				return;
			string emitterGravity = nedEmitterGravity.Value.Trim();
			bool noGravity = emitterGravity == "0" || emitterGravity == string.Empty;

			if (noGravity)
				spEmitterGravityCenter.Visibility = Visibility.Collapsed;
			else
				spEmitterGravityCenter.Visibility = Visibility.Visible;
		}

		private void NedParticleGravity_TextChanged(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("ParticleGravity");
			if (spParticleGravityCenter == null)
				return;
			string particleGravity = nedParticleGravity.Value.Trim();
			bool noGravity = particleGravity == "0" || particleGravity == string.Empty;

			if (noGravity)
				spParticleGravityCenter.Visibility = Visibility.Collapsed;
			else
				spParticleGravityCenter.Visibility = Visibility.Visible;
		}


		public AnimationSprites AnimationSprites
		{
			get
			{
				return GetActiveAnimation();
			}
			set
			{
				AnimationSprites activeAnimation = GetActiveAnimation();
				if (activeAnimation == value)
					return;

				OnPropertyChanged();

				if (value == AnimationSprites.Smoke)
					rbDenseSmoke.IsChecked = true;
				else if (value == AnimationSprites.Poof)
					rbPoof.IsChecked = true;
				else if (value == AnimationSprites.EmbersLarge)
				{
					rbEmbers.IsChecked = true;
					rbEmbersLarge.IsChecked = true;
				}
				else if (value == AnimationSprites.EmbersMedium)
				{
					rbEmbers.IsChecked = true;
					rbEmbersMedium.IsChecked = true;
				}
				else if (value == AnimationSprites.Sparks)
					rbSparkShower.IsChecked = true;
				else if (value == AnimationSprites.BloodGush)
				{
					rbBloodGush.IsChecked = true;
					rbBloodSplatter.IsChecked = true;
				}
				else if (value == AnimationSprites.BloodLarger)
				{
					rbBloodLarger.IsChecked = true;
					rbBloodSplatter.IsChecked = true;
				}
				else if (value == AnimationSprites.BloodLarge)
				{
					rbBloodLarge.IsChecked = true;
					rbBloodSplatter.IsChecked = true;
				}
				else if (value == AnimationSprites.BloodMedium)
				{
					rbBloodMedium.IsChecked = true;
					rbBloodSplatter.IsChecked = true;
				}
				else if (value == AnimationSprites.BloodSmall)
				{
					rbBloodSmall.IsChecked = true;
					rbBloodSplatter.IsChecked = true;
				}
				else if (value == AnimationSprites.BloodSmaller)
				{
					rbBloodSmaller.IsChecked = true;
					rbBloodSplatter.IsChecked = true;
				}
				else if (value == AnimationSprites.BloodSmallest)
				{
					rbBloodSmallest.IsChecked = true;
					rbBloodSplatter.IsChecked = true;
				}
				else if (value == AnimationSprites.Fumes)
					rbFumes.IsChecked = true;
				else if (value == AnimationSprites.Stars)
					rbStars.IsChecked = true;
				else if (value == AnimationSprites.Restrained)
					rbRestrained.IsChecked = true;
				else if (value == AnimationSprites.Heart)
					rbHeart.IsChecked = true;
				else
					return;
				OnPropertyChanged("AnimationSprites");
			}
		}

		private AnimationSprites GetActiveAnimation()
		{
			if (rbDenseSmoke.IsChecked ?? false)
				return AnimationSprites.Smoke;
			if (rbPoof.IsChecked ?? false)
				return AnimationSprites.Poof;
			if (rbEmbers.IsChecked ?? false)
				if (rbEmbersLarge.IsChecked ?? false)
					return AnimationSprites.EmbersLarge;
				else
					return AnimationSprites.EmbersMedium;

			if (rbSparkShower.IsChecked ?? false)
				return AnimationSprites.Sparks;

			if (rbBloodSplatter.IsChecked ?? false)
				if (rbBloodGush.IsChecked ?? false)
					return AnimationSprites.BloodGush;
				else if (rbBloodLarger.IsChecked ?? false)
					return AnimationSprites.BloodLarger;
				else if (rbBloodLarge.IsChecked ?? false)
					return AnimationSprites.BloodLarge;
				else if (rbBloodMedium.IsChecked ?? false)
					return AnimationSprites.BloodMedium;
				else if (rbBloodSmall.IsChecked ?? false)
					return AnimationSprites.BloodSmall;
				else if (rbBloodSmaller.IsChecked ?? false)
					return AnimationSprites.BloodSmaller;
				else if (rbBloodSmallest.IsChecked ?? false)
					return AnimationSprites.BloodSmallest;

			if (rbFumes.IsChecked ?? false)
				return AnimationSprites.Fumes;
			if (rbStars.IsChecked ?? false)
				return AnimationSprites.Stars;
			if (rbRestrained.IsChecked ?? false)
				return AnimationSprites.Restrained;
			if (rbHeart.IsChecked ?? false)
				return AnimationSprites.Heart;

			return AnimationSprites.Unknown;
		}

		private void RbDenseSmoke_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
			OnPropertyChanged("AnimationSprites");
		}

		private void RbPoof_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
			OnPropertyChanged("AnimationSprites");
		}

		private void RbSparkle_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
			OnPropertyChanged("AnimationSprites");
		}

		private void RbEmbers_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Visible;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
			OnPropertyChanged("AnimationSprites");
		}

		private void RbBloodSplatter_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Visible;
			OnPropertyChanged("AnimationSprites");
		}

		private void RbFumes_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
			OnPropertyChanged("AnimationSprites");
		}

		private void RbStars_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
			OnPropertyChanged("AnimationSprites");
		}

		private void BtnNavSoundFile_Click(object sender, RoutedEventArgs e)
		{
			// Create OpenFileDialog 
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

			// Set filter for file extension and default file extension 
			dlg.DefaultExt = ".mp3";
			dlg.Filter = "MP3 Files (*.mp3)|*.mp3|WAV Files (*.wav)|*.wav";
			const string soundFolder = @"D:\Dropbox\DX\Twitch\CodeRushed\MrAnnouncerBot\OverlayManager\wwwroot\GameDev\Assets\DragonH\SoundEffects";
			dlg.InitialDirectory = soundFolder;

			// Display OpenFileDialog by calling ShowDialog method 
			bool? result = dlg.ShowDialog();

			// Get the selected file name and display in a TextBox 
			if (result == true)
			{
				// Open document 
				string filename = dlg.FileName;
				if (filename.StartsWith(soundFolder + "\\"))
					filename = filename.Substring(soundFolder.Length + 1);
				tbxSoundFileName.Text = filename;
			}
		}

		private void AnyRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("AnyRadioButton");
			SettingsChanged();
		}
		Effect GetSoundEffect()
		{
			return new SoundEffect(tbxSoundFileName.Name);
		}
		string GetAnimationName()
		{
			if (rbDenseSmoke.IsChecked ?? false)
				return "DenseSmoke";
			if (rbPoof.IsChecked ?? false)
				return "Poof";
			if (rbEmbers.IsChecked ?? false)
				if (rbEmbersLarge.IsChecked ?? false)
					return "EmbersLarge";
				else
					return "EmbersMedium";

			if (rbSparkShower.IsChecked ?? false)
				return "SparkShower";

			if (rbBloodSplatter.IsChecked ?? false)
				if (rbBloodGush.IsChecked ?? false)
					return "BloodGush";
				else if (rbBloodLarger.IsChecked ?? false)
					return "BloodLarger";
				else if (rbBloodLarge.IsChecked ?? false)
					return "BloodLarge";
				else if (rbBloodMedium.IsChecked ?? false)
					return "BloodMedium";
				else if (rbBloodSmall.IsChecked ?? false)
					return "BloodSmall";
				else if (rbBloodSmaller.IsChecked ?? false)
					return "BloodSmaller";
				else if (rbBloodSmallest.IsChecked ?? false)
					return "BloodSmallest";

			if (rbFumes.IsChecked ?? false)
				return "Fumes";
			if (rbStars.IsChecked ?? false)
				return "Stars";
			if (rbRestrained.IsChecked ?? false)
				return "Restrained";
			if (rbHeart.IsChecked ?? false)
				return "Heart";

			return string.Empty;
		}

		void SetFromAnimationName(string spriteName)
		{
			if (spriteName == "DenseSmoke" || spriteName == "")
				rbDenseSmoke.IsChecked = true;
			else if (spriteName == "Poof")
				rbPoof.IsChecked = true;
			else if (spriteName == "EmbersLarge")
			{
				rbEmbers.IsChecked = true;
				rbEmbersLarge.IsChecked = true;
			}
			else if (spriteName == "EmbersMedium")
			{
				rbEmbers.IsChecked = true;
				rbEmbersMedium.IsChecked = true;
			}
			else if (spriteName == "SparkShower")
				rbSparkShower.IsChecked = true;
			else if (spriteName.StartsWith("Blood"))
			{
				rbBloodSplatter.IsChecked = true;
				if (spriteName == "BloodGush")
					rbBloodGush.IsChecked = true;
				else if (spriteName == "BloodLarger")
					rbBloodLarger.IsChecked = true;
				else if (spriteName == "BloodLarge")
					rbBloodLarge.IsChecked = true;
				else if (spriteName == "BloodMedium")
					rbBloodMedium.IsChecked = true;
				else if (spriteName == "BloodSmall")
					rbBloodSmall.IsChecked = true;
				else if (spriteName == "BloodSmaller")
					rbBloodSmaller.IsChecked = true;
				else if (spriteName == "BloodSmallest")
					rbBloodSmallest.IsChecked = true;
			}
			else if (spriteName == "Fumes")
				rbFumes.IsChecked = true;

			else if (spriteName == "Stars")
				rbStars.IsChecked = true;

			else if (spriteName == "Restrained")
				rbRestrained.IsChecked = true;
			else if (spriteName == "rbHeart")
				rbHeart.IsChecked = true;
		}

		double GetNum(string str)
		{
			if (double.TryParse(str.Trim(), out double result))
				return result;
			return 0;
		}

		DndCore.Vector ToVector(string text)
		{
			string[] split = text.Split(',');
			if (split.Length >= 2)
				return new DndCore.Vector(GetNum(split[0]), GetNum(split[1]));
			else
				return DndCore.Vector.zero;
		}

		TargetType GetVisualEffectTarget()
		{
			if (rbActivePlayer.IsChecked ?? false)
				return TargetType.ActivePlayer;
			else if (rbActiveEnemy.IsChecked ?? false)
				return TargetType.ActiveEnemy;
			else if (rbScrollPos.IsChecked ?? false)
				return TargetType.ScrollPosition;
			else
				return TargetType.ScreenPosition;
		}

		VisualEffectTarget GetTarget()
		{
			VisualEffectTarget visualEffectTarget;
			if (rbScreenPos.IsChecked ?? false)
				visualEffectTarget = new VisualEffectTarget(ToVector(txtCoordinates.Text));
			else
				visualEffectTarget = new VisualEffectTarget();

			visualEffectTarget.targetType = GetVisualEffectTarget();
			visualEffectTarget.targetOffset = ToVector(tbxTargetOffset.Text);

			return visualEffectTarget;
		}

		Effect GetAnimationEffect()
		{
			return new AnimationEffect(GetAnimationName(), GetTarget(), 0, nedAdjustHue.ValueAsDouble, nedAdjustSaturation.ValueAsDouble, nedAdjustBrightness.ValueAsDouble);
		}

		TargetValue ToTargetValue(TargetValueEdit tve)
		{
			double relativeVariance = 0;
			double absoluteVariance = 0;
			string varianceStr = tve.Variance.Trim();
			if (varianceStr.EndsWith("%"))          // "0%" <- relative
				relativeVariance = GetNum(varianceStr.Substring(0, varianceStr.Length - 1)) / 100.0;
			else // "1" <- absolute
				absoluteVariance = GetNum(varianceStr);

			double divisor = 1;
			if (tve.Units == "%")
				divisor = 100.0;
			return new TargetValue(GetNum(tve.Value) / divisor, relativeVariance, absoluteVariance, GetNum(tve.Min),
				GetNum(tve.Max), GetNum(tve.Drift), tve.GetTargetBinding());
		}
		Effect GetEmitterEffect()
		{
			EmitterEffect emitterEffect = new EmitterEffect();

			emitterEffect.bonusVelocityVector = ToVector(tbxBonusVelocity.Text);
			emitterEffect.brightness = ToTargetValue(tvBrightness);
			emitterEffect.edgeSpread = nedEdgeSpread.ValueAsDouble / 100.0;
			emitterEffect.effectKind = EffectKind.Emitter;
			emitterEffect.emitterAirDensity = nedAirDensity.ValueAsDouble;
			emitterEffect.emitterGravity = nedEmitterGravity.ValueAsDouble;
			if (rbEmitterRound.IsChecked ?? false)
				emitterEffect.emitterShape = EmitterShape.Circular;
			else
				emitterEffect.emitterShape = EmitterShape.Rectangular;

			emitterEffect.emitterWindDirection = ToVector(tbxWindDirection.Text);
			emitterEffect.emitterInitialVelocity = ToVector(tbxEmitterInitialVelocity.Text);
			emitterEffect.fadeInTime = nedParticleFadeInTime.ValueAsDouble;
			emitterEffect.gravityCenter = ToVector(tbxEmitterGravityCenter.Text);
			emitterEffect.height = nedEmitterHeight.ValueAsDouble;
			emitterEffect.hue = ToTargetValue(tvParticleHue);
			emitterEffect.lifeSpan = nedParticleLifeSpan.ValueAsDouble;
			emitterEffect.maxConcurrentParticles = nedMaxConcurrentParticles.ValueAsDouble;
			emitterEffect.maxOpacity = nedParticleMaxOpacity.ValueAsDouble / 100.0;
			emitterEffect.maxTotalParticles = nedMaxTotalParticles.ValueAsDouble;
			emitterEffect.minParticleSize = nedMinParticleSize.ValueAsDouble;
			emitterEffect.particleAirDensity = nedParticleAirDensity.ValueAsDouble;
			emitterEffect.particleGravity = nedParticleGravity.ValueAsDouble;
			emitterEffect.particleGravityCenter = ToVector(tbxParticleGravityCenter.Text);
			emitterEffect.particleInitialDirection = ToVector(tbxParticleInitialDirection.Text);
			emitterEffect.particleInitialVelocity = ToTargetValue(tvInitialVelocity);
			emitterEffect.particleMass = nedParticleMass.ValueAsDouble;
			emitterEffect.particleRadius = ToTargetValue(tvParticleRadius);
			emitterEffect.particlesPerSecond = nedParticlesPerSecond.ValueAsDouble;
			emitterEffect.particleWindDirection = ToVector(tbxParticleWindDirection.Text);
			emitterEffect.radius = nedEmitterRadius.ValueAsDouble;
			emitterEffect.renderOldestParticlesLast = ckbRenderOldestParticlesLast.IsChecked ?? false;
			emitterEffect.saturation = ToTargetValue(tvParticleSaturation);
			emitterEffect.target = GetTarget();
			emitterEffect.timeOffsetMs = 0;
			emitterEffect.width = nedEmitterWidth.ValueAsDouble;
			return emitterEffect;
		}

		public Effect GetEffect()
		{
			if (rbSoundEffect.IsChecked ?? false)
				return GetSoundEffect();
			else if (rbAnimation.IsChecked ?? false)
				return GetAnimationEffect();
			else if (rbEmitter.IsChecked ?? false)
				return GetEmitterEffect();
			return null;
		}

		string ToVectorString(DndCore.Vector vector)
		{
			return vector.x + ", " + vector.y;
		}

		void LoadFromTarget(VisualEffectTarget visualEffectTarget)
		{
			if (visualEffectTarget == null)
				return;

			switch (visualEffectTarget.targetType)
			{
				case TargetType.ActivePlayer:
					rbActivePlayer.IsChecked = true;
					break;

				case TargetType.ActiveEnemy:
					rbActiveEnemy.IsChecked = true;
					break;

				case TargetType.ScrollPosition:
					rbScrollPos.IsChecked = true;
					break;

				case TargetType.ScreenPosition:
					rbScreenPos.IsChecked = true;
					break;
			}

			switch (visualEffectTarget.targetPage)
			{
				case TargetPage.Main:
					rbnPageMain.IsChecked = true;
					cmbMainItems.SelectedValue = visualEffectTarget.entryName;
					break;

				case TargetPage.Skills:
					rbnPageSkills.IsChecked = true;
					cmbSkillItems.SelectedValue = visualEffectTarget.entryName;
					break;

				case TargetPage.Equipment:
					rbnPageEquipment.IsChecked = true;
					cmbEquipmentItems.SelectedValue = visualEffectTarget.entryName;
					break;
			}

			txtCoordinates.Text = ToVectorString(visualEffectTarget.screenPosition);
			tbxTargetOffset.Text = ToVectorString(visualEffectTarget.targetOffset);
		}

		void LoadFromAnimation(AnimationEffect animationEffect)
		{
			SetFromAnimationName(animationEffect.spriteName);
			nedAdjustHue.Value = animationEffect.hueShift.ToString();
			nedAdjustBrightness.Value = animationEffect.brightness.ToString();
			nedAdjustSaturation.Value = animationEffect.saturation.ToString();
			//animationEffect.startFrameIndex;
		}

	void LoadFromSoundEffect(SoundEffect soundEffect)
		{
			tbxSoundFileName.Text = soundEffect.soundFileName;
		}
		public void LoadFromItem(EffectEntry effectEntry)
		{
			if (effectEntry.EffectKind == EffectKind.Animation)
			{
				rbAnimation.IsChecked = true;
				LoadFromTarget(effectEntry.AnimationEffect.target);
			}
			else if (effectEntry.EffectKind == EffectKind.Emitter)
			{
				rbEmitter.IsChecked = true;
				LoadFromTarget(effectEntry.EmitterEffect.target);
			}
			else if (effectEntry.EffectKind == EffectKind.SoundEffect)
				rbSoundEffect.IsChecked = true;

			LoadFromAnimation(effectEntry.AnimationEffect);
			LoadFromEmitter(effectEntry.EmitterEffect);
			LoadFromSoundEffect(effectEntry.SoundEffect);
		}

		void SaveToTarget(VisualEffectTarget visualEffectTarget)
		{
			if (rbActivePlayer.IsChecked ?? false)
				visualEffectTarget.targetType = TargetType.ActivePlayer;
			if (rbActiveEnemy.IsChecked ?? false)
				visualEffectTarget.targetType = TargetType.ActiveEnemy;
			if (rbScrollPos.IsChecked ?? false)
				visualEffectTarget.targetType = TargetType.ScrollPosition;
			if (rbScreenPos.IsChecked ?? false)
				visualEffectTarget.targetType = TargetType.ScreenPosition;


			if (rbnPageMain.IsChecked ?? false)
			{
				visualEffectTarget.targetPage = TargetPage.Main;
				visualEffectTarget.entryName = cmbMainItems.SelectedValue?.ToString();
			}

			if (rbnPageEquipment.IsChecked ?? false)
			{
				visualEffectTarget.targetPage = TargetPage.Equipment;
				visualEffectTarget.entryName = cmbEquipmentItems.SelectedValue?.ToString();
			}

			if (rbnPageSkills.IsChecked ?? false)
			{
				visualEffectTarget.targetPage = TargetPage.Skills;
				visualEffectTarget.entryName = cmbSkillItems.SelectedValue?.ToString();
			}

			visualEffectTarget.screenPosition = ToVector(txtCoordinates.Text);
			visualEffectTarget.targetOffset = ToVector(tbxTargetOffset.Text);
		}
		void SaveToAnimation(AnimationEffect animationEffect)
		{
			animationEffect.spriteName = GetAnimationName();
			animationEffect.hueShift = nedAdjustHue.ValueAsDouble;
			animationEffect.brightness = nedAdjustBrightness.ValueAsDouble;
			animationEffect.saturation = nedAdjustSaturation.ValueAsDouble;
			animationEffect.startFrameIndex = 0;
		}

		void SetTargetValueEdit(TargetValueEdit targetValueEdit, TargetValue targetValue)
		{
			double multiplier = 1;
			if (targetValueEdit.Units == "%")
				multiplier = 100.0;
			targetValueEdit.Value = (targetValue.value * multiplier).ToString();
			if (targetValue.absoluteVariance != 0)
			{
				targetValueEdit.Variance = targetValue.absoluteVariance.ToString();
			}
			else
			{
				targetValueEdit.Variance = (100 * targetValue.relativeVariance).ToString() + "%";
			}
			targetValueEdit.Binding = targetValue.targetBinding.ToString();
			targetValueEdit.Drift = targetValue.drift.ToString();
			targetValueEdit.Min = targetValue.min.ToString();
			targetValueEdit.Max = targetValue.max.ToString();
		}

		void LoadFromEmitter(EmitterEffect emitterEffect)
		{
			if (emitterEffect.emitterShape == EmitterShape.Circular)
			{
				rbEmitterRound.IsChecked = true;
				nedEmitterRadius.ValueAsDouble = emitterEffect.radius;
			}
			else
			{
				rbEmitterRectangular.IsChecked = true;
				nedEmitterWidth.ValueAsDouble = emitterEffect.width;
				nedEmitterHeight.ValueAsDouble = emitterEffect.height;
			}
			nedEdgeSpread.ValueAsDouble = emitterEffect.edgeSpread;
			tbxEmitterInitialVelocity.Text = ToVectorString(emitterEffect.emitterInitialVelocity);
			nedEmitterGravity.ValueAsDouble = emitterEffect.emitterGravity;
			tbxEmitterGravityCenter.Text = ToVectorString(emitterEffect.gravityCenter);

			tbxWindDirection.Text = ToVectorString(emitterEffect.emitterWindDirection);
			nedAirDensity.ValueAsDouble = emitterEffect.emitterAirDensity;
			nedParticlesPerSecond.ValueAsDouble = emitterEffect.particlesPerSecond;
			nedMaxConcurrentParticles.ValueAsDouble = emitterEffect.maxConcurrentParticles;
			nedMaxTotalParticles.ValueAsDouble = emitterEffect.maxTotalParticles;
			nedParticleGravity.ValueAsDouble = emitterEffect.particleGravity;
			tbxParticleGravityCenter.Text = ToVectorString(emitterEffect.particleGravityCenter);
			nedParticleMass.ValueAsDouble = emitterEffect.particleMass;
			SetTargetValueEdit(tvParticleRadius, emitterEffect.particleRadius);
			nedMinParticleSize.ValueAsDouble = emitterEffect.minParticleSize;
			tbxParticleInitialDirection.Text = ToVectorString(emitterEffect.particleInitialDirection);
			SetTargetValueEdit(tvInitialVelocity, emitterEffect.particleInitialVelocity);
			tbxParticleWindDirection.Text = ToVectorString(emitterEffect.particleWindDirection);
			nedParticleAirDensity.ValueAsDouble = emitterEffect.particleAirDensity;
			tbxBonusVelocity.Text = ToVectorString(emitterEffect.bonusVelocityVector);
			ckbRenderOldestParticlesLast.IsChecked = emitterEffect.renderOldestParticlesLast;
			nedParticleFadeInTime.ValueAsDouble = emitterEffect.fadeInTime;
			nedParticleMaxOpacity.ValueAsDouble = emitterEffect.maxOpacity;
			nedParticleLifeSpan.ValueAsDouble = emitterEffect.lifeSpan;

			SetTargetValueEdit(tvParticleHue, emitterEffect.hue);
			SetTargetValueEdit(tvParticleSaturation, emitterEffect.saturation);
			SetTargetValueEdit(tvBrightness, emitterEffect.brightness);
		}

		void SaveToEmitter(EmitterEffect emitterEffect)
		{
			if (rbEmitterRound.IsChecked ?? false)
			{
				emitterEffect.emitterShape = EmitterShape.Circular;
				emitterEffect.radius = nedEmitterRadius.ValueAsDouble;
			}
			else
			{
				emitterEffect.emitterShape = EmitterShape.Rectangular;
				emitterEffect.width = nedEmitterWidth.ValueAsDouble;
				emitterEffect.height = nedEmitterHeight.ValueAsDouble;
			}
			emitterEffect.edgeSpread = nedEdgeSpread.ValueAsDouble;
			emitterEffect.emitterInitialVelocity = ToVector(tbxEmitterInitialVelocity.Text);
			emitterEffect.emitterGravity = nedEmitterGravity.ValueAsDouble;
			emitterEffect.gravityCenter = ToVector(tbxEmitterGravityCenter.Text);
			emitterEffect.emitterWindDirection = ToVector(tbxWindDirection.Text);
			emitterEffect.emitterAirDensity = nedAirDensity.ValueAsDouble;
			emitterEffect.particlesPerSecond = nedParticlesPerSecond.ValueAsDouble;
			emitterEffect.maxConcurrentParticles = nedMaxConcurrentParticles.ValueAsDouble;
			emitterEffect.maxTotalParticles = nedMaxTotalParticles.ValueAsDouble;
			emitterEffect.particleGravity = nedParticleGravity.ValueAsDouble;
			emitterEffect.particleGravityCenter = ToVector(tbxParticleGravityCenter.Text);
			emitterEffect.particleMass = nedParticleMass.ValueAsDouble;
			emitterEffect.particleRadius = ToTargetValue(tvParticleRadius);
			emitterEffect.minParticleSize = nedMinParticleSize.ValueAsDouble;
			emitterEffect.particleInitialDirection = ToVector(tbxParticleInitialDirection.Text);
			emitterEffect.particleInitialVelocity = ToTargetValue(tvInitialVelocity);
			emitterEffect.particleWindDirection = ToVector(tbxParticleWindDirection.Text);
			emitterEffect.particleAirDensity = nedParticleAirDensity.ValueAsDouble;
			emitterEffect.bonusVelocityVector = ToVector(tbxBonusVelocity.Text);
			emitterEffect.renderOldestParticlesLast = ckbRenderOldestParticlesLast.IsChecked ?? false;
			emitterEffect.fadeInTime = nedParticleFadeInTime.ValueAsDouble;
			emitterEffect.maxOpacity = nedParticleMaxOpacity.ValueAsDouble;
			emitterEffect.lifeSpan = nedParticleLifeSpan.ValueAsDouble;
			emitterEffect.hue = ToTargetValue(tvParticleHue);
			emitterEffect.saturation = ToTargetValue(tvParticleSaturation);
			emitterEffect.brightness = ToTargetValue(tvBrightness);
		}
		void SaveToSound(SoundEffect soundEffect)
		{
			soundEffect.soundFileName = tbxSoundFileName.Text;
		}
		public void SaveToItem(EffectEntry effectEntry, string propertyName)
		{
			if (rbAnimation.IsChecked ?? false)
			{
				effectEntry.EffectKind = EffectKind.Animation;
				if (propertyName != STR_EffectKind)
					SaveToTarget(effectEntry.AnimationEffect.target);
				else
					LoadFromTarget(effectEntry.AnimationEffect.target);
			}

			if (rbEmitter.IsChecked ?? false)
			{
				effectEntry.EffectKind = EffectKind.Emitter;
				if (propertyName != STR_EffectKind)
					SaveToTarget(effectEntry.EmitterEffect.target);
				else
					LoadFromTarget(effectEntry.EmitterEffect.target);
			}

			if (rbSoundEffect.IsChecked ?? false)
			{
				effectEntry.EffectKind = EffectKind.SoundEffect;
			}
			SaveToSound(effectEntry.SoundEffect);
			SaveToAnimation(effectEntry.AnimationEffect);
			SaveToEmitter(effectEntry.EmitterEffect);
		}

		private void TxtCoordinates_TextChanged(object sender, TextChangedEventArgs e)
		{
			OnPropertyChanged("ScreenPosCoordinates");
			Any_TextChanged();
		}

		private void TbxTargetOffset_TextChanged(object sender, TextChangedEventArgs e)
		{
			OnPropertyChanged("TargetOffset");
			Any_TextChanged();
		}

		public string TargetOffset
		{
			get
			{
				return tbxTargetOffset.Text;
			}
			set
			{
				if (tbxTargetOffset.Text == value)
					return;
				tbxTargetOffset.Text = value;
				OnPropertyChanged();
			}
		}

		private void RbHeart_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
			OnPropertyChanged("AnimationSprites");
		}

		public string Hue
		{
			get
			{
				return nedAdjustHue.Value;
			}
			set
			{
				if (value == nedAdjustHue.Value)
					return;

				nedAdjustHue.Value = value;
				OnPropertyChanged();
			}
		}

		public string Saturation
		{
			get
			{
				return nedAdjustSaturation.Value;
			}
			set
			{
				if (value == nedAdjustSaturation.Value)
					return;

				nedAdjustSaturation.Value = value;
				OnPropertyChanged();
			}
		}

		public string Brightness
		{
			get
			{
				return nedAdjustBrightness.Value;
			}
			set
			{
				if (value == nedAdjustBrightness.Value)
					return;

				nedAdjustBrightness.Value = value;
				OnPropertyChanged();
			}
		}

		private void NedAdjustHue_TextChanged(object sender, RoutedEventArgs e)
		{
			AnyNumEdit_Changed();
			OnPropertyChanged("Hue");
		}

		private void NedAdjustSaturation_TextChanged(object sender, RoutedEventArgs e)
		{
			AnyNumEdit_Changed();
			OnPropertyChanged("Saturation");
		}

		private void NedAdjustBrightness_TextChanged(object sender, RoutedEventArgs e)
		{
			AnyNumEdit_Changed();
			OnPropertyChanged("Brightness");
		}
	}
}
