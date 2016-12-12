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

//create list of mobs

//create boss mobs

//create all mobs on game create

//create spawn timers on event or on timers

//remove normal mob spawn and rocks when entering mine
//create persistant mob and rocks when entering mine ONLY the first time.
//load persistant mobs and rocks and ladder if previously visited


namespace Demiacle_SVM {
    public class ModEntry : Mod {

        public ModData modData;
        private PersistantMonsters persistantMonsters;
        private ScytheDamageMod scytheDamageMod;

        public ModEntry() {
            updateXmlSerializer();
            persistantMonsters = new PersistantMonsters( this );
            scytheDamageMod = new ScytheDamageMod( this );       
        }
        
        public override void Entry(IModHelper helper) {
            ControlEvents.KeyPressed += this.ReceiveKeyPress;
            //LocationEvents.CurrentLocationChanged += this.OnLocationChange;
            //PlayerEvents.FarmerChanged += persistantMonsters.OnFarmerChange;
            GraphicsEvents.OnPreRenderEvent += persistantMonsters.onPreRenderEvent;
            GraphicsEvents.OnPostRenderEvent += persistantMonsters.onPostRenderEvent;
            PlayerEvents.LoadedGame += this.onLoadedGame;
            PlayerEvents.LoadedGame += persistantMonsters.onLoadedGame;
            //PlayerEvents.FarmerChanged += persistantMonsters.onFarmerChanged;
            TimeEvents.OnNewDay += persistantMonsters.onNewDay;
            PlayerEvents.InventoryChanged += scytheDamageMod.onInvChange;
            LocationEvents.CurrentLocationChanged += persistantMonsters.onLocationChange;
        }

        private void ReceiveKeyPress(object sender, EventArgsKeyPressed e){
            if( Game1.currentLocation.name == null) {
                return;
            }
            this.Monitor.Log( $"Player pressed {e.KeyPressed} and is currently in {Game1.currentLocation.name} at location {Game1.player.position} and color is {Game1.outdoorLight}" );
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
            if( Serializer.FileExists( playerName ) ) {
                this.Monitor.Log( "Mod file already exists for this player.... loading" );
                modData = Serializer.ReadFromXmlFile( modData, playerName );
                // create file and ModData
            } else {
                this.Monitor.Log( "Mod file does not exist for this player... creating file" );
                modData = new ModData();
                Serializer.WriteToXmlFile( modData, playerName );
            }
        }

        /// <summary>
        /// Updates the XmlSerializer to allow specific monster types.
        /// This allows us to save all the specific monster data between days.
        /// </summary>
        private void updateXmlSerializer() {
            StardewValley.SaveGame.locationSerializer = new XmlSerializer( typeof( GameLocation ), new Type[ 38 ]
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
                typeof (ShadowBrute),
                typeof (ShadowShaman),
                typeof (Skeleton),
                typeof (RockGolem),
                typeof (SkeletonWarrior),
                typeof (SkeletonMage),
                typeof(ScytheDamageMod.Scythe)
            } );

            SaveGame.farmerSerializer = new XmlSerializer( typeof( Farmer ), new Type[ 2 ] { typeof( Tool ), typeof( ScytheDamageMod.Scythe ) } );

            SaveGame.serializer = new XmlSerializer( typeof( SaveGame ), new Type[ 39 ]
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
                  typeof (ShadowBrute),
                  typeof (ShadowShaman),
                  typeof (Skeleton),
                  typeof (SkeletonWarrior),
                  typeof (SkeletonMage),
                  typeof(ScytheDamageMod.Scythe)
                } );
        }
    }
}
