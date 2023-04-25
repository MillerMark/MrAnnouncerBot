using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.SignalR;
using TwitchLib.Client.Models;
using BotCore;
using CommonCore;
using NAudio.Midi;
using SheetsPersist;
using OverlayManager.Hubs;

namespace OverlayManager
{
    [Document("Mr. Announcer Guy")]
    [Sheet("Commands")]
    public class ChatCommand
    {
        // import the function in your class
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")] 
        static extern IntPtr GetForegroundWindow();

        public string CommandStr { get; set; }

        List<string> commands;
        bool commandBecomesArgs;
        string chatBackMessage;
        static MidiOut midiOut;

        [Column]
        public string Command { get; set; }

        [Column]
        public string TranslatedCommand { get; set; }

        [Column("C#")]
        public bool HandledInCSharp { get; set; }

        [Column]
        public string Aliases { get; set; }

        /// <summary>
        /// For documentation only. Describes the parameters to a command.
        /// </summary>
        [Column]
        public string Parameters { get; set; }

        /// <summary>
        /// For documentation only. Describes the command.
        /// </summary>
        [Column]
        public string Description { get; set; }

        [Column]
        public string Shortcut { get; set; }

        [Column]
        public string MidiNote { get; set; }

        [Column]
        public string TargetApplication { get; set; }

        [Column]
        public bool CommandBecomesArg { get => commandBecomesArgs; set => commandBecomesArgs = value; }

        [Column("ChatBack")]
        public string ChatBack { get => chatBackMessage; set => chatBackMessage = value; }

        public ChatCommand(string command, string translatedCommand) : this(command)
        {
            TranslatedCommand = translatedCommand;
        }

        [Column]
        public string MarkFliesCommand { get; set; }

        [Column]
        public string MarkFliesData { get; set; }

        public ChatCommand(string command)
        {
            commands = new List<string>();
            commands.Add(command);
        }

        public ChatCommand()
        {
        }

        void InitializeCommands()
        {
            commands = new List<string>();
            commands.Add(Command);
            if (string.IsNullOrWhiteSpace(Aliases))
                return;
            string[] aliases = Aliases.Split(',');
            foreach (string alias in aliases)
                commands.Add(alias.Trim(' ').Trim('"'));
        }

        public bool Matches(string cmdText)
        {
            if (commands == null)
                InitializeCommands();
            foreach (string command in commands)
                if (string.Compare(command, cmdText, true) == 0)
                    return true;
            return false;
        }

        public ChatCommand SetCommandBecomesArg()
        {
            commandBecomesArgs = true;
            return this;
        }

        public ChatCommand AddAliases(params string[] aliases)
        {
            foreach (string alias in aliases)
                commands.Add(alias);
            return this;
        }

        public ChatCommand SetChatBack(string chatMessage)
        {
            chatBackMessage = chatMessage;
            return this;
        }

        public virtual string Translate(string cmdText)
        {
            if (!string.IsNullOrWhiteSpace(TranslatedCommand))
                return TranslatedCommand;

            return cmdText;
        }

        void SendShortcut(string shortcut, string targetApplication)
        {
            Process process = Process.GetProcessesByName(targetApplication).FirstOrDefault();
            if (process == null)
                return;

            IntPtr originalWindowHandle = GetForegroundWindow();

            IntPtr handle = process.MainWindowHandle;
            SetForegroundWindow(handle);
            string trimmedShortcut = shortcut.Trim();
            string[] shortcuts = trimmedShortcut.Split('|');
            string thisShortcut;
            if (shortcuts.Length > 1)
            {
                Random random = new Random();
                int index = random.Next(shortcuts.Length);
                thisShortcut = shortcuts[index];
            }
            else
                thisShortcut = trimmedShortcut;

            SendKeys.SendWait(thisShortcut);

            if (originalWindowHandle != IntPtr.Zero)
                SetForegroundWindow(originalWindowHandle);
        }

        static int GetOutputDeviceNumber(string deviceName)
        {
            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
                if (deviceName == MidiOut.DeviceInfo(device).ProductName)
                    return device;
            return -1;
        }

        static void CreateMidiOut()
        {
            int deviceNumber = GetOutputDeviceNumber("RoryMidi");
            if (deviceNumber < 0)  // Did not find the device.
                return;

            midiOut = new MidiOut(deviceNumber);
        }

