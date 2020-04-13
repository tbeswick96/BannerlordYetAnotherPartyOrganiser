using UIExtenderLib;
using UIExtenderLib.Prefab;

namespace TroopManager {
    [PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
    public class PartyTroopSortButtonsExtension : PrefabExtensionInsertPatch {
        public override int Position => PositionLast;
        public override string Name => "PartyTroopSortButtons";
    }
    
    [PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
    public class OtherTroopSortButtonsExtension : PrefabExtensionInsertPatch {
        public override int Position => PositionLast;
        public override string Name => "OtherTroopSortButtons";
    }
}
