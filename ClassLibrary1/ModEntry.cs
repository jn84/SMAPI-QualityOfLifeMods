using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;

using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using System.Xml.Serialization;
using Demiacle_SVM.OutdoorMonsters;
using Demiacle_SVM.OutdoorMonsters.AI;
using Demiacle_SVM.UiMods;
using Microsoft.Xna.Framework.Audio;
using Polenter.Serialization;
using System.IO;
using System.Reflection;

//create list of mobs

//create boss mobs

//create all mobs on game create

//create spawn timers on event or on timers

//remove normal mob spawn and rocks when entering mine
//create persistant mob and rocks when entering mine ONLY the first time.
//load persistant mobs and rocks and ladder if previously visited


namespace Demiacle_SVM {
    public class ModEntry : Mod {

        public static ModData modData;
        public static string modDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Mods\\Demiacle_SVM\\";
        private static string saveFilePostfix = "_modData.xml";
        public static ModEntry modEntry;
        public static Boolean isTesting = false;
        private static SharpSerializer serializer = new SharpSerializer();
            
        public override void Entry(IModHelper helper) {

            serializer.PropertyProvider.AttributesToIgnore.Clear();
            serializer.PropertyProvider.AttributesToIgnore.Add( typeof( XmlIgnoreAttribute ) );
                

            modEntry = this;
            modData = new ModData();

            // General mods needed for all other mods
            GameEvents.GameLoaded += overrideXMLSerializer;
            ControlEvents.KeyPressed += ReceiveKeyPress;
            PlayerEvents.LoadedGame += loadModData;

            MenuEvents.MenuChanged += SkipIntro.onMenuChange;

            //updateXmlSerializer();
            PersistantMonsters persistantMonsters = new PersistantMonsters();
            ScytheDamageMod scytheDamageMod = new ScytheDamageMod();
            SpeedMod speedMod = new SpeedMod();
            MineShaftMod mineShaftMod = new MineShaftMod();

            UiModAccurateHearts uiModAccurateHearts = new UiModAccurateHearts();
            UiModLocationOfTownsfolk uiModLocationOfTownsfolk = new UiModLocationOfTownsfolk( uiModAccurateHearts );
            UiModItemRolloverInformation uiModItemrolloverInformation = new UiModItemRolloverInformation();
            UiModExperience uiModExperience = new UiModExperience();
            UiModLuckOfDay uiModluckOfDay = new UiModLuckOfDay();

            OptionsPage optionPage = new OptionsPage( modData, uiModAccurateHearts, uiModLocationOfTownsfolk, uiModItemrolloverInformation, uiModExperience, uiModluckOfDay );

        }

        internal static void Log( string log ) {
            if( isTesting ) {
                System.Console.WriteLine( log );
                return;
            }

            modEntry.Monitor.Log( log );
        }

        private void ReceiveKeyPress(object sender, EventArgsKeyPressed e){

            if( Game1.currentLocation.name == null) {
                return;
            }

            foreach (NPC npc in Game1.currentLocation.characters) {
                //this.Monitor.Log("char list after " + npc.name);
            }

            if ( $"{e.KeyPressed}" == "Q" ) {
                List<NPC> characters = Game1.currentLocation.characters;
                Bat bat1 = new Bat( Game1.player.position );
                int num1 = 1;
                bat1.focusedOnFarmers = num1 != 0;
                characters.Add( bat1 );
                Game1.playSound("batScreech");
            }

            if( $"{e.KeyPressed}" == "N" ) {

                updateModData();
                //serializer.Serialize( modData.uiOptions, modDirectory + Game1.player.name + saveFilePostfix );

                //Log( "Current getTilelocationpoint is X: " + Game1.player.getTileX() + " Y:" + Game1.player.getTileY() );

                //Game1.performTenMinuteClockUpdate();

                /*
                Log( "Current gettilelocation.x is X: " + Game1.player.getTileLocation().X + " Y:" + Game1.player.position.Y / Game1.tileSize );
                Log( "Current gettilex is X: " + Game1.player.getTileX() + " Y:" + Game1.player.position.Y / Game1.tileSize );
                Log( "Current getTilelocationpoint is X: " + Game1.player.getTileLocationPoint() + " Y:" + Game1.player.position.Y / Game1.tileSize );

                Log("Map size is width: " + Game1.currentLocation.map.DisplayWidth / Game1.tileSize +" height: "+ +Game1.currentLocation.map.DisplayHeight / Game1.tileSize );*/

            }

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
                Serializer.ReadFromXmlFile( out modData, playerName );    

            // create file and ModData
            } else {
                this.Monitor.Log( $"Mod data does not exist for this {playerName}... creating file" );
                updateModData();
            }

        }

