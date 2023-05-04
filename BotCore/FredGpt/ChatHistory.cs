using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace BotCore
{
    public class ChatHistory
    {
        private const int MaxHistory = 10;
        StringBuilder historyText = new StringBuilder();
        public Queue<ChatPair> ChatPairs { get; set; }
        public string InitialPrompt { get; set; }
        bool isDirty;
        bool fredHasResponded;
        bool waitingForFredToRespond;
        DateTime? waitTimeStart;

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
            waitingForFredToRespond = false;
            isDirty = true;
            ChatPair lastEntry = ChatPairs.Last();
            lastEntry.FredResponse += completion;
        }

        public void AddHumanPart(string humanPrompt)
        {
            waitingForFredToRespond = true;
            isDirty = true;
            while (ChatPairs.Count >= MaxHistory)
                ChatPairs.Dequeue();

            ChatPairs.Enqueue(new ChatPair(humanPrompt));
        }

        public void AddCompletionPrompt(string completionPrompt)
        {
            waitingForFredToRespond = true;
            if (waitTimeStart == null)
                waitTimeStart = DateTime.UtcNow;
            isDirty = true;
            ChatPair lastEntry = ChatPairs.Last();
            lastEntry.FredResponse = completionPrompt;
        }

        public void Reset()
        {
            if (ChatPairs.Count > 0)
                ChatPairs = new Queue<ChatPair>(ChatPairs.Take(ChatPairs.Count - 1));
        }

        public bool WaitingForFredToRespond => waitingForFredToRespond;

        public double SecondsSinceWeStartedWaiting
        {
            get
            {
                if (waitTimeStart.HasValue)
                    return (DateTime.UtcNow - waitTimeStart.Value).TotalSeconds;
                return double.MaxValue;
            }
        }
        
    }
}
