using System;
using System.Linq;

namespace BotCore
{
    public class ChatPair
    {
        public string HumanPrompt { get; init; }
        public string FredResponse { get; set; }
        public ChatPair(string humanPrompt)
        {
            HumanPrompt = humanPrompt;
        }

        public override string ToString()
        {
            return HumanPrompt + Environment.NewLine + FredResponse + Environment.NewLine;
        }
    }
}
