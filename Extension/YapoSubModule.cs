using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using UIExtenderLib;
using YAPO.Global;
using YAPO.Services;

// ReSharper disable UnusedType.Global

namespace YAPO {
    public class YapoSubModule : MBSubModuleBase {
        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();
            UIExtender.Register();

            try {
                new Harmony("YAPO").PatchAll();
            } catch (Exception exception) {
                InformationManager.DisplayMessage(new InformationMessage("YAPO Patch Failed " + exception.Message));
            }
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

            if (Input.IsKeyPressed(InputKey.U)) {
                States.PartyVmMixin.ExecuteActionUpgrade();
                States.PartyScreenWidget.Context.TwoDimensionContext.PlaySound("panels/twopanel_open");
            } else if (Input.IsKeyPressed(InputKey.R)) {
                States.PartyVmMixin.ExecuteActionRecruit();
                States.PartyScreenWidget.Context.TwoDimensionContext.PlaySound("panels/twopanel_open");
            }

            if (!Input.IsKeyDown(InputKey.LeftControl) || !Input.IsKeyDown(InputKey.LeftShift)) return;
            if (Input.IsKeyPressed(InputKey.A)) {
                States.PartyVmMixin.ExecuteSortPartyAscending();
                States.PartyVmMixin.ExecuteSortOtherAscending();
                States.PartyScreenWidget.Context.TwoDimensionContext.PlaySound("panels/twopanel_open");
            } else if (Input.IsKeyPressed(InputKey.D)) {
                States.PartyVmMixin.ExecuteSortPartyDescending();
                States.PartyVmMixin.ExecuteSortOtherDescending();
                States.PartyScreenWidget.Context.TwoDimensionContext.PlaySound("panels/twopanel_open");
            }
        }
    }
}
