using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using UIExtenderLib;

namespace TroopManager {
    public class TroopManagerSubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();
            UIExtender.Register();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) {
            if (!(game.GameType is Campaign)) return;

            MBObjectManager.Instance.RegisterType<Sorter>("Sorter", "Sorters");
        }
    }
}
