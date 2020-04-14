using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using UIExtenderLib;
using YAPO.Global;
using YAPO.Services;

namespace YAPO {
    public class YapoSubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();
            UIExtender.Register();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) {
            if (!(game.GameType is Campaign)) return;

            MBObjectManager.Instance.RegisterType<TroopSorterService>("Sorter", "Sorters");
        }

        protected override void OnApplicationTick(float dt) {
            if (States.PartyVmMixin == null || Campaign.Current == null || Campaign.Current.CurrentMenuContext != null && (!Campaign.Current.CurrentMenuContext.GameMenu.IsWaitActive || Campaign.Current.TimeControlModeLock)) {
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

            if (!Input.IsKeyDown(InputKey.LeftControl) || !Input.IsKeyDown(InputKey.LeftShift)) return;
            if (Input.IsKeyPressed(InputKey.A)) {
                States.PartyVmMixin.ExecuteSortPartyAscending();
                States.PartyVmMixin.ExecuteSortOtherAscending();
            } else if (Input.IsKeyPressed(InputKey.D)) {
                States.PartyVmMixin.ExecuteSortPartyDescending();
                States.PartyVmMixin.ExecuteSortOtherDescending();
            } else if (Input.IsKeyPressed(InputKey.U)) {
                States.PartyVmMixin.ExecuteActionUpgrade();
            } else if (Input.IsKeyPressed(InputKey.R)) {
                States.PartyVmMixin.ExecuteActionRecruit();
            }
        }
    }
}
