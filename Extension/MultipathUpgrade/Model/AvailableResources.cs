﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;

namespace YAPO.MultipathUpgrade.Model
{
    public class AvailableResources
    {
        public AvailableResources(bool hotkeyControl)
        {
            int cost = 0;
            if (!hotkeyControl)
            {
                int dailyCost = GetMostAccurateDailyCost();
                cost = dailyCost * YapoSettings.Instance.DaysToPayDailyCostsBuffer;
            }

            AvailableGold = Hero.MainHero.Clan.Gold + cost;
        }

        public int AvailableGold { get; set; }
        public Dictionary<ItemCategory, int> ItemsOfCategoryWithCount { get; set; } = new Dictionary<ItemCategory, int>();

        private int GetMostAccurateDailyCost()
        {
            try
            {
                int.TryParse(
                    CampaignUIHelper.GetGoldTooltip(Hero.MainHero.Clan)
                        .First(property => property.DefinitionLabel == "Expected Change").ValueLabel, out int dailyChange);

                return Math.Min(dailyChange, 0);
            }
            catch 
            {
                //ignored
            }
            return 0;
        }

        public void UpdateAvailableResources(PartyCharacterVM troops, int troopsToUpgrade, int upgradesPerTargetType)
        {
            AvailableGold -= troops.Character.UpgradeCost(PartyBase.MainParty, 0) * troopsToUpgrade;

            if (troops.Character.UpgradeTargets[upgradesPerTargetType]?.UpgradeRequiresItemFromCategory != null)
            {
                ItemsOfCategoryWithCount[troops.Character.UpgradeTargets[upgradesPerTargetType]?.UpgradeRequiresItemFromCategory] -= troopsToUpgrade;
            }
        }
    }
}
