using System;
using System.Linq;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

// ReSharper disable once ParameterTypeCanBeEnumerable.Global

namespace YAPO.Services
{
    public static class PartyHeaderCountHelper
    {
        public static string PopulatePartyListLabel(MBBindingList<PartyCharacterVM> partyList, int limit = 0)
        {
            int troopsActive = partyList.Sum(item => Math.Max(0, item.Number - item.WoundedCount));
            int troopsWeak = partyList.Sum(item => item.Number < item.WoundedCount ? 0 : item.WoundedCount);

            MBTextManager.SetTextVariable("COUNT", troopsActive);
            MBTextManager.SetTextVariable("WEAK_COUNT", troopsWeak);
            if (limit == 0)
            {
                return troopsWeak > 0
                           ? GameTexts.FindText("str_party_list_label_with_weak_without_max").ToString()
                           : troopsActive.ToString();
            }

            MBTextManager.SetTextVariable("MAX_COUNT", limit);
            if (troopsWeak > 0)
            {
                MBTextManager.SetTextVariable("PARTY_LIST_TAG", "");
                MBTextManager.SetTextVariable("WEAK_COUNT", troopsWeak);
                MBTextManager.SetTextVariable("TOTAL_COUNT", troopsActive + troopsWeak);
                return GameTexts.FindText("str_party_list_label_with_weak_and_total").ToString();
            }

            MBTextManager.SetTextVariable("PARTY_LIST_TAG", "");
            return GameTexts.FindText("str_party_list_label").ToString();
        }
    }
}
