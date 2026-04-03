using System;
using System.Collections.Generic;

namespace MrAnnouncerBot
{
    public static class ActionParser
    {
        public static IEnumerable<(string Key, string Value)> ParseLines(string eventActions)
        {
            if (string.IsNullOrWhiteSpace(eventActions)) yield break;
            foreach (var line in eventActions.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int colon = line.IndexOf(':');
                if (colon < 0) continue;
                yield return (line.Substring(0, colon).Trim().ToLower(),
                              line.Substring(colon + 1).Trim());
            }
        }
    }
}
