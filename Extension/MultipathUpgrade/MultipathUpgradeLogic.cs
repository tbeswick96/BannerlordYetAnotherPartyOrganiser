using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Library;
using YAPO.MultipathUpgrade.Enum;
using YAPO.MultipathUpgrade.Model;
using YAPO.MultipathUpgrade.Services;

namespace YAPO.MultipathUpgrade
{
    public static class MultipathUpgradeLogic
    {
        public static bool TryGetUpgradePath(PartyCharacterVM troops, out PartyScreenLogic.PartyCommand.UpgradeTargetType upgradeTarget)
        {
            int upgradeIndexByBandit = GetUpgradePathIndexByIsBandit(troops);

            if (upgradeIndexByBandit > -1)
            {
                upgradeTarget = (PartyScreenLogic.PartyCommand.UpgradeTargetType) upgradeIndexByBandit;
                return true;
            }

            int upgradeIndexByCultureStrength = GetUpgradePathByCultureStrength(troops);

            if (upgradeIndexByCultureStrength > -1)
            {
                upgradeTarget = (PartyScreenLogic.PartyCommand.UpgradeTargetType) upgradeIndexByCultureStrength;
                return true;
            }

            upgradeTarget = PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget6;
            return false;
        }

        private static int GetUpgradePathIndexByIsBandit(PartyCharacterVM troops)
        {
            return troops.Character.UpgradeTargets.Count(o => o.Culture.IsBandit) == 1 ? troops.Character.UpgradeTargets.FindIndex(o => o.Culture.IsBandit) : -1;
        }

        private static int GetUpgradePathByCultureStrength(PartyCharacterVM troops)
        {
            List<UpgradeCandidate> candidates = troops.Character.UpgradeTargets.Select((x, index) => new UpgradeCandidate
            {
                UpgradeTargetIndex = index,
                UpgradeClassTipsWhichAreSpecialties = UpgradeTreeCrawler.GetUpgradeTreeTips(x)
                                                                        .Where(y => GetPreferredClassTypesByCulture(troops.Character.Culture.ToString()).Any(z => z == y.ClassType)).ToList()
            }).ToList();

            //if no candidate has a specialty in its tips ==> No upgrade
            if (candidates.All(x => x.UpgradeClassTipsWhichAreSpecialties.Count == 0))
            {
                return -1;
            }

            //if one candidate has any specialty in its tips ==> candidate gets the upgrade
            if (candidates.Count(x => x.UpgradeClassTipsWhichAreSpecialties.Count > 0) == 1)
            {
                return candidates.First(c => c.UpgradeClassTipsWhichAreSpecialties.Count > 0).UpgradeTargetIndex;
            }

            List<CharacterClassType> firstNotSecond = candidates.First().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType)
                                                                .Except(candidates.Last().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType)).ToList();
            List<CharacterClassType> secondNotFirst = candidates.Last().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType)
                                                                .Except(candidates.First().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType)).ToList();

            //if all candidates have different specialties in their tips ==> no upgrade
            if (firstNotSecond.Any() || secondNotFirst.Any()) return -1;

            //if all candidates have the same specialties in their tips ==> Candidates will be tested by equipmentProperty
            if (firstNotSecond.Any() || secondNotFirst.Any()) return -1;

            if (!HasPreferredWeaponLoadout()) return -1;

            UpgradeCandidate candidate;
            if (Settings.Instance.PreferShield && candidates.All(x => x.UpgradeClassTipsWhichAreSpecialties.All(c => c.ClassType == CharacterClassType.INFANTRY)))
            {
                candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_SHIELD);
                if (candidate != null)
                {
                    return candidate.UpgradeTargetIndex;
                }
            }

            if (Settings.Instance.RangedPreference == (int) RangedPreference.CROSSBOWS && candidates.All(x => x.UpgradeClassTipsWhichAreSpecialties.All(c => c.ClassType == CharacterClassType.RANGED)))
            {
                candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_CROSS_BOW);
                if (candidate != null)
                {
                    return candidate.UpgradeTargetIndex;
                }
            }

            if (Settings.Instance.RangedPreference != (int) RangedPreference.BOWS || !candidates.All(x => x.UpgradeClassTipsWhichAreSpecialties.All(c => c.ClassType == CharacterClassType.RANGED))) return -1;

            candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_BOW);
            if (candidate != null)
            {
                return candidate.UpgradeTargetIndex;
            }


            return -1;
        }

        private static IEnumerable<CharacterClassType> GetPreferredClassTypesByCulture(string cultureName)
        {
            return Settings.Instance.PreferredTroopsByCulture.FirstOrDefault(culture => culture.CultureIdentifier == cultureName)?.TroopClasses ?? new List<CharacterClassType>();
        }

        private static bool HasPreferredWeaponLoadout() => Settings.Instance.RangedPreference != (int) RangedPreference.NONE || Settings.Instance.PreferShield;

        private static UpgradeCandidate GetCandidateWithWeaponType(IEnumerable<UpgradeCandidate> candidates, EquipmentProperties equipmentProperty)
        {
            List<UpgradeCandidate> superCandidates = candidates.SelectMany(x => x.UpgradeClassTipsWhichAreSpecialties, (candidate, upgradeCharacter) => new {candidate, upgradeCharacter})
                                                               .Where(x => (x.upgradeCharacter.EquipmentProperties & equipmentProperty) != 0).Select(x => x.candidate).ToList();

            return superCandidates.Count == 1 ? superCandidates[0] : null;
        }
    }
}
