using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace YAPO.Fixes.Formations
{
    public class FixedFormationsBehaviour : CampaignBehaviorBase
    {
        public static readonly FixedFormationsBehaviour Instance = new FixedFormationsBehaviour();

        public Dictionary<BasicCharacterObject, FormationClass> FormationClasses;

        public bool GameStarted { get; set; }

        public override void RegisterEvents()
        {
            // Nothing to register
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (!GameStarted)
            {
                FormationClasses = new Dictionary<BasicCharacterObject, FormationClass>();
            }

            dataStore.SyncData("formation_class_map", ref FormationClasses);
        }
    }
}
