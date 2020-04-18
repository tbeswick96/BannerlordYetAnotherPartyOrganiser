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
        public static bool TryGetUpgradePath(PartyCharacterVM troops, out PartyScreenLogic.PartyCommand.UpgradeTargetType upgradeTarget)
        {
            int upgradeIndexByBandit = GetUpgradePathIndexByIsBandit(troops);

            if (upgradeIndexByBandit > -1)
            {
                upgradeTarget = (PartyScreenLogic.PartyCommand.UpgradeTargetType)upgradeIndexByBandit;
                return true;
            }

            int upgradeIndexByCultureStrength = GetUpgradePathByCultureStrength(troops);

            if (upgradeIndexByCultureStrength > -1)
            {
                upgradeTarget = (PartyScreenLogic.PartyCommand.UpgradeTargetType)upgradeIndexByCultureStrength;
                return true;
            }

            upgradeTarget = PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget6;
            return false;
        }

        public static int GetUpgradePathIndexByIsBandit(PartyCharacterVM troops)
        {
            if (troops.Character.UpgradeTargets.Count(o => o.Culture.IsBandit) == 1)
            {
                return troops.Character.UpgradeTargets.FindIndex(o => o.Culture.IsBandit);
            }

            return -1;
        }

        public static int GetUpgradePathByCultureStrength(PartyCharacterVM troops)
        {
            List<UpgradeCandidate> candidates = new List<UpgradeCandidate>();

            for (int i = 0; i < troops.Character.UpgradeTargets.Length; i++)
            {
                UpgradeCandidate upgradeCandidate = new UpgradeCandidate
                {
                    UpgradeTargetIndex = i,
                    UpgradeClassTipsWhichAreSpecialties = UpgradeTreeCrawler
                        .GetUpgradeTreeTips(troops.Character.UpgradeTargets[i]).Where(cl =>
                            GetPreferredClassTypesByCulture(troops.Character.Culture.GetName().ToString())
                                .Any(c => c == cl.ClassType)).ToList()
                };

                candidates.Add(upgradeCandidate);
            }

            //if no candidate has a specialty in its tips ==> No upgrade
            if (candidates.All(candidate => candidate.UpgradeClassTipsWhichAreSpecialties.Count == 0))
            {
                return -1;
            }
             
            //if one candidate has any specialty in its tips ==> candidate gets the upgrade
            if (candidates.Count(candidate => candidate.UpgradeClassTipsWhichAreSpecialties.Count > 0) == 1)
            {
                return candidates.First(c => c.UpgradeClassTipsWhichAreSpecialties.Count > 0).UpgradeTargetIndex;
            }

            var firstNotSecond = candidates.First().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType)
                .Except(candidates.Last().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType));
            var secondNotFirst = candidates.Last().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType)
                .Except(candidates.First().UpgradeClassTipsWhichAreSpecialties.Select(c => c.ClassType));

            //if all candidates have different specialties in their tips ==> no upgrade
            if (firstNotSecond.Any() || secondNotFirst.Any())
            {
                return -1;
            }

            //if all candidates have the same specialties in their tips ==> Candidates will be tested by equipmentProperty
            if (!firstNotSecond.Any() && !secondNotFirst.Any())
            {
                if (HasPreferredWeaponLoadout())
                {
                    if (Settings.Instance.PreferShield && candidates.All(candidate =>
                            candidate.UpgradeClassTipsWhichAreSpecialties.All(c =>
                                c.ClassType == CharacterClassType.INFANTRY)))
                    {
                        var candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_SHIELD);

                        if (candidate != null)
                        {
                            return candidate.UpgradeTargetIndex;
                        }
                    }

                    if (Settings.Instance.RangedPreference == (int)RangedPreference.CROSSBOWS && candidates.All(candidate =>
                            candidate.UpgradeClassTipsWhichAreSpecialties.All(c =>
                                c.ClassType == CharacterClassType.RANGED)))
                    {
                        var candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_CROSS_BOW);

                        if (candidate != null)
                        {
                            return candidate.UpgradeTargetIndex;
                        }
                    }

                    if (Settings.Instance.RangedPreference == (int)RangedPreference.BOWS && candidates.All(candidate =>
                            candidate.UpgradeClassTipsWhichAreSpecialties.All(c =>
                                c.ClassType == CharacterClassType.RANGED)))
                    {
                        var candidate = GetCandidateWithWeaponType(candidates, EquipmentProperties.HAS_BOW);

                        if (candidate != null)
                        {
                            return candidate.UpgradeTargetIndex;
                        }
                    }
                }
            }

            return -1;
        }

        public static List<CharacterClassType> GetPreferredClassTypesByCulture(string cultureName)
        {
            return Settings.Instance.PreferredTroopsByCulture.FirstOrDefault(culture => culture.CultureIdentifier == cultureName)
                       ?.TroopClasses ?? new List<CharacterClassType>();
        }

        private static bool HasPreferredWeaponLoadout()
        {
            return Settings.Instance.RangedPreference != (int)RangedPreference.NONE || Settings.Instance.PreferShield;
        }

        private static UpgradeCandidate GetCandidateWithWeaponType(List<UpgradeCandidate> candidates, EquipmentProperties equipmentProperty)
        {
            var superCandidates = new List<UpgradeCandidate>();

            foreach (var candidate in candidates)
            {
                foreach (var upgradeCharacter in candidate.UpgradeClassTipsWhichAreSpecialties)
                {
                    if ((upgradeCharacter.EquipmentProperties & equipmentProperty) != 0)
                    {
                        superCandidates.Add(candidate);
                    }
                }
            }

            return superCandidates.Count == 1 ? superCandidates[0] : null;
        }
    }
}
