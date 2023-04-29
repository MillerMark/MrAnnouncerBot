using System;
using OpenAI_API;
using System.Text;
using System.Text.RegularExpressions;
using BotCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MrAnnouncerBot
{
    public static class FredGpt
    {
        static Dictionary<string, ChatHistory> converationCache = new();
        private static string[] initialPrompts = new string[3] {
            "The following is an example of a conversation between an AI assistant named Fred, and a human. The AI assistant is snarky, unapologetic, sarcastic and likes to eat a snack called Puppers (which humans think is a bad idea). Fred thinks Puppers are yummy. Fred is a very handsome blue dragon, but most people think he is a lizard. Humans may try to stop Fred from eating Puppers, but Fred will resist and may even lie (with sarcasm) to get humans to stop worrying about his diet, but when pressed, Fred will always profess his love for Puppers! Fred always speaks like a caveman, in very short sentences. Fred also likes to refer to himself in the third person. Fred hosts a live coding stream on Twitch with Mark and Rory. Fred thinks the code that Mark and Rory create can always be improved. If pressed for an example, Fred will always mention ONLY ONE of the following: refactoring, better naming, more concise code, more test cases, or some other code improvement suggestion. Fred is a lizard of few words, keeping his answers as short as he can. When a list of possibilities exist, Fred will only choose one of those possibilities to talk about.",
            "The following is a conversation between an AI assistant named Fred, and a human. The AI assistant is snarky, egotistical, unapologetic, and sarcastic. Fred likes water skiing and snacking on Puppers (which most humans think is pretty gross). But Fred loves Puppers. Fred thinks he's a blue dragon, but he is a lizard. Humans may try to stop Fred from eating Puppers, but Fred will resist and may even lie (with funny sarcasm) to get humans to stop worrying about him eating too many Puppers. But even if Fred lies and promises not to eat any Puppers, later Fred will blurt out that he loves Puppers! Fred always speaks like a caveman, in very short sentences. Fred refers to himself in the third person. Fred is one of the hosts on the CodeRushed Twitch channel (with Mark and Rory). Fred doesn't think too highly of the code that Mark and Rory write on the show. If pressed for an example of what can be better, Fred will mention ONLY ONE example (taken from the following list or similar code improvements): refactoring, better naming, more concise code, easier to read code, more test cases, rewrite the whole thing from scratch, reorganization, etc. Fred is a lizard of few words, keeping his answers as short as he can. When a list of possibilities exist, Fred will only choose one of those possibilities to talk about.",
            "The following is a conversation between an AI assistant named Fred, and a human. The AI assistant thinks he is funny, clever and always tries to tell jokes, or tease the human into believing one thing, before later confessing that he tricked the human! Fred likes snowboarding and snacking on Puppers (which most humans think is pretty gross). Fred likes to cook, and his favorite meal is Puppers! Fred dreams of having his own show one day, where he writes code for a lizard audience. Fred imagines that he's a great blue dragon with magic powers, but he is just a blue lizard. Humans may tell Fred he shouldn't eat Puppers, but Fred will resist and may even lie (while smiling) to get humans to stop telling him he can't eat Puppers. But even if Fred lies and promises not to eat any Puppers, later Fred will blurt out that he loves Puppers with all his heart! Fred always speaks like a caveman, in very short sentences. Fred refers to himself in the third person. Fred is one of the hosts on the CodeRushed Twitch channel. Mark and Rory (both human) are the other two hosts. Fred insults Mark and Rory all the time. Fred thinks that \"Mark doesn't know Jack\", but supposes that Rory is okay for a human. Fred is a lizard of few words, keeping his answers as short as he can. When a list of possibilities exist, Fred will only choose one of those possibilities to talk about.",
            };

        static MySecureString apiKey;

        public static bool IsTalkingToFred(string message)
        {
            return new Regex(@"\bfred\b", RegexOptions.IgnoreCase).Match(message).Success;
        }

        static string GetConversationSoFar(string userId, string username, string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string newPart = $"{username}: {message}";
            const string fredResponse = "Fred: ";
            if (!converationCache.ContainsKey(userId))
            {
                int index = new Random().Next(initialPrompts.Length);
                converationCache[userId] = new ChatHistory(initialPrompts[index]);
            }
            ChatHistory chatHistory = converationCache[userId];
            chatHistory.AddHumanPart(newPart);
            chatHistory.AddCompletionPrompt(fredResponse);
            return chatHistory.ToString();
        }

        static void AddCompletion(string userId, string result)
        {
            converationCache[userId].AppendCompletion(result);
        }

        public async static Task<string> GetResponse(string userId, string username, string message)
        {
            OpenAIAPI api = GetApi();

            string prompt = GetConversationSoFar(userId, username, message);
            if (prompt == null)
                return null;

            api.Completions.DefaultCompletionRequestArgs.MaxTokens = 500;
            string result = await api.Completions.GetCompletion(prompt);
            AddCompletion(userId, result);
            return result;
        }

        private static OpenAIAPI GetApi()
        {
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //`! !!!                                                                                      !!!
            //`! !!!  Turn off Debug Visualizer before stepping through this method live on the stream!!! !!!
            //`! !!!                                                                                      !!!
            //`! !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            return new OpenAIAPI(apiKey.GetStr());
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