        /// <summary>
        /// Updates the XmlSerializer to allow specific monster types.
        /// This allows us to save all the specific monster data between days.
        /// </summary>
        private void overrideXMLSerializer( object sender, EventArgs e ) {
            StardewValley.SaveGame.locationSerializer = new XmlSerializer( typeof( GameLocation ), new Type[ 42 ]
            {
                typeof (Tool),
                typeof (Crow),
                typeof (Duggy),
                typeof (Fireball),
                typeof (Ghost),
                typeof (GreenSlime),
                typeof (LavaCrab),
                typeof (RockCrab),
                typeof (ShadowGuy),
                typeof (SkeletonWarrior),
                typeof (Child),
                typeof (Pet),
                typeof (Dog),
                typeof (StardewValley.Characters.Cat),
                typeof (Horse),
                typeof (SquidKid),
                typeof (Grub),
                typeof (Fly),
                typeof (DustSpirit),
                typeof (Bug),
                typeof (BigSlime),
                typeof (BreakableContainer),
                typeof (MetalHead),
                typeof (ShadowGirl),
                typeof (Monster),
                typeof (TerrainFeature),
                typeof (Bat),
                typeof (Ghost),
                typeof (GreenSlime),
                typeof (Mummy),
                typeof (RockCrab),
                typeof (OutDoorShadowBrute),
                typeof (ShadowShaman),
                typeof (Skeleton),
                typeof (RockGolem),
                typeof (SkeletonWarrior),
                typeof (SkeletonMage),
                typeof(ScytheDamageMod.Scythe),
                typeof(MoveTowardsTarget),
                typeof(PathFinding),
                typeof(HoldStill),
                typeof(OutDoorRockCrab)
            } );

            SaveGame.farmerSerializer = new XmlSerializer( typeof( Farmer ), new Type[ 2 ] { typeof( Tool ), typeof( ScytheDamageMod.Scythe ) } );

            SaveGame.serializer = new XmlSerializer( typeof( SaveGame ), new Type[ 43 ]
                {
                  typeof (Tool),
                  typeof (GameLocation),
                  typeof (Crow),
                  typeof (Duggy),
                  typeof (Bug),
                  typeof (BigSlime),
                  typeof (Fireball),
                  typeof (Ghost),
                  typeof (Child),
                  typeof (Pet),
                  typeof (Dog),
                  typeof (StardewValley.Characters.Cat),
                  typeof (Horse),
                  typeof (GreenSlime),
                  typeof (LavaCrab),
                  typeof (RockCrab),
                  typeof (ShadowGuy),
                  typeof (SkeletonMage),
                  typeof (SquidKid),
                  typeof (Grub),
                  typeof (Fly),
                  typeof (DustSpirit),
                  typeof (Quest),
                  typeof (MetalHead),
                  typeof (ShadowGirl),
                  typeof (Monster),
                  typeof (TerrainFeature),
                  typeof (Bat),
                  typeof (Ghost),
                  typeof (GreenSlime),
                  typeof (Mummy),
                  typeof (RockCrab),
                  typeof (RockGolem),
                  typeof (OutDoorShadowBrute),
                  typeof (ShadowShaman),
                  typeof (Skeleton),
                  typeof (SkeletonWarrior),
                  typeof (SkeletonMage),
                  typeof(ScytheDamageMod.Scythe),
                typeof(MoveTowardsTarget),
                typeof(PathFinding),
                typeof(HoldStill),
                  typeof(OutDoorRockCrab)
                } );
        }

        /// <summary>
        /// Overwrites the current modData to file
        /// </summary>
        internal static void updateModData() {
            Serializer.WriteToXmlFile( modData, Game1.player.name );
        }

    }
}
