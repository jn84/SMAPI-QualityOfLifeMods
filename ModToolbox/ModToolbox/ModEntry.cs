using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ModToolbox;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ModToolbox {
    public class ModEntry : Mod {

        //public static ModData modData;
        public static ModEntry modEntry;
        public static IModHelper helper;
        // TODO Replace with helper method mod already has a saver and loader so change to use that
        public static readonly string modDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Mods\\Demiacle_UiMod\\";
        public const string saveFilePostfix = "_modData.xml";
        public static Boolean isTesting = false;

        private ModToolboxButton button = new ModToolboxButton();

        public override void Entry( IModHelper helper ) {
            ModEntry.helper = helper;
            ModEntry.modEntry = this;

            GraphicsEvents.OnPostRenderEvent += drawButton;
            ControlEvents.MouseChanged += handleButtonclick;

        }

        private void handleButtonclick( object sender, EventArgsMouseStateChanged e ) {
            if ( Game1.activeClickableMenu is TitleMenu == false ) {
                return;
            }

            if( e.NewState.LeftButton == ButtonState.Pressed ) {
                button.receiveLeftClick( e.NewPosition.X, e.NewPosition.Y );
            }
        }

        private void drawButton( object sender, EventArgs e ) {
            if( Game1.activeClickableMenu is TitleMenu == false ) {
                return;
            }

            button.draw( Game1.spriteBatch );
        }

        internal static void Log( string log ) {
            if( isTesting ) {
                System.Console.WriteLine( log );
                return;
            }

            modEntry.Monitor.Log( log );
        }

    }
}
