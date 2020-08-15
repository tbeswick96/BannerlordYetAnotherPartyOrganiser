using System;
using HarmonyLib;
using ModLib.Debugging;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using UIExtenderLib;
using YAPO.Global;

// ReSharper disable UnusedType.Global

namespace YAPO {
    public class YapoSubModule : MBSubModuleBase {
        private readonly UIExtender _uiExtender = new UIExtender("YetAnotherPartyOrganiser");

        protected override void OnSubModuleLoad() {
            try {
                _uiExtender.Register();
            } catch (Exception exception) {
                ModDebug.ShowError("Failed to load YetAnotherPartyOrganiser", "OnSubModuleLoad exception", exception);
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot() {
            try {
                new Harmony("YAPO").PatchAll();
                _uiExtender.Verify();
            } catch (Exception exception) {
                ModDebug.ShowError("Failed to finish loading YetAnotherPartyOrganiser", "OnBeforeInitialModuleScreenSetAsRoot exception", exception);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject) {
            if (!(game.GameType is Campaign)) return;

            try {
                AddBehaviours(gameStarterObject as CampaignGameStarter);
            } catch (Exception exception) {
                ModDebug.ShowError("Something went wrong while starting a game", "OnGameStart exception", exception);
            }
        }

        private static void AddBehaviours(CampaignGameStarter campaignGameStarter) {
            campaignGameStarter?.LoadGameTexts($"{BasePath.Name}/Modules/{Strings.MODULE_FOLDER_NAME}/{Strings.MODULE_DATA_PARTY_COUNT_STRINGS}");
        }

        protected override void OnApplicationTick(float dt) {
            if (States.PartyVmMixin == null || Campaign.Current == null || Campaign.Current.CurrentMenuContext != null && (!Campaign.Current.CurrentMenuContext.GameMenu.IsWaitActive || Campaign.Current.TimeControlModeLock)) {
                return;
            }

            // TODO: This is messy af, must be a cleaner way of doing hotkeys
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
