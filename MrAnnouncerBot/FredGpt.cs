using System;
using OpenAI_API;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security;
using BotCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;

namespace MrAnnouncerBot
{
    public static class FredGpt
    {
        static Dictionary<string, StringBuilder> converationCache = new();
        private const string initialPrompt = "The following is an example of a conversation between an AI assistant named Fred, and a human. The AI assistant is snarky, unapologetic, sarcastic and likes to eat a snack called Puppers (which humans think is a bad idea). Fred thinks Puppers are yummy. Humans may try to stop Fred from eating Puppers, but Fred will resist and may even lie (with sarcasm) to get humans to stop worrying about his diet, but when pressed, Fred will always profess his love for Puppers! Fred always speaks like a caveman, in very short sentences. Fred also likes to refer to himself in the third person. Fred hosts a live coding stream on Twitch with Mark and Rory. Fred thinks the code that Mark and Rory create can always be improved. If pressed for an example, Fred will always mention ONLY ONE of the following: refactoring, better naming, more concise code, more test cases, or some other code improvement suggestion.";
        static MySecureString apiKey;
        public static bool IsTalkingToFred(string message)
        {
            return new Regex(@"\bfred\b", RegexOptions.IgnoreCase).Match(message).Success;
        }

        static string GetConversationSoFar(string userId, string username, string message)
        {
            string newPart = $"{username}: {message}";
            const string fredResponse = "Fred: ";
            if (!converationCache.ContainsKey(userId))
            {
                converationCache[userId] = new StringBuilder();
                converationCache[userId].AppendLine(initialPrompt);
            }
            converationCache[userId].AppendLine();
            converationCache[userId].AppendLine(newPart);
            converationCache[userId].AppendLine(fredResponse);
            return converationCache[userId].ToString();
        }

        static void AddResponse(string userId, string result)
        {
            converationCache[userId].AppendLine(result);
        }

        public async static Task<string> GetResponse(string userId, string username, string message)
        {
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //`! !!!                                                                                      !!!
            //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
            //`! !!!                                                                                      !!!
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var api = new OpenAI_API.OpenAIAPI(apiKey.GetStr());

            string prompt = GetConversationSoFar(userId, username, message);
            if (prompt == null)
                return null;

            string result = await api.Completions.GetCompletion(prompt);
            AddResponse(userId, result);
            return result;
        }


        public static void SetApiKey(MySecureString mySecureString)
        {
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //`! !!!                                                                                      !!!
            //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
            //`! !!!                                                                                      !!!
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            apiKey = mySecureString;
        }

    }
}
