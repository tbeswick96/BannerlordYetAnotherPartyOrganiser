using System;
using System.Collections.Generic;
using System.Linq;
using MountAndBlade.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using YAPO.Global;
using YAPO.MultipathUpgrade;

namespace YAPO.Services
{
    public static class TroopActionService
    {
        public static UpgradeResults UpgradeTroops(PartyVM partyVm, PartyScreenLogic partyScreenLogic)
        {
            UpgradeResults results = new UpgradeResults();

            List<PartyCharacterVM> upgradableTroops = GetUpgradeableTroops(partyVm);
            if (upgradableTroops.IsEmpty())
            {
                Global.Helpers.Message("No troops available to upgrade");
                return results;
            }

            List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> commands =
                PrepareUpgradeCommands(upgradableTroops, ref results);

            if (results.UpgradedTotal == 0)
            {
                Global.Helpers.Message(results.MultiPathSkipped > 0
                                           ? $"No troops upgraded. {results.MultiPathSkipped} troop types with multi-path upgrades were skipped"
                                           : "No troops upgraded");
            }
            else
            {
                ExecuteCommands(commands, partyVm, partyScreenLogic);
            }

            return results;
        }

        public static RecruitmentResults RecruitPrisoners(PartyVM partyVm, PartyScreenLogic partyScreenLogic)
        {
            RecruitmentResults results = new RecruitmentResults();

            List<PartyCharacterVM> recruitablePrisoners = GetRecruitablePrisoners(partyVm);
            if (recruitablePrisoners.IsEmpty())
            {
                Global.Helpers.Message("No prisoners available to recruit");
                return results;
            }

            int partySpace = partyScreenLogic.RightOwnerParty.PartySizeLimit -
                             partyScreenLogic.MemberRosters[1].TotalManCount;
            partySpace = partySpace < 0 ? 0 : partySpace;
            if (partySpace == 0)
            {
                if (States.HotkeyControl)
                {
                    Global.Helpers.Message("Party limit reached! Limit overriden, proceeding...");
                }
                else
                {
                    Global.Helpers.Message("Party limit reached! Cannot recruit prisoners. (Hold control to override)");
                    return results;
                }
            }

            List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> commands =
                PrepareRecruitmentCommands(recruitablePrisoners, partySpace, ref results, partyVm);

            if (commands.Count == 0)
            {
                Global.Helpers.Message("No prisoners recruited");
            }
            else
            {
                ExecuteCommands(commands, partyVm, partyScreenLogic);
            }

            return results;
        }

        private static List<PartyCharacterVM> GetUpgradeableTroops(PartyVM partyVm)
        {
            return partyVm.MainPartyTroops.Where(x =>
                                                     !x.IsHero &&
                                                     x.IsTroopUpgradable &&
                                                     x.IsUpgrade1Available &&
                                                     !x.IsUpgrade1Insufficient &&
                                                     x.NumOfTarget1UpgradesAvailable > 0 ||
                                                     x.IsUpgrade2Available &&
                                                     !x.IsUpgrade2Insufficient &&
                                                     x.NumOfTarget2UpgradesAvailable > 0)
                          .ToList();
        }

        private static List<PartyCharacterVM> GetRecruitablePrisoners(PartyVM partyVm)
        {
            return partyVm.MainPartyPrisoners.Where(x =>
                                                        !x.IsHero &&
                                                        x.IsRecruitablePrisoner &&
                                                        x.NumOfRecruitablePrisoners > 0)
                          .ToList();
        }

        private static List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> PrepareUpgradeCommands(
            List<PartyCharacterVM> upgradableTroops,
            ref UpgradeResults upgradeResults)
        {
            List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> commands =
                new List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>>();
            foreach (PartyCharacterVM troops in upgradableTroops)
            {
                PartyScreenLogic.PartyCommand.UpgradeTargetType upgradeTargetType =
                    PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget1;

                if (troops.IsUpgrade2Exists)
                {
                    if (MultipathUpgradeLogic.TryGetUpgradePath(troops, out upgradeTargetType))
                    {
                        upgradeResults.MultiPathUpgraded++;
                    }
                    else
                    {
                        upgradeResults.MultiPathSkipped++;
                        continue;
                    }
                }

                int troopsToUpgrade = troops.NumOfUpgradeableTroops;
                upgradeResults.UpgradedTypes++;
                upgradeResults.UpgradedTotal += troopsToUpgrade;

                PartyScreenLogic.PartyCommand upgradeCommand = new PartyScreenLogic.PartyCommand();
                upgradeCommand.FillForUpgradeTroop(troops.Side,
                                                   troops.Type,
                                                   troops.Character,
                                                   troopsToUpgrade,
                                                   upgradeTargetType);
                commands.Add(new Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>(troops, upgradeCommand));
            }

            return commands;
        }

        private static List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> PrepareRecruitmentCommands(
            List<PartyCharacterVM> recruitablePrisoners,
            int partySpace,
            ref RecruitmentResults results,
            PartyVM partyVm)
        {
            List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> commands =
                new List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>>();

            CharacterObject playerSelectedCharacter = partyVm.CurrentCharacter.Character;

            foreach (PartyCharacterVM prisoners in recruitablePrisoners)
            {
                if (partySpace == 0 && !States.HotkeyControl) break;

                int prisonerCount = prisoners.NumOfRecruitablePrisoners;
                int prisonersToRecruit = States.HotkeyControl ? prisonerCount : Math.Min(prisonerCount, partySpace);
                int numOfRemainingRecruitablePrisoners = prisonerCount - prisonersToRecruit;
                results.RecruitedTypes++;
                results.RecruitedTotal += prisonersToRecruit;
                partySpace -= prisonersToRecruit;

                partyVm.CurrentCharacter.Character = prisoners.Character;

                Campaign.Current.GetCampaignBehavior<IRecruitPrisonersCampaignBehavior>()
                        .SetRecruitableNumber(partyVm.CurrentCharacter.Character,
                                              numOfRemainingRecruitablePrisoners + 1);

                PartyScreenLogic.PartyCommand recruitCommand = new PartyScreenLogic.PartyCommand();
                recruitCommand.FillForRecruitTroop(prisoners.Side,
                                                   prisoners.Type,
                                                   prisoners.Character,
                                                   prisonersToRecruit);
                commands.Add(new Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>(prisoners, recruitCommand));

                partyVm.CurrentCharacter.UpdateRecruitable();
            }

            partyVm.CurrentCharacter.Character = playerSelectedCharacter;

            return commands;
        }

        private static void ExecuteCommands(List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> commands,
                                            PartyVM partyVm,
                                            PartyScreenLogic partyScreenLogic)
        {
            foreach ((PartyCharacterVM partyCharacterVm, PartyScreenLogic.PartyCommand command) in commands)
            {
                partyVm.CurrentCharacter = partyCharacterVm;
                partyScreenLogic.AddCommand(command);
            }
        }
    }
}
