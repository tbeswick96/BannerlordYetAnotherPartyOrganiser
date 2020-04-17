using System.Collections.Generic;
using System.Xml.Serialization;
using YAPO.MultipathUpgrade.Enum;

namespace YAPO.MultipathUpgrade.Model
{
    public class PreferredTroopsByCulture
    {
        [XmlAttribute("Culture")]
        public string CultureIdentifier { get; set; }
        [XmlElement("TroopClass")]
        public List<CharacterClassType> TroopClasses = new List<CharacterClassType>();
    }
}
