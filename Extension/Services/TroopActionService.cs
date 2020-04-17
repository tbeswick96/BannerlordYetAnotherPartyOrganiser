using System;
using System.Collections.Generic;
using System.Linq;
using MountAndBlade.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using YAPO.Global;
using YAPO.MultipathUpgrade;

namespace YAPO.Services
{
    public static class TroopActionService
    {
        public static (int, int, int) UpgradeTroops(PartyVM partyVm, PartyScreenLogic partyScreenLogic)
        {
            List<PartyCharacterVM> upgradableTroops = partyVm.MainPartyTroops.Where(x => !x.IsHero && x.IsTroopUpgradable && x.IsUpgrade1Available && x.NumOfTarget1UpgradesAvailable > 0 || x.IsUpgrade2Available && x.NumOfTarget2UpgradesAvailable > 0).ToList();
            if (!upgradableTroops.Any())
            {
                Global.Helpers.Message("No troops available to upgrade");
                return (0, 0, 0);
            }

            int upgradedTotal = 0;
            int upgradedTypes = 0;
            int multiPathSkipped = 0;
            List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> commands = new List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>>();
            foreach (PartyCharacterVM troops in upgradableTroops)
            {
                PartyScreenLogic.PartyCommand.UpgradeTargetType upgradeTargetType =
                    PartyScreenLogic.PartyCommand.UpgradeTargetType.UpgradeTarget1;

                if (troops.IsUpgrade2Exists)
                {
                    if (!MultipathUpgradeLogic.TryGetUpgradePath(troops, out upgradeTargetType))
                    {
                        multiPathSkipped++;
                        continue;
                    }
                }

                int troopsToUpgrade = troops.NumOfUpgradeableTroops;
                upgradedTypes++;
                upgradedTotal += troopsToUpgrade;

                PartyScreenLogic.PartyCommand upgradeCommand = new PartyScreenLogic.PartyCommand();
                upgradeCommand.FillForUpgradeTroop(troops.Side, troops.Type, troops.Character, troopsToUpgrade, upgradeTargetType);
                commands.Add(new Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>(troops, upgradeCommand));
            }

            if (upgradedTotal == 0)
            {
                Global.Helpers.Message(multiPathSkipped > 0 ? $"No troops upgraded. {multiPathSkipped} troop types with multi-path upgrades were skipped" : "No troops upgraded");
                return (0, 0, 0);
            }

            foreach ((PartyCharacterVM partyCharacterVm, PartyScreenLogic.PartyCommand command) in commands)
            {
                partyVm.CurrentCharacter = partyCharacterVm;
                partyScreenLogic.AddCommand(command);
            }

            return (upgradedTotal, upgradedTypes, multiPathSkipped);
        }

        public static (int, int) RecruitPrisoners(PartyVM partyVm, PartyScreenLogic partyScreenLogic)
        {
            List<PartyCharacterVM> recruitablePrisoners = partyVm.MainPartyPrisoners.Where(x => !x.IsHero && x.IsTroopRecruitable && x.NumOfRecruitablePrisoners > 0).ToList();
            if (!recruitablePrisoners.Any())
            {
                Global.Helpers.Message("No prisoners available to recruit");
                return (0, 0);
            }

            int partySpace = partyScreenLogic.RightOwnerParty.PartySizeLimit - partyScreenLogic.MemberRosters[1].TotalManCount;
            partySpace = partySpace < 0 ? 0 : partySpace;
            if (partySpace == 0)
            {
                if (States.HotkeyControl)
                {
                    Global.Helpers.Message("Party limit reached! Limit overriden, proceeding...");
                } else
                {
                    Global.Helpers.Message("Party limit reached! Cannot recruit prisoners. (Hold control to override)");
                    return (0, 0);
                }
            }

            CharacterObject playerSelectedCharacter = partyVm.CurrentCharacter.Character;

            int recruitedTypes = 0;
            int recruitedTotal = 0;
            List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>> commands = new List<Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>>();
            foreach (var prisoners in recruitablePrisoners)
            {
                if (partySpace == 0 && !States.HotkeyControl) break;

                int prisonerCount = prisoners.NumOfRecruitablePrisoners;
                int prisonersToRecruit = States.HotkeyControl ? prisonerCount : Math.Min(prisonerCount, partySpace);
                int numOfRemainingRecruitablePrisoners = prisonerCount - prisonersToRecruit;
                recruitedTypes++;
                recruitedTotal += prisonersToRecruit;
                partySpace -= prisonersToRecruit;

                partyVm.CurrentCharacter.Character = prisoners.Character;

                Campaign.Current.GetCampaignBehavior<IRecruitPrisonersCampaignBehavior>()
                    .SetRecruitableNumber(partyVm.CurrentCharacter.Character, numOfRemainingRecruitablePrisoners + 1);

                PartyScreenLogic.PartyCommand recruitCommand = new PartyScreenLogic.PartyCommand();
                recruitCommand.FillForRecruitTroop(prisoners.Side, prisoners.Type, prisoners.Character, prisonersToRecruit);
                commands.Add(new Tuple<PartyCharacterVM, PartyScreenLogic.PartyCommand>(prisoners, recruitCommand));

                partyVm.CurrentCharacter.UpdateRecruitable();
            }

            partyVm.CurrentCharacter.Character = playerSelectedCharacter;

            if (commands.Count == 0)
            {
                Global.Helpers.Message("No prisoners recruited");
                return (0, 0);
            }

            foreach ((PartyCharacterVM partyCharacterVm, PartyScreenLogic.PartyCommand command) in commands)
            {
                partyVm.CurrentCharacter = partyCharacterVm;
                partyScreenLogic.AddCommand(command);
            }

            return (recruitedTotal, recruitedTypes);
        }
    }
}
