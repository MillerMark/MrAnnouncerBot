using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MrAnnouncerBot
{
    public class ChatHistory
    {
        private const int MaxHistory = 10;
        StringBuilder historyText = new StringBuilder();
        public Queue<ChatPair> ChatPairs { get; set; }
        public string InitialPrompt { get; set; }
        bool isDirty;

        public ChatHistory(string initialPrompt)
        {
            InitialPrompt = initialPrompt;
            ChatPairs = new Queue<ChatPair>(MaxHistory);
        }

        void RebuildHistoryText()
        {
            historyText.Clear();
            historyText.AppendLine(InitialPrompt);
            foreach (ChatPair chatPair in ChatPairs)
                historyText.AppendLine(chatPair.ToString());
            isDirty = false;
        }

        public override string ToString()
        {
            if (isDirty)
                RebuildHistoryText();
            return historyText.ToString();
        }

        public void AppendCompletion(string completion)
        {
            isDirty = true;
            ChatPair lastEntry = ChatPairs.Last();
            lastEntry.FredResponse += completion;
        }

        public void AddHumanPart(string humanPrompt)
        {
            isDirty = true;
            while (ChatPairs.Count >= MaxHistory)
                ChatPairs.Dequeue();

            ChatPairs.Enqueue(new ChatPair(humanPrompt));
        }

        public void AddCompletionPrompt(string completionPrompt)
        {
            isDirty = true;
            ChatPair lastEntry = ChatPairs.Last();
            lastEntry.FredResponse = completionPrompt;
        }
    }
}
