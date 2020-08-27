// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Diagnostics;
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
            using (XmlReader reader = XmlReader.Create(XmlPathHelper.GetXmlPath(Id), new XmlReaderSettings {IgnoreComments = true, IgnoreWhitespace = true})) {
                XmlDocument.Load(reader);
            }

            Debug.Assert(XmlDocument.HasChildNodes, $"Failed to parse extension ({Id}) XML!");
        }

        public sealed override string Id => "PartyTroopSortButtons";
        public override int Position => PositionLast;
        private XmlDocument XmlDocument { get; } = new XmlDocument();

        public override XmlDocument GetPrefabExtension() => XmlDocument;
    }

    [PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
    public class OtherTroopSortButtonsExtension : PrefabExtensionInsertPatch {
        public OtherTroopSortButtonsExtension() {
            using (XmlReader reader = XmlReader.Create(XmlPathHelper.GetXmlPath(Id), new XmlReaderSettings {IgnoreComments = true, IgnoreWhitespace = true})) {
                XmlDocument.Load(reader);
            }

            Debug.Assert(XmlDocument.HasChildNodes, $"Failed to parse extension ({Id}) XML!");
        }

        public sealed override string Id => "OtherTroopSortButtons";
        public override int Position => PositionLast;
        private XmlDocument XmlDocument { get; } = new XmlDocument();

        public override XmlDocument GetPrefabExtension() => XmlDocument;
    }

    [PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
    public class TroopActionButtonsExtension : PrefabExtensionInsertPatch {
        public TroopActionButtonsExtension() {
            using (XmlReader reader = XmlReader.Create(XmlPathHelper.GetXmlPath(Id), new XmlReaderSettings {IgnoreComments = true, IgnoreWhitespace = true})) {
                XmlDocument.Load(reader);
            }

            Debug.Assert(XmlDocument.HasChildNodes, $"Failed to parse extension ({Id}) XML!");
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
