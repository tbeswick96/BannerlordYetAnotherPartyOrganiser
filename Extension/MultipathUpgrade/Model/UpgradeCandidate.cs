using System.Collections.Generic;

namespace YAPO.MultipathUpgrade.Model
{
    public class UpgradeCandidate
    {
        public int UpgradeTargetIndex { get; set; }
        public List<CharacterClass> UpgradeClassTipsWhichAreSpecialties { get; set; }
    }
}
