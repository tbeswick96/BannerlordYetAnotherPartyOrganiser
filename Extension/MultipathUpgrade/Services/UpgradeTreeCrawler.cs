using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using YAPO.MultipathUpgrade.Model;

namespace YAPO.MultipathUpgrade.Services
{
    public static class UpgradeTreeCrawler
    {
        private static List<CharacterClass> _characterClasses;

        public static List<CharacterClass> GetUpgradeTreeTips(CharacterObject characterObject)
        {
            _characterClasses = new List<CharacterClass>();
            ProcessTree(characterObject);
            return _characterClasses;
        }

        private static void ProcessTree(CharacterObject characterObject)
        {
            if (characterObject.UpgradeTargets != null)
            {
                foreach (CharacterObject upgradeTarget in characterObject.UpgradeTargets)
                {
                    if (upgradeTarget.UpgradeTargets != null && upgradeTarget.UpgradeTargets.Any())
                    {
                        ProcessTree(upgradeTarget);
                        continue;
                    }

                    _characterClasses.Add(CharacterClassCreator.CreateCharacterClass(upgradeTarget));
                }
            }

            if (_characterClasses.IsEmpty())
            {
                _characterClasses.Add(CharacterClassCreator.CreateCharacterClass(characterObject));
            }
        }
    }
}
