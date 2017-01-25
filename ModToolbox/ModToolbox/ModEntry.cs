using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DemiacleSvm {
    public class ModEntry : Mod {

        //public static ModData modData;
        public static ModEntry modEntry;
        public static IModHelper helper;
        // TODO Replace with helper method mod already has a saver and loader so change to use that
        public static readonly string modDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Mods\\Demiacle_UiMod\\";
        public const string saveFilePostfix = "_modData.xml";
        public static Boolean isTesting = false;

        public override void Entry( IModHelper helper ) {
            ModEntry.helper = helper;
            FieldInfo[] x = helper.Reflection.GetPrivateFields();
            ModEntry.modEntry = this;
            //modData = new ModData();

            // Loads the correct settings on character load
            SaveEvents.AfterLoad += loadModData;

        }

        internal static void Log( string log ) {
            if( isTesting ) {
                System.Console.WriteLine( log );
                return;
            }

            modEntry.Monitor.Log( log );
        }

        /// <summary>
        /// Loads mod specific data
        /// </summary>
        private void loadModData( object sender, EventArgs e ) {

            string playerName = Game1.player.name;

            // File: \Mods\Demiacle_SVM\playerName_modData.xml
            // load file 
            if ( Game1.player.name == "ModToolbox" ) {
                initializeMod();
            }

        }

        private void initializeMod() {
            GraphicsEvents.OnPostRenderEvent += drawTextures;
        }

        private void drawTextures( object sender, EventArgs e ) {
            if (  Game1.activeClickableMenu is SpriteSheetFinder == false ) {
                Game1.activeClickableMenu = new SpriteSheetFinder();
            }
        }
    }
}
