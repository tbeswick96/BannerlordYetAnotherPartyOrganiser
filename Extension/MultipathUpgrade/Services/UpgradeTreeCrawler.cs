using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using YAPO.MultipathUpgrade.Model;

namespace YAPO.MultipathUpgrade.Services {
    public static class UpgradeTreeCrawler {
        private static List<CharacterClass> characterClasses;

        public static IEnumerable<CharacterClass> GetUpgradeTreeTips(CharacterObject characterObject) {
            characterClasses = new List<CharacterClass>();
            ProcessTree(characterObject);
            return characterClasses;
        }

        private static void ProcessTree(CharacterObject characterObject) {
            if (characterObject.UpgradeTargets != null) {
                foreach (CharacterObject upgradeTarget in characterObject.UpgradeTargets) {
                    if (upgradeTarget.UpgradeTargets != null && upgradeTarget.UpgradeTargets.Any()) {
                        ProcessTree(upgradeTarget);
                        continue;
                    }

                    characterClasses.Add(CharacterClassCreator.CreateCharacterClass(upgradeTarget));
                }
            }

            if (characterClasses.IsEmpty()) {
                characterClasses.Add(CharacterClassCreator.CreateCharacterClass(characterObject));
            }
        }
    }
}
