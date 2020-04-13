using System.Diagnostics;
using TaleWorlds.Core;

namespace TroopManager {
    public class Helpers {
        [Conditional("DEBUG")]
        public static void DebugMessage(string message) {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }
    }
}
