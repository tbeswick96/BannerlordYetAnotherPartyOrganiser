using System;
using System.Diagnostics;
using TaleWorlds.Core;

namespace YAPO.Global {
    public static class Helpers {
        [Conditional("DEBUG")]
        public static void DebugMessage(string message) {
            Message(message);
        }

        public static void Message(string message) {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }

        public static string AsString(this SortMode sortByOption) {
            return sortByOption switch {
                SortMode.ALPHABETICAL => "Name",
                SortMode.TYPE => "Type",
                SortMode.GROUP => "Group",
                SortMode.TIER => "Tier",
                SortMode.CULTURE => "Culture",
                SortMode.NONE => "Nothing",
                SortMode.COUNT => "Count",
                _ => throw new ArgumentOutOfRangeException(nameof(sortByOption), sortByOption, null)
            };
        }
    }
}
