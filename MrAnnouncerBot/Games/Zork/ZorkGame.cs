using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace MrAnnouncerBot.Games.Zork
{
    public class ZorkGame
    {
        private readonly TwitchClient client;
        private readonly string channel;
        private Dictionary<string, Action<OnChatCommandReceivedArgs>> commands;
        private List<ZorkPlayer> players;
        private Queue<string> directMessageQueue;

        private readonly Dictionary<int, ZorkLocation> locations = new Dictionary<int, ZorkLocation>
        {
            { 0, new ZorkLocation {
                    Name = "West of House",
                    ShortDescription = "standing west of a white house.",
                    LongDescription = "standing in an opn field west of a white house, with a boarded front door.",
                    Items = new List<ZorkItem>
                    {
                        new ZorkItem
                        {
                            Name = "small mailbox",
                            IsOpen = true,
                            Contents = new List<ZorkItem>
                            {
                                new ZorkItem
                                {
                                    Name = "A leaflet"
                                }
                            }
                        }
                    }
                }
            }
        };

        public ZorkGame(TwitchClient client, string channel)
        {
            this.client = client;
            this.channel = channel;
            directMessageQueue = new Queue<string>();
            RegisterCommands();
            InitPlayers();
        }

        private void InitPlayers()
        {
            players = new List<ZorkPlayer>();
        }

        private void RegisterCommands()
        {
            commands = new Dictionary<string, Action<OnChatCommandReceivedArgs>>
            {
                {"help", CmdHelp },
                {"look", CmdLook},
                { "l", CmdLook}
            };
        }

        public void HandleCommand(OnChatCommandReceivedArgs e)
        {
            if (players.Any(x => x.UserId == e.Command.ChatMessage.UserId))
            {
                if (e.Command.CommandText == "zork" && e.Command.ArgumentsAsList.Count == 0)
                {
                    SendDirectMessage(GetUserName(e), "You are already playing zork. Type \"!zork [cmd]\" or \"!zork help\" to get a list of commands.");
                }
                else
                {
                    var cmds = e.Command.ArgumentsAsList;
                    if (commands.ContainsKey(cmds[0]))
                    {
                        commands[cmds[0]](e);
                    }
                    else
                    {
                        SendDirectMessage(GetUserName(e), $"I don't know the word \"{cmds[0]}\"");
                    }
                }
            }
            else
            {
                if (e.Command.ArgumentsAsList.Count == 0)
                {
                    players.Add(new ZorkPlayer
                    {
                        UserId = e.Command.ChatMessage.UserId,
                        Location = locations.First().Value,
                        Score = 0,
                        Moves = 1
                    });
                    CmdIntro(e);
                }
                else
                {
                    SendDirectMessage(GetUserName(e), "You are not playing zork. Type !zork to join the game");
                }
            }
        }

        private void CmdIntro(OnChatCommandReceivedArgs e)
        {
            QueueDirectMessage("Welcome to ZORK.");
            CmdLook(e);
        }

        private void CmdLook(OnChatCommandReceivedArgs e)
        {
            var player = GetPlayer(e.Command.ChatMessage.UserId);
            QueueDirectMessage(player.Location.Name);
            QueueDirectMessage($"You are {player.Location.LongDescription}");
            foreach (var item in player.Location.Items)
            {
                QueueDirectMessage($"There is a {item.Name} here.");
                if (item.Contents.Any() && item.IsOpen)
                {
                    QueueDirectMessage($"The {item.Name} contains:");
                    foreach (var content in item.Contents)
                    {
                        QueueDirectMessage($"{content.Name}");
                    }
                }
            }
            SendQueuedMessages(GetUserName(e));
            SendPublicMessage($"{e.Command.ChatMessage.DisplayName} is {player.Location.ShortDescription}");
        }

        private void CmdHelp(OnChatCommandReceivedArgs e)
        {
            QueueDirectMessage("Here are the zork commands you can use while playing the game: ");
            QueueDirectMessage("help       h       This list of commands ");
            QueueDirectMessage("look       l       Looks around at current location ");
            SendQueuedMessages(GetUserName(e));
        }

        private void QueueDirectMessage(string msg)
        {
            directMessageQueue.Enqueue(msg);
        }

        private void SendDirectMessage(string userId, string msg)
        {
            client.SendWhisper(userId, msg);
        }

        private void SendQueuedMessages(string userId)
        {
            var messages = directMessageQueue.ToArray();
            var message = new StringBuilder();
            foreach (var msg in messages)
            {
                message.Append(msg);
            }
            client.SendWhisper(userId, message.ToString());
            directMessageQueue.Clear();
        }

        private string GetUserName(OnChatCommandReceivedArgs e)
        {
            return e.Command.ChatMessage.Username;
        }

        private ZorkPlayer GetPlayer(string userid)
        {
            return players.First(x => x.UserId == userid);
        }

        private void SendPublicMessage(string msg)
        {
            client.SendMessage(channel, msg);
        }
    }
}