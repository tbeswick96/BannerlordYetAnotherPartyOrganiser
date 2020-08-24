// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Xml;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs;
using TaleWorlds.Engine;
using YAPO.Global;
using Path = System.IO.Path;

namespace YAPO {
    [PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
    public class PartyTroopSortButtonsExtension : PrefabExtensionInsertPatch {
        public PartyTroopSortButtonsExtension() {
            XmlDocument.Load(XmlPathHelper.GetXmlPath(Id));
        }

        public sealed override string Id => "PartyTroopSortButtons";
        public override int Position => PositionLast;
        private XmlDocument XmlDocument { get; } = new XmlDocument();

        public override XmlDocument GetPrefabExtension() => XmlDocument;
    }

    [PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
    public class OtherTroopSortButtonsExtension : PrefabExtensionInsertPatch {
        public OtherTroopSortButtonsExtension() {
            XmlDocument.Load(XmlPathHelper.GetXmlPath(Id));
        }

        public sealed override string Id => "OtherTroopSortButtons";
        public override int Position => PositionLast;
        private XmlDocument XmlDocument { get; } = new XmlDocument();

        public override XmlDocument GetPrefabExtension() => XmlDocument;
    }

    [PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
    public class TroopActionButtonsExtension : PrefabExtensionInsertPatch {
        public TroopActionButtonsExtension() {
            XmlDocument.Load(XmlPathHelper.GetXmlPath(Id));
        }

        public sealed override string Id => "TroopActionButtons";
        public override int Position => PositionLast;
        private XmlDocument XmlDocument { get; } = new XmlDocument();

        public override XmlDocument GetPrefabExtension() => XmlDocument;
    }

    internal static class XmlPathHelper {
        public static string GetXmlPath(string id) => Path.Combine(Utilities.GetBasePath(), "Modules", Strings.MODULE_FOLDER_NAME, "GUI", "PrefabExtensions", $"{id}.xml");
    }
}
