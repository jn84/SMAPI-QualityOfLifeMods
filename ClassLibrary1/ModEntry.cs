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
        private static string saveFilePostfix = "_modData";
        private PersistantMonsters persistantMonsters;
        private ScytheDamageMod scytheDamageMod;
        private SpeedMod speedMod;
        private MineShaftMod mineShaftMod;
        public static ModEntry modEntry;
        public static Boolean isTesting = false;

        public ModEntry() {
            modEntry = this;
            //updateXmlSerializer();
            persistantMonsters = new PersistantMonsters();
            scytheDamageMod = new ScytheDamageMod();
            speedMod = new SpeedMod();
            mineShaftMod = new MineShaftMod();
        }

        internal static void Log( string log ) {
            if( isTesting ) {
                System.Console.WriteLine( log );
                return;
            }

            modEntry.Monitor.Log( log );
        }

        public override void Entry(IModHelper helper) {
            //general mods needed for all other mods
            GameEvents.GameLoaded += this.updateXmlSerializer;
            ControlEvents.KeyPressed += this.ReceiveKeyPress;
            PlayerEvents.LoadedGame += this.onLoadedGame;
            //LocationEvents.CurrentLocationChanged += this.OnLocationChange;


            //Weapon and tool mod
            PlayerEvents.InventoryChanged += scytheDamageMod.onInvChange;


            //persistantMonster mod
            GraphicsEvents.OnPreRenderEvent += persistantMonsters.onPreRenderEvent;
            GraphicsEvents.OnPostRenderEvent += persistantMonsters.onPostRenderEvent;
            LocationEvents.CurrentLocationChanged += persistantMonsters.onLocationChange;
            PlayerEvents.LoadedGame += persistantMonsters.onLoadedGame;
            TimeEvents.OnNewDay += persistantMonsters.onNewDay;
            TimeEvents.DayOfMonthChanged += persistantMonsters.onDayChange;
            GameEvents.OneSecondTick += persistantMonsters.onGameOneSecondTick;
            //LocationEvents.CurrentLocationChanged += PathFinder.onLocationChange;
            LocationEvents.LocationObjectsChanged += PathFinderMap.Instance.updatePassableTilesOnLocationObjectsChanged;
            LocationEvents.CurrentLocationChanged += PathFinderMap.Instance.updateMapOnChangeLocation;
            //PlayerEvents.FarmerChanged += persistantMonsters.onFarmerChanged;



            //minshaft mod
            //PlayerEvents.LoadedGame += mineShaftMod.onLoad;
            LocationEvents.CurrentLocationChanged += mineShaftMod.onLocationChange;
            GraphicsEvents.OnPreRenderGuiEvent += mineShaftMod.OnPreRenderGuiEvent;

            //speed mod
            PlayerEvents.InventoryChanged += speedMod.onInvChange;
            GameEvents.SecondUpdateTick += speedMod.checkTileForRoad;
            GameEvents.QuarterSecondTick += speedMod.forcePlayerToSpeed;
        }

        private void ReceiveKeyPress(object sender, EventArgsKeyPressed e){
            if( Game1.currentLocation.name == null) {
                return;
            }
            //this.Monitor.Log( $"Player pressed {e.KeyPressed} and is currently in {Game1.currentLocation.name} at location {Game1.player.position} and color is {Game1.outdoorLight}" );
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
                Log( "Current getTilelocationpoint is X: " + Game1.player.getTileX() + " Y:" + Game1.player.getTileY() );
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
        internal void onLoadedGame( object sender, EventArgsLoadedGameChanged e ) {
            this.Monitor.Log( "On Farmer Changed is being called" );

            string playerName = Game1.player.name;
            this.Monitor.Log( "Current farmer loaded: " + playerName );

            // file \Mods\Demiacle_SVM\playerName.xml
            // load file 
            if( Serializer.FileExists( playerName + saveFilePostfix ) ) {
                this.Monitor.Log( "Mod file already exists for this player.... loading" );
                Serializer.ReadFromXmlFile( out modData, playerName + saveFilePostfix );
                // create file and ModData
            } else {
                this.Monitor.Log( "Mod file does not exist for this player... creating file" );
                modData = new ModData();
                updateModData();
            }
        }

        /// <summary>
        /// Updates the XmlSerializer to allow specific monster types.
        /// This allows us to save all the specific monster data between days.
        /// </summary>
        private void updateXmlSerializer( object sender, EventArgs e ) {
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
            Serializer.WriteToXmlFile( modData, Game1.player.name + saveFilePostfix );
        }
    }
}
