using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;

namespace YAPO.GUI
{
    public static class WidgetHelpers
    {
        public static void RefreshWidgetState(this List<Widget> allWidgets, string widgetName, Func<bool> statePredicate)
        {
            Widget widget = allWidgets.FirstOrDefault(x => x.Id == widgetName);
            Widget widgetDisabled = allWidgets.FirstOrDefault(x => x.Id == $"{widgetName}Disabled");
            if (widget == null || widgetDisabled == null) return;

            bool state = statePredicate();
            widget.IsHidden = !state;
            widgetDisabled.IsHidden = state;
        }
    }
}
