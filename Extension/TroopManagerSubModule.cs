using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TroopManager.Global;
using TroopManager.Services;
using UIExtenderLib;

namespace TroopManager {
    public class TroopManagerSubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();
            UIExtender.Register();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) {
            if (!(game.GameType is Campaign)) return;

            MBObjectManager.Instance.RegisterType<TroopSorterService>("Sorter", "Sorters");
        }

        protected override void OnApplicationTick(float dt) {
            if (Campaign.Current == null || Campaign.Current.CurrentMenuContext != null && (!Campaign.Current.CurrentMenuContext.GameMenu.IsWaitActive || Campaign.Current.TimeControlModeLock)) {
                return;
            }

            if (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl)) {
                if (!States.HotkeyControl) {
                    States.HotkeyControl = true;
                }
            } else {
                if (States.HotkeyControl) {
                    States.HotkeyControl = false;
                }
            }
        }
    }
}
