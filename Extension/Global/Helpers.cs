using System;
using System.Diagnostics;
using TaleWorlds.Core;

namespace YAPO.Global
{
    public static class Helpers
    {
        [Conditional("DEBUG")]
        public static void DebugMessage(string message)
        {
            Message(message);
        }

        public static void Message(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }

        public static string AsString(this SortMode sortByOption)
        {
            switch (sortByOption)
            {
                case SortMode.ALPHABETICAL: return "Name";
                case SortMode.TYPE: return "Type";
                case SortMode.GROUP: return "Group";
                case SortMode.TIER: return "Tier";
                case SortMode.CULTURE: return "Culture";
                case SortMode.NONE: return "Nothing";
                case SortMode.COUNT: return "Count";
                default: throw new ArgumentOutOfRangeException(nameof(sortByOption), sortByOption, null);
            }
        }
    }
}
