using DemiacleSvm.UiMods;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;

namespace DemiacleSvm {
    public class ModEntry : Mod {

        public static ModData modData;
        private readonly string modDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Mods\\Demiacle_SVM\\";
        private const string saveFilePostfix = "_modData.xml";
        public static ModEntry modEntry;
        public static Boolean isTesting = false;
            
        public override void Entry(IModHelper helper) {

            modEntry = this;
            modData = new ModData();

            // Loads the correct settings on character load
            PlayerEvents.LoadedGame += loadModData;

            // Mods
            MenuEvents.MenuChanged += SkipIntro.onMenuChange;

            var uiModAccurateHearts = new UiModAccurateHearts();
            var uiModLocationOfTownsfolk = new UiModLocationOfTownsfolk();
            var uiModItemrolloverInformation = new UiModItemRolloverInformation();
            var uiModExperience = new UiModExperience();
            var uiModluckOfDay = new UiModLuckOfDay();

            var uiMods = new List<UiModWithOptions>();
            uiMods.Add( uiModAccurateHearts );
            uiMods.Add( uiModLocationOfTownsfolk );
            uiMods.Add( uiModItemrolloverInformation );
            uiMods.Add( uiModExperience );
            uiMods.Add( uiModluckOfDay );

            var optionPage = new OptionsPage( uiMods );

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
        internal void loadModData( object sender, EventArgsLoadedGameChanged e ) {

            string playerName = Game1.player.name;

            // File: \Mods\Demiacle_SVM\playerName_modData.xml
            // load file 
            if( File.Exists( modDirectory + playerName + saveFilePostfix ) ) {
                this.Monitor.Log( $"Mod data already exists for this {playerName}.... loading" );
                ModData loadedData = new ModData();
                Serializer.ReadFromXmlFile( out loadedData, playerName );

                // Only load options valid for this build
                foreach( var data in loadedData.uiOptions ) {
                    if( modData.uiOptions.ContainsKey( data.Key ) ) {
                        modData.uiOptions[ data.Key ] = loadedData.uiOptions[ data.Key ];
                    }
                }

                // Always load character location data
                // Beware this may need a check later
                modData.locationOfTownsfolkOptions = loadedData.locationOfTownsfolkOptions;
                    
                // If need to add more options create object here and merge with loaded data

            // create file and ModData
            } else {
                this.Monitor.Log( $"Mod data does not exist for this {playerName}... creating file" );
                updateModData();
            }

        }

        /// <summary>
        /// Overwrites the current modData to file
        /// </summary>
        internal static void updateModData() {
            Serializer.WriteToXmlFile( modData, Game1.player.name );
        }

    }
}
