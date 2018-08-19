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
        private readonly Queue<string> directMessageQueue;

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
                            Name = "mailbox",
                            Description = "small mailbox",
                            IsOpen = false,
                            IsContainer = true,
                            Contents = new List<ZorkItem>
                            {
                                new ZorkItem
                                {
                                    Name = "leaflet",
                                    Description = "A leaflet"
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
                { "l", CmdLook},
                {"drop", CmdDrop},
                {"get", CmdGet },
                {"take", CmdGet },
                {"open", CmdOpen },
                {"inventory", CmdInventory },
                {"i", CmdInventory }
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

        private void QueueDirectMessage(string msg)
        {
            directMessageQueue.Enqueue(msg);
        }

        private void SendDirectMessage(string userName, string msg)
        {
            client.SendWhisper(userName, msg);
        }

        private void SendQueuedMessages(string userName)
        {
            var messages = directMessageQueue.ToArray();
            var message = new StringBuilder();
            foreach (var msg in messages)
            {
                message.Append(msg);
            }
            client.SendWhisper(userName, message.ToString());
            directMessageQueue.Clear();
        }

        private string GetUserName(OnChatCommandReceivedArgs e)
        {
            return e.Command.ChatMessage.Username;
        }

        private string GetUserId(OnChatCommandReceivedArgs e)
        {
            return e.Command.ChatMessage.UserId;
        }

        private ZorkPlayer GetPlayer(string userid)
        {
            return players.First(x => x.UserId == userid);
        }

        private void SendPublicMessage(string msg)
        {
            client.SendMessage(channel, msg);
        }

        // all zork commands go below

        private void CmdIntro(OnChatCommandReceivedArgs e)
        {
            QueueDirectMessage("Welcome to ZORK.");
            CmdLook(e);
        }

        private void CmdLook(OnChatCommandReceivedArgs e)
        {
            var player = GetPlayer(GetUserId(e));
            QueueDirectMessage(player.Location.Name);
            QueueDirectMessage($"You are {player.Location.LongDescription}");
            foreach (var item in player.Location.Items)
            {
                QueueDirectMessage($"There is a {item.Description} here.");
                if (item.Contents.Any() && item.IsOpen)
                {
                    QueueDirectMessage($"The {item.Name} contains:");
                    foreach (var content in item.Contents)
                    {
                        QueueDirectMessage($"{content.Description}");
                    }
                }
            }
            SendQueuedMessages(GetUserName(e));
            SendPublicMessage($"{e.Command.ChatMessage.DisplayName} is {player.Location.ShortDescription}");
        }

        private void CmdHelp(OnChatCommandReceivedArgs e)
        {
            QueueDirectMessage("Here are the zork commands you can use while playing the game:");
            QueueDirectMessage("help        h       This list of commands");
            QueueDirectMessage("look        l       Looks around at current location");
            QueueDirectMessage("drop                Removes an item from inventory; places it in current room");
            QueueDirectMessage("get/take            Removes an item from current room; places it in your inventory");
            QueueDirectMessage("open                Opens the container, whether it is in the room or your inventory");
            QueueDirectMessage("inventory   i       Show contents of inventory");
            SendQueuedMessages(GetUserName(e));
        }

        private void CmdDrop(OnChatCommandReceivedArgs e)
        {
            var player = GetPlayer(GetUserId(e));
            var args = e.Command.ArgumentsAsList;

            if (args.Count == 1)
            {
                SendDirectMessage(GetUserName(e), "What do you want to drop?");
            }
            else
            {
                var itemToDrop = args[1];
                var itemInInventory = player.Inventory.FirstOrDefault(x => x.Name.Equals(itemToDrop, StringComparison.InvariantCultureIgnoreCase));
                if (itemInInventory == null)
                {
                    SendDirectMessage(GetUserName(e), $"You don't have a {itemToDrop}");
                }
                else
                {
                    player.Inventory.Remove(itemInInventory);
                    player.Location.Items.Add(itemInInventory);
                    SendDirectMessage(GetUserName(e), "Dropped.");
                }
            }
        }

        private void CmdGet(OnChatCommandReceivedArgs e)
        {
            var player = GetPlayer(GetUserId(e));
            var userName = GetUserName(e);
            var args = e.Command.ArgumentsAsList;

            if (args.Count == 1)
            {
                SendDirectMessage(userName, "What do you want to get?");
            }
            else
            {
                // TODO handle get/take all
                var itemToGet = args[1];
                // TODO might be other items that you can't "get" so build a list if that happens
                if (itemToGet.Equals("mailbox", StringComparison.InvariantCultureIgnoreCase))
                {
                    SendDirectMessage(userName, "It is securely anchored.");
                }
                else if (itemToGet.Equals("grue", StringComparison.CurrentCultureIgnoreCase))
                {
                    SendDirectMessage(userName, "You can't be serious.");
                }
                else
                {
                    // TODO check vocab if it's not an item we recognize then say I don't know the word "{itemToGet}".
                    var target = player.Location.Items.FirstOrDefault(x =>
                        x.Name.Equals(itemToGet, StringComparison.InvariantCultureIgnoreCase));
                    if (target == null)
                    {
                        foreach (var locationItem in player.Location.Items)
                        {
                            if (locationItem.IsContainer && locationItem.IsOpen)
                            {
                                target = locationItem.Contents.FirstOrDefault(x =>
                                    x.Name.Equals(itemToGet, StringComparison.CurrentCultureIgnoreCase));
                                if (target == null) continue;
                                player.Inventory.Add(target);
                                player.Location.Items.Remove(target);
                                SendDirectMessage(GetUserName(e), "Taken.");
                                return;
                            }
                        }
                        SendDirectMessage(userName, $"You can't see any {itemToGet} here!");
                    }
                    else
                    {
                        player.Inventory.Add(target);
                        player.Location.Items.Remove(target);
                        SendDirectMessage(GetUserName(e), "Taken.");
                    }
                }
            }
        }

        private void CmdOpen(OnChatCommandReceivedArgs e)
        {
            var player = GetPlayer(GetUserId(e));
            var userName = GetUserName(e);
            var args = e.Command.ArgumentsAsList;

            if (args.Count == 1)
            {
                // TODO open without params does it always assume a door is here?
                SendDirectMessage(userName, "(door) The door cannot be opened.");
            }
            else
            {
                var target = args[1];
                if (target.Equals("grue", StringComparison.CurrentCultureIgnoreCase))
                {
                    SendDirectMessage(userName, "You must tell me how to do that to a lurking grue.");
                }
                else
                {
                    // TODO check vocab if it's not an item we recognize then say I don't know the word "{itemToGet}".
                    // TODO check items in inventory as well to open
                    var itemToOpen = player.Location.Items.FirstOrDefault(x =>
                        x.Name.Equals(target, StringComparison.InvariantCultureIgnoreCase));
                    if (itemToOpen == null)
                    {
                        SendDirectMessage(userName, $"You can't see any {target} here!");
                    }
                    else
                    {
                        if (itemToOpen.IsOpen)
                        {
                            SendDirectMessage(userName, "It is already open.");
                        }
                        else
                        {
                            itemToOpen.IsOpen = true;
                            // TODO hardcoded to be the mailbox for now, each "open" item probably has it's own message
                            SendDirectMessage(userName, "Opening the small mailbox reveals a leaflet.");
                        }
                    }
                }
            }
        }

        private void CmdInventory(OnChatCommandReceivedArgs e)
        {
            var player = GetPlayer(GetUserId(e));
            var userName = GetUserName(e);

            if (player.Inventory.Any())
            {
                QueueDirectMessage("You are carrying:");
                foreach (var item in player.Inventory)
                {
                    QueueDirectMessage(item.Description);
                }
                SendQueuedMessages(userName);
            }
            else
            {
                SendDirectMessage(userName, "You are empty-handed.");
            }
        }
    }
}