using System.Diagnostics;
using TaleWorlds.Core;

namespace TroopManager.Global {
    public static class Helpers {
        [Conditional("DEBUG")]
        public static void DebugMessage(string message) {
            Message(message);
        }
        
        public static void Message(string message) {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }
    }
}
