using UIExtenderLib;
using UIExtenderLib.Prefab;

namespace YAPO {
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

    [PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
    public class TroopActionButtonsExtension : PrefabExtensionInsertPatch {
        public override int Position => PositionLast;
        public override string Name => "TroopActionButtons";
    }
}
