using BotCore;
using NAudio.Midi;
using Microsoft.AspNetCore.SignalR;
using OverlayManager.Hubs;
using System;
using SheetsPersist;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

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

        [Column]
        public string Aliases { get; set; }

        [Column]
        public string Shortcut { get; set; }

        [Column]
        public string MidiNote { get; set; }

        [Column]
        public string TargetApplication { get; set; }

        [Column]
        public bool CommandBecomesArg { get => commandBecomesArgs; set => commandBecomesArgs = value; }

        [Column("ChatBack")]
        // TODO: Rename ChatBack2 to ChatBack
        public string ChatBack2 { get => chatBackMessage; set => chatBackMessage = value; }

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
            if (shortcuts.Length > 0)
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

        int GetOutputDeviceNumber(string deviceName)
        {
            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
                if (deviceName == MidiOut.DeviceInfo(device).ProductName)
                    return device;
            return -1;
        }

        void CreateMidiOut()
        {
            int deviceNumber = GetOutputDeviceNumber("RoryMidi");
            if (deviceNumber < 0)  // Did not find the device.
                return;

            midiOut = new MidiOut(deviceNumber);
        }

        int GetNote(string midiNote)
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

        void SendMidi(string midiNote)
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
            {
                ExecuteChatCommand(hub, chatMessage, args, showsWatched, targetCommand);
            }

            if (chatBackMessage != null)
                Twitch.Chat(Twitch.CodeRushedClient, chatBackMessage);
        }

        private void ExecuteChatCommand(IHubContext<CodeRushedHub, IOverlayCommands> hub, ChatMessage chatMessage, string args, int showsWatched, string targetCommand)
        {
            UserInfo userInfo = UserInfo.FromChatMessage(chatMessage, showsWatched);

            if (string.IsNullOrWhiteSpace(MarkFliesCommand))
                hub.Clients.All.ExecuteCommand(targetCommand, args, userInfo.userId, userInfo.userName, userInfo.displayName, userInfo.color, userInfo.showsWatched);
            else
            {
                if (!string.IsNullOrWhiteSpace(args))
                    MarkFliesData = args;
                hub.Clients.All.ControlSpaceship(MarkFliesCommand, MarkFliesData, userInfo.userId, userInfo.userName, userInfo.displayName, userInfo.color, userInfo.showsWatched);
            }
        }

        public static void RegisterSheet()
        {
            GoogleSheets.RegisterDocumentID("Mr. Announcer Guy", "1s-j-4EF3KbI8ZH0nSj4G4a1ApNFPz_W5DK9A9JTyb3g");
        }
    }
}
