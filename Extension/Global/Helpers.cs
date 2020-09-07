using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using TaleWorlds.Core;

namespace YAPO.Global {
    public static class Helpers {
        public static string AsString(this SortMode sortByOption) {
            switch (sortByOption) {
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

        public static string AsString(this TypeSortOption typeSortOption) {
            switch (typeSortOption) {
                // TODO: Add more options
                case TypeSortOption.CAVALRY: return "Cavalry";
                case TypeSortOption.RANGED_CAVALRY: return "Ranged Cavalry";
                case TypeSortOption.INFANTRY: return "Infantry";
                case TypeSortOption.RANGED: return "Archers";
                default: throw new ArgumentOutOfRangeException(nameof(typeSortOption), typeSortOption, null);
            }
        }

        [Conditional("DEBUG")]
        public static void DebugMessage(string message) {
            Message(message);
        }

        public static void Message(string message) {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }

        // From Modlib---
        public static void ShowError(string message, string title = "", Exception exception = null) {
            if (string.IsNullOrWhiteSpace(title)) {
                title = "";
            }

            MessageBox.Show(message + "\n\n" + exception?.ToStringFull(), title);
        }

        private static string ToStringFull(this Exception ex) => ex != null ? GetString(ex) : "";

        private static string GetString(Exception ex) {
            StringBuilder sb = new StringBuilder();
            GetStringRecursive(ex, sb);
            sb.AppendLine();
            sb.AppendLine("Stack trace:");
            sb.AppendLine(ex.StackTrace);
            return sb.ToString();
        }

        private static void GetStringRecursive(Exception ex, StringBuilder sb) {
            while (true) {
                sb.AppendLine(ex.GetType().Name + ":");
                sb.AppendLine(ex.Message);
                if (ex.InnerException == null) {
                    return;
                }

                sb.AppendLine();
                ex = ex.InnerException;
            }
        }
        // --------------
    }
}
