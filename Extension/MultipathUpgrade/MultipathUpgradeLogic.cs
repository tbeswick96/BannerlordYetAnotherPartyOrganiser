using System;
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
        public static bool TryGetUpgradePaths(PartyCharacterVM troops,
            out List<(int, PartyScreenLogic.PartyCommand.UpgradeTargetType)> upgradeTargets)
        {
            int upgradeIndexByBandit = GetUpgradePathIndexByIsBandit(troops);

            if (upgradeIndexByBandit > -1)
            {
                upgradeTargets = new List<(int, PartyScreenLogic.PartyCommand.UpgradeTargetType)>
                {
                    (troops.NumOfUpgradeableTroops, (PartyScreenLogic.PartyCommand.UpgradeTargetType) upgradeIndexByBandit)
                };
                return true;
            }

            int upgradeIndexByCultureStrength = GetUpgradePathByCultureStrength(troops);

            if (upgradeIndexByCultureStrength > -1)
            {
                upgradeTargets = new List<(int, PartyScreenLogic.PartyCommand.UpgradeTargetType)>
                {
                    (troops.NumOfUpgradeableTroops, (PartyScreenLogic.PartyCommand.UpgradeTargetType) upgradeIndexByCultureStrength)
                };
                return true;
            }

            if (YapoSettings.Instance.SplitUpgrades)
            {
                var targets = GetAllUpgradePaths(troops).ToList();
                upgradeTargets = DetermineMaxCountOfUpgradesPerTarget(troops, targets).ToList();
                return true;
            }

            upgradeTargets = new List<(int, PartyScreenLogic.PartyCommand.UpgradeTargetType)> {(0, PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget6)};
            return false;
        }

        private static IEnumerable<(int, PartyScreenLogic.PartyCommand.UpgradeTargetType)> DetermineMaxCountOfUpgradesPerTarget(
            PartyCharacterVM troops, List<PartyScreenLogic.PartyCommand.UpgradeTargetType> upgradeTargets)
        {
            int upgradesPerTarget = troops.NumOfUpgradeableTroops / upgradeTargets.Count;
            int leftoverUpgrades = troops.NumOfUpgradeableTroops % upgradeTargets.Count;

            upgradeTargets.Shuffle();

            foreach (var target in upgradeTargets)
            {
                int maxCountOfUpgrades = upgradesPerTarget;
                if (leftoverUpgrades > 0)
                {
                    maxCountOfUpgrades++;
                    leftoverUpgrades--;
                }

                yield return (maxCountOfUpgrades, target);
            }
        }

        private static IEnumerable<PartyScreenLogic.PartyCommand.UpgradeTargetType> GetAllUpgradePaths(PartyCharacterVM troops)
        {
            for (int i = 0; i < troops.Character.UpgradeTargets.Length; i++)
            {
                yield return (PartyScreenLogic.PartyCommand.UpgradeTargetType) i;
            }
        }

        private static int GetUpgradePathIndexByIsBandit(PartyCharacterVM troops)
        {
            return troops.Character.UpgradeTargets.Count(o => o.Culture.IsBandit) == 1
                ? troops.Character.UpgradeTargets.FindIndex(o => o.Culture.IsBandit)
                : -1;
        }

        private static int GetUpgradePathByCultureStrength(PartyCharacterVM troops)
        {
            List<UpgradeCandidate> candidates = troops.Character.UpgradeTargets.Select((x, index) =>
                new UpgradeCandidate
                {
                    UpgradeTargetIndex = index,
                    UpgradeClassTipsWhichAreSpecialties = UpgradeTreeCrawler.GetUpgradeTreeTips(x)
                        .Where(y => GetPreferredClassTypesByCulture(troops.Character.Culture.ToString())
                            .Any(z => z == y.ClassType)).ToList()
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

            List<CharacterClassType> firstNotSecond = candidates.First().UpgradeClassTipsWhichAreSpecialties
                .Select(c => c.ClassType)
                .Except(candidates.Last().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType)).ToList();
            List<CharacterClassType> secondNotFirst = candidates.Last().UpgradeClassTipsWhichAreSpecialties
                .Select(c => c.ClassType)
                .Except(candidates.First().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType)).ToList();

            //if all candidates have different specialties in their tips ==> no upgrade
            if (firstNotSecond.Any() || secondNotFirst.Any()) return -1;

            //if all candidates have the same specialties in their tips ==> Candidates will be tested by equipmentProperty
            if (firstNotSecond.Any() || secondNotFirst.Any()) return -1;

            if (!HasPreferredWeaponLoadout()) return -1;

            UpgradeCandidate candidate;
            if (YapoSettings.Instance.PreferShield && candidates.All(x =>
                    x.UpgradeClassTipsWhichAreSpecialties.All(c => c.ClassType == CharacterClassType.INFANTRY)))
            {
                candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_SHIELD);
                if (candidate != null)
                {
                    return candidate.UpgradeTargetIndex;
                }
            }

            if (YapoSettings.Instance.RangedPreference == (int) RangedPreference.CROSSBOWS && candidates.All(x =>
                    x.UpgradeClassTipsWhichAreSpecialties.All(c => c.ClassType == CharacterClassType.RANGED)))
            {
                candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_CROSS_BOW);
                if (candidate != null)
                {
                    return candidate.UpgradeTargetIndex;
                }
            }

            if (YapoSettings.Instance.RangedPreference != (int) RangedPreference.BOWS || !candidates.All(x =>
                    x.UpgradeClassTipsWhichAreSpecialties.All(c => c.ClassType == CharacterClassType.RANGED)))
                return -1;

            candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_BOW);
            if (candidate != null)
            {
                return candidate.UpgradeTargetIndex;
            }


            return -1;
        }

        private static IEnumerable<CharacterClassType> GetPreferredClassTypesByCulture(string cultureName)
        {
            return YapoSettings.Instance.PreferredTroopsByCulture
                       .FirstOrDefault(culture => culture.CultureIdentifier == cultureName)?.TroopClasses ??
                   new List<CharacterClassType>();
        }

        private static bool HasPreferredWeaponLoadout() =>
            YapoSettings.Instance.RangedPreference != (int) RangedPreference.NONE || YapoSettings.Instance.PreferShield;

        private static UpgradeCandidate GetCandidateWithWeaponType(IEnumerable<UpgradeCandidate> candidates,
            EquipmentProperties equipmentProperty)
        {
            List<UpgradeCandidate> superCandidates = candidates.SelectMany(x => x.UpgradeClassTipsWhichAreSpecialties,
                    (candidate, upgradeCharacter) => new {candidate, upgradeCharacter})
                .Where(x => (x.upgradeCharacter.EquipmentProperties & equipmentProperty) != 0).Select(x => x.candidate)
                .ToList();

            return superCandidates.Count == 1 ? superCandidates[0] : null;
        }

        private static Random rng = new Random();
        private static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
