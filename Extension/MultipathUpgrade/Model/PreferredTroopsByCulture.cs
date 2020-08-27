using System.Collections.Generic;
using System.Xml.Serialization;
using YAPO.MultipathUpgrade.Enum;

namespace YAPO.MultipathUpgrade.Model {
    public class PreferredTroopsByCulture {
        [XmlElement("TroopClass")] public List<CharacterClassType> TroopClasses = new List<CharacterClassType>();

        [XmlAttribute("Culture")]
        public string CultureIdentifier { get; set; }
    }
}
