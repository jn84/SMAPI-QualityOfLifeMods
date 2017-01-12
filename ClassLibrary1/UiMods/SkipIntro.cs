using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System.Reflection;

namespace DemiacleSvm.UiMods {
    class SkipIntro {

        internal static void onMenuChange( object sender, EventArgsClickableMenuChanged e ) {
            try {
                TitleMenu menu = e.NewMenu as TitleMenu;
                menu.skipToTitleButtons();

                FieldInfo logoTimer = menu.GetType().GetField( "chuckleFishTimer", BindingFlags.Instance | BindingFlags.NonPublic );
                logoTimer.SetValue( menu, 0 );

                MenuEvents.MenuChanged -= SkipIntro.onMenuChange;

            } catch ( Exception exception ) {
                ModEntry.Log( "This should never be called." );
            }
        }

    }
}
