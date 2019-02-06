using System;
using System.Collections.Generic;
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

namespace DHDM
{
	/// <summary>
	/// Interaction logic for EffectBuilder.xaml
	/// </summary>
	public partial class EffectBuilder : UserControl
	{
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
		}

		private void RbEmitter_Checked(object sender, RoutedEventArgs e)
		{
			spTargetOptions.Visibility = Visibility.Visible;
			spEmitterOptions.Visibility = Visibility.Visible;
			spAnimationOptions.Visibility = Visibility.Collapsed;
			spSoundOptions.Visibility = Visibility.Collapsed;
			SettingsChanged();
		}

		private void RbSoundEffect_Checked(object sender, RoutedEventArgs e)
		{
			spTargetOptions.Visibility = Visibility.Collapsed;
			spEmitterOptions.Visibility = Visibility.Collapsed;
			spAnimationOptions.Visibility = Visibility.Collapsed;
			spSoundOptions.Visibility = Visibility.Visible;
			tbxSoundFileName.Focus();
			SettingsChanged();
		}

		void SettingsChanged()
		{

		}
		private void RbActivePlayer_Checked(object sender, RoutedEventArgs e)
		{
			if (spScrollTarget != null)
				spScrollTarget.Visibility = Visibility.Collapsed;
			SettingsChanged();
		}

		private void RbActiveEnemy_Checked(object sender, RoutedEventArgs e)
		{
			spScrollTarget.Visibility = Visibility.Collapsed;
			SettingsChanged();
		}

		private void RbScrollPos_Checked(object sender, RoutedEventArgs e)
		{
			spScrollTarget.Visibility = Visibility.Visible;
			SettingsChanged();
		}

		// TODO: Add an event handler for txtCoordinates mouse down to check RbScreenPos if not checked.
		private void RbScreenPos_Checked(object sender, RoutedEventArgs e)
		{
			spScrollTarget.Visibility = Visibility.Collapsed;
			SettingsChanged();
		}

		private void Any_TextChanged(object sender, TextChangedEventArgs e)
		{
			SettingsChanged();
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
			SettingsChanged();
		}

		private void RbEmitterRound_Checked(object sender, RoutedEventArgs e)
		{
			if (spCircularOptions != null)
				spCircularOptions.Visibility = Visibility.Visible;
			if (spRectangularOptions != null)
				spRectangularOptions.Visibility = Visibility.Collapsed;
			SettingsChanged();
		}

		private void RbEmitterRectangular_Checked(object sender, RoutedEventArgs e)
		{
			if (spCircularOptions != null)
				spCircularOptions.Visibility = Visibility.Collapsed;
			if (spRectangularOptions != null)
				spRectangularOptions.Visibility = Visibility.Visible;
			SettingsChanged();
		}

		private void AnyNumEdit_Changed(object sender, RoutedEventArgs e)
		{
			SettingsChanged();
		}

		private void CkbRenderOldestParticlesLast_Checked(object sender, RoutedEventArgs e)
		{
			SettingsChanged();
		}

		private void CkbRenderOldestParticlesLast_Unchecked(object sender, RoutedEventArgs e)
		{
			SettingsChanged();
		}

		private void AnyTargetValue_Changed(object sender, RoutedEventArgs e)
		{
			SettingsChanged();
		}

		private void NedEmitterGravity_TextChanged(object sender, RoutedEventArgs e)
		{
			// 
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
			if (spParticleGravityCenter == null)
				return;
			string particleGravity = nedParticleGravity.Value.Trim();
			bool noGravity = particleGravity == "0" || particleGravity == string.Empty;

			if (noGravity)
				spParticleGravityCenter.Visibility = Visibility.Collapsed;
			else
				spParticleGravityCenter.Visibility = Visibility.Visible;
		}

		private void RbDenseSmoke_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
		}

		private void RbPoof_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
		}

		private void RbSparkle_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
		}

		private void RbEmbers_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Visible;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
		}

		private void RbBloodSplatter_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Visible;
		}

		private void RbFumes_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
		}

		private void RbStars_Checked(object sender, RoutedEventArgs e)
		{
			if (spEmbersSelector != null)
				spEmbersSelector.Visibility = Visibility.Collapsed;
			if (spBloodSelector != null)
				spBloodSelector.Visibility = Visibility.Collapsed;
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
			Nullable<bool> result = dlg.ShowDialog();

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

		double GetNum(string str)
		{
			if (double.TryParse(str.Trim(), out double result))
				return result;
			return 0;
		}

		Vector ToVector(string text)
		{
			string[] split = text.Split(',');
			if (split.Length >= 2)
				return new Vector(GetNum(split[0]), GetNum(split[1]));
			else
				return Vector.zero;
		}

		VisualEffectTarget GetTarget()
		{
			VisualEffectTarget visualEffectTarget;
			if (rbScreenPos.IsChecked ?? false)
				visualEffectTarget = new VisualEffectTarget(ToVector(txtCoordinates.Text));
			else
				visualEffectTarget = new VisualEffectTarget();

			if (rbActivePlayer.IsChecked ?? false)
				visualEffectTarget.targetType = TargetType.ActivePlayer;
			else if (rbActiveEnemy.IsChecked ?? false)
				visualEffectTarget.targetType = TargetType.ActiveEnemy;
			else if (rbScrollPos.IsChecked ?? false)
				visualEffectTarget.targetType = TargetType.ScrollPosition;
			else
				visualEffectTarget.targetType = TargetType.ScreenPosition;

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
	}
}
