using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using BotCore;
using TwitchLib.Client.Events;
using BotCore;

namespace ChatWatch
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DispatcherTimer statusUpdateTimer;

		DateTime timeOfLastChatSoundEffect = DateTime.Now;
		DateTime timeOfLastHighEmphasisChatSoundEffect = DateTime.Now;
		DateTime startTime;

		public MainWindow()
		{
			Twitch.InitializeConnections();
			InitializeComponent();
			statusUpdateTimer = new DispatcherTimer(DispatcherPriority.Send);
			statusUpdateTimer.Tick += new EventHandler(StatusUpdateHandler);
			statusUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
			HookupTwitchEvents();
		}

		static MessageImportance GetMessageImportance(string message)
		{
			if (message == null)
				return MessageImportance.Low;
			if (message.StartsWith("!"))
				return MessageImportance.Low;

			string[] words = message.Split(' ');
			if (message.EndsWith("!"))
				return MessageImportance.High;
			else if (words == null)
				return MessageImportance.Low;
			else if (words.Length > 6)
				return MessageImportance.High;
			else if (words.Length > 3)
				return MessageImportance.Medium;

			return MessageImportance.Low;
		}
		public void Chat(string message)
		{
			Twitch.Chat(message);
		}

		void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			HandleChatMessage(e.ChatMessage.DisplayName, e.ChatMessage.Message);
		}

		void HandleChatMessage(string displayName, string message)
		{
			if (displayName == "MrAnnouncerBot")
				return;
			MessageImportance messageImportance = GetMessageImportance(message);

			if (messageImportance == MessageImportance.Low)
				return;

			ShowChatMessage(displayName, messageImportance, message);
			DateTime now = DateTime.Now;
			double secondsSinceLastChatSoundEffect = (now - timeOfLastChatSoundEffect).TotalSeconds;
			double secondsSinceLastHighEmphasisChatSoundEffect = (now - timeOfLastHighEmphasisChatSoundEffect).TotalSeconds;

			const double minSecondsBetweenChatSoundEffects = 10d;
			const double minSecondsBetweenHighEmphasisChatSoundEffects = 5d;
			if (secondsSinceLastChatSoundEffect > minSecondsBetweenChatSoundEffects)
			{
				timeOfLastChatSoundEffect = now;
			}
			else if (messageImportance == MessageImportance.High && secondsSinceLastHighEmphasisChatSoundEffect > minSecondsBetweenHighEmphasisChatSoundEffects)
			{
				timeOfLastChatSoundEffect = now;
				timeOfLastHighEmphasisChatSoundEffect = now;
			}
			else
				return;

			PlayChatMessageSoundEffect(displayName, messageImportance, message);
		}

		void HookupTwitchEvents()
		{
			//Twitch.Client.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			//Twitch.Client.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
			Twitch.Client.OnMessageReceived += Client_OnMessageReceived;
			//Twitch.Client.OnUserJoined += TwitchClient_OnUserJoined;
			//Twitch.Client.OnUserLeft += TwitchClient_OnUserLeft;
		}

		void PlayChatMessageSoundEffect(string displayName, MessageImportance messageImportance, string message)
		{
			string soundFolder = Environment.CurrentDirectory;

			string fileName;
			int index;

			if (messageImportance == MessageImportance.High)
			{
				index = new Random((int)DateTime.Now.Ticks).Next(58) + 1;
				fileName = $"emphasis ({index}).wav";
				soundFolder += @"\SoundEffects\Emphasis\";
			}
			else
			{
				index = new Random((int)DateTime.Now.Ticks).Next(55) + 1;
				fileName = $"subtle ({index}).wav";
				soundFolder += @"\SoundEffects\Subtle\";
			}
			startTime = DateTime.Now;

			if (displayName == "wil_bennett" || displayName == "SurlyDev" || displayName == "TheHugoDahl" || displayName == "str8buttah" || displayName == "Instafluff")
			{
				fileName = displayName + ".wav";
				soundFolder += @"\SoundEffects\Known\";
			}

			MediaPlayer mediaPlayer = new MediaPlayer();
			mediaPlayer.Open(new Uri($"{soundFolder}{fileName}"));
			mediaPlayer.Play();
			statusUpdateTimer.Start();
		}

		private void ShowChatMessage(string displayName, MessageImportance messageImportance, string message)
		{
			Dispatcher.Invoke(() =>
			{
				if (messageImportance == MessageImportance.High)
				{
					rectMedium.Visibility = Visibility.Hidden;
					rectImportant.Opacity = 1.0;
					rectImportant.Visibility = Visibility.Visible;
				}
				else
				{
					rectMedium.Visibility = Visibility.Visible;
					rectMedium.Opacity = 1.0;
					rectImportant.Visibility = Visibility.Hidden;
				}
				tbStatus.Text = message;
				tbAuthor.Text = displayName;
				startTime = DateTime.Now;
				statusUpdateTimer.Start();
			});
		}

		private const double maxLifespanSeconds = 5;

		void StatusUpdateHandler(object sender, EventArgs e)
		{
			double secondsAlive = (DateTime.Now - startTime).TotalSeconds;
			if (secondsAlive > maxLifespanSeconds)
			{
				rectMedium.Visibility = Visibility.Hidden;
				rectImportant.Visibility = Visibility.Hidden;
				statusUpdateTimer.Stop();
				return;
			}
			double opacity = 1 - secondsAlive / maxLifespanSeconds;
			if (rectMedium.Visibility == Visibility.Visible)
				rectMedium.Opacity = opacity;
			else if (rectImportant.Visibility == Visibility.Visible)
				rectImportant.Opacity = opacity;
		}

		public void Whisper(string userName, string message)
		{
			Twitch.Whisper(userName, message);
		}

		public enum MessageImportance
		{
			Low,
			Medium,
			High
		}

		private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			Window window = (Window)sender;
			window.Topmost = true;
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (WindowState == WindowState.Maximized)
				WindowState = WindowState.Normal;
			else if (WindowState == WindowState.Normal)
				WindowState = WindowState.Maximized;
		}
		
	}
}