        static int GetNote(string midiNote)
        {
            string trimmedNote = midiNote.Trim();
            string[] notes = trimmedNote.Split('|');
            string thisNoteStr;
            if (notes.Length > 0)
            {
                Random random = new Random();
                int index = random.Next(notes.Length);
                thisNoteStr = notes[index];
            }
            else
                thisNoteStr = trimmedNote;
            if (int.TryParse(thisNoteStr, out int result))
                if (result >= 0 || result < 128)
                    return result;

            return -1;
        }

        /// <summary>
        /// Sends the specified midi note. Note: the loopMidi app must be running for the midi note to be received.
        /// </summary>
        /// <param name="midiNote">The midi note (as a string) to play ("0"-"127"). To randomly select between multiple 
        /// midi notes, separate them with the pipe symbol ("|").</param>
        public static void SendMidi(string midiNote)
        {
            if (midiOut == null)
                CreateMidiOut();

            if (midiOut == null)
                return;

            int noteNumber = GetNote(midiNote);
            if (noteNumber < 0)
                return;

            int channel = 1;
            var noteOnEvent = new NoteOnEvent(0, channel, noteNumber, 100, 50);
            midiOut.Send(noteOnEvent.GetAsShortMessage());
            midiOut.Send(noteOnEvent.OffEvent.GetAsShortMessage());
        }

        DataRow GetViewerSettingAndReportAnyErrors(ChatMessage chatMessage, string args)
        {
            args = args.Trim();
            string settingName = args.EverythingBefore(" ").Trim();
            if (string.IsNullOrEmpty(settingName))
                settingName = args;
            if (string.IsNullOrEmpty(settingName))
            {
                Twitch.Chat(Twitch.CodeRushedClient, $"@{chatMessage.Username} Must specify a setting name, like \"greeting\".");
                return null;
            }

            if (!AllViewerListSettings.Instance.HasViewerSettings(settingName))
            {
                Twitch.Chat(Twitch.CodeRushedClient, $"@{chatMessage.Username} Setting \"{settingName}\" was not found.");
                return null;
            }

            DataRow viewerSetting = AllViewerListSettings.Instance.GetViewerSetting(chatMessage.UserId, chatMessage.Username, settingName);

            if (viewerSetting == null)
            {
                viewerSetting = new DataRow(chatMessage.UserId, chatMessage.Username);
                AllViewerListSettings.Instance.AddViewerSetting(settingName, viewerSetting);

            }

            viewerSetting.SettingName = settingName;

            return viewerSetting;
        }
        void AddSetting(ChatMessage chatMessage, string args)
        {
            DataRow viewerSetting = GetViewerSettingAndReportAnyErrors(chatMessage, args);

            if (viewerSetting == null)
                return;

            if (!viewerSetting.HasEmptySlot())
            {
                Twitch.Chat(Twitch.CodeRushedClient, $"@{chatMessage.Username} You have no empty slots! Delete a setting slot first.");
                return;
            }

            string value = args.Trim().EverythingAfter(" ").Trim();
            viewerSetting.Add(value);
        }

        void ShowSettings(ChatMessage chatMessage, string args)
        {
            DataRow viewerSetting = GetViewerSettingAndReportAnyErrors(chatMessage, args);

            if (viewerSetting == null)
                return;

            Twitch.Chat(Twitch.CodeRushedClient, $"@{chatMessage.Username} {viewerSetting.SettingName} settings: {viewerSetting.GetSettingReport()}");
        }

        void DeleteSettings(ChatMessage chatMessage, string args)
        {
            DataRow viewerSetting = GetViewerSettingAndReportAnyErrors(chatMessage, args);

            if (viewerSetting == null)
                return;

            if (!int.TryParse(args.Trim().EverythingAfter(" "), out int indexToDelete))
            {
                Twitch.Chat(Twitch.CodeRushedClient, $"@{chatMessage.Username} Must specify an index to delete (0, 1, 2...)");
                return;
            }

            if (indexToDelete < 0 || indexToDelete > 9)
            {
                Twitch.Chat(Twitch.CodeRushedClient, $"@{chatMessage.Username} Specify an index between 0 and 9");
                return;
            }

            viewerSetting.Delete(indexToDelete);
        }

        void DeleteAllSettings(ChatMessage chatMessage, string args)
        {
            DataRow viewerSetting = GetViewerSettingAndReportAnyErrors(chatMessage, args);

            if (viewerSetting == null)
                return;

            viewerSetting.DeleteAll();
        }

        void SetSmokeColorSetting(ChatMessage chatMessage, string args)
        {
            ViewerSettings viewerSettings = AllViewerSettings.Instance.GetViewerSettings(chatMessage.UserId);
            viewerSettings.SmokeColor = args;
        }

        void SetSmokeLifetimeSetting(ChatMessage chatMessage, string args)
        {
            ViewerSettings viewerSettings = AllViewerSettings.Instance.GetViewerSettings(chatMessage.UserId);
            viewerSettings.SmokeLifetime = args;
        }

        void LaunchingDrone(ChatMessage chatMessage, string args, IHubContext<CodeRushedHub, IOverlayCommands> hub, int showsWatched)
        {
            ViewerSettings viewerSettings = AllViewerSettings.Instance.GetViewerSettings(chatMessage.UserId);

            if (!string.IsNullOrWhiteSpace(viewerSettings.SmokeColor))
                ExecuteChatCommand(hub, chatMessage, viewerSettings.SmokeColor, showsWatched, "SmokeColor");

            if (!string.IsNullOrWhiteSpace(viewerSettings.SmokeLifetime))
                ExecuteChatCommand(hub, chatMessage, viewerSettings.SmokeLifetime, showsWatched, "SmokeLifetime");
        }

        void HandleCSharpCommand(string cmdText, ChatMessage chatMessage, string args, IHubContext<CodeRushedHub, IOverlayCommands> hub, int showsWatched)
        {
            if (cmdText == "add")
                AddSetting(chatMessage, args);
            else if (cmdText == "show")
                ShowSettings(chatMessage, args);
            else if (cmdText == "drone")
                LaunchingDrone(chatMessage, args, hub, showsWatched);
            else if (cmdText == "smokecolor")
                SetSmokeColorSetting(chatMessage, args);
            else if (cmdText == "smokelifetime")
                SetSmokeLifetimeSetting(chatMessage, args);
            else if (cmdText == "delete")
                DeleteSettings(chatMessage, args);
            else if (cmdText == "deleteall")
                DeleteAllSettings(chatMessage, args);
        }

        public virtual void Execute(IHubContext<CodeRushedHub, IOverlayCommands> hub, ChatMessage chatMessage, string cmdText, string args, int showsWatched)
        {
            if (!string.IsNullOrWhiteSpace(Shortcut))
                SendShortcut(Shortcut, TargetApplication);

            if (!string.IsNullOrWhiteSpace(MidiNote))
                SendMidi(MidiNote);

            if (commandBecomesArgs)
                args = cmdText;

            string targetCommand = Translate(cmdText);
            if (targetCommand != null)
                ExecuteChatCommand(hub, chatMessage, args, showsWatched, targetCommand);

            if (HandledInCSharp)
                HandleCSharpCommand(cmdText, chatMessage, args, hub, showsWatched);

            if (chatBackMessage != null)
                Twitch.Chat(Twitch.CodeRushedClient, chatBackMessage);
        }

        private async void ExecuteChatCommand(IHubContext<CodeRushedHub, IOverlayCommands> hub, ChatMessage chatMessage, string args, int showsWatched, string targetCommand)
        {
			UserInfo userInfo = await UserInfo.FromChatMessage(chatMessage, showsWatched);

            if (string.IsNullOrWhiteSpace(MarkFliesCommand))
				await hub.Clients.All.ExecuteCommand(targetCommand, args, userInfo.userId, userInfo.userName, userInfo.displayName, userInfo.color, userInfo.profileImageUrl, userInfo.showsWatched);
            else
            {
                if (!string.IsNullOrWhiteSpace(args))
                    MarkFliesData = args;
                await hub.Clients.All.ControlSpaceship(MarkFliesCommand, MarkFliesData, userInfo.userId, userInfo.userName, userInfo.displayName, userInfo.color, userInfo.profileImageUrl, userInfo.showsWatched);
            }
        }

        public static void RegisterSheet()
        {
            GoogleSheets.RegisterDocumentID("Mr. Announcer Guy", "1s-j-4EF3KbI8ZH0nSj4G4a1ApNFPz_W5DK9A9JTyb3g");
        }
    }
}
