using System;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using StardewValley.Tools;

namespace Demiacle_SVM
{
    class PersistantMonsters {
        List<Monster> monsterGlidersToFix = new List<Monster>();
        Random random = new Random();
        
        private ModEntry mod;

        /// <summary>
        /// Mod that adds a slew of monsters and allows them to persist between days.
        /// Also makes mines persistant instead of respawn randomized, adds elevator every level of mine, and makes all mines dark.
        /// Also expands the dark forest area.
        /// </summary>
        public PersistantMonsters( ModEntry mod ) {
            
            this.mod = mod;
        }

        /// <summary>
        /// Creates all the monsters if they havent been created
        /// Moves all the positions of monsters to their home position
        /// </summary>
        public void onNewDay(object sender, EventArgsNewDay e) {
            foreach ( GameLocation location in Game1.locations) {
                foreach (NPC npc in location.characters) {
                    if (npc is Monster) {
                        //DefaultPosition is the position the npc was spawned at
                        npc.position = npc.DefaultPosition;
                    }
                }
            }
        }

        

        internal void onLocationChange( object sender, EventArgsCurrentLocationChanged e ) {
            //Game1.globalOutdoorLighting -= 0.1f;
            Game1.outdoorLight = new Color( new Vector3( 220, 220, 220 ) );
            Game1.ambientLight = new Color( new Vector3( 220, 220, 220 ) );
        }

        internal void onLoadedGame( object sender, EventArgsLoadedGameChanged e ) {
            if( !mod.modData.hasMonstersBeenCreated ) {
                createAllMonsters();
            }
        }
        
        /// <summary>
        /// Allows rendering of glider mobs since the draw method for glider mobs is only called on certain GameLocation
        /// WARNING: This does create graphical glitches because the gliders are now being rendered behind buildings however the 
        /// due to limitations of the api the only other option currently known is to render above all elements
        /// </summary>
        public void onPreRenderEvent(object sender, EventArgs e) {
            if ( currentLocationDrawsGliders() ) {
                return;
            }

            foreach (NPC npc in Game1.currentLocation.characters ) {
                if (npc is Monster) {
                    ((Monster)npc).isGlider = false;
                    monsterGlidersToFix.Add(((Monster)npc));
                }
            }            
        }
        

        /// <summary>
        /// Generates all the world map monsters
        /// </summary>
        private void createAllMonsters() {
            foreach ( GameLocation location in Game1.locations ) {
                //mod.Monitor.Log( location.ToString() );
                List<NPC> characters = location.characters;


                int minX = 100;
                int maxX = 600;
                int minY = 80;
                int maxY = 300;

                int omitMinX = 0;
                int omitMaxX = 0;
                int omitMinY = 0;
                int omitMaxY = 0;

                int amountOfFarmMobsToSpawn = 200;
                int amountOfBusMobsToSpawn = 64;
                int amountOfForestMobsToSpawn = 500;

                switch ( location.name ){
                    case "FarmHouse":
                        mod.Monitor.Log( "Building FarmHouse mobs" );

                        characters.Add( new Bat( new Vector2( 300, 200 ) ) );
                        break;
                    case "FarmCave":
                        mod.Monitor.Log( "Building FarmCave mobs" );
                        int amountOfBatsToSpawn = 17;
                        minX = 100;
                        maxX = 600;
                        minY = 80;
                        maxY = 300;

                        for ( int i = 0; i < amountOfBatsToSpawn; i++ ) {
                            Vector2 position = new Vector2( random.Next( minX, maxX ), random.Next( minY, maxY ) );
                            characters.Add( new Bat( position ) );
                        }
                        
                        //bat1.focusedOnFarmers = true;
                        //characters.Add(bat1);
                        break;
                    case "BusStop":
                        mod.Monitor.Log( "Building BusStop mobs" );
                        
                        minX = 300;
                        maxX = 1750;
                        minY = 380;
                        maxY = 1200;

                        omitMinX = 670;
                        omitMaxX = 1200;
                        omitMinY = 410;
                        omitMaxY = 640;

                        for( int i = 0; i < amountOfBusMobsToSpawn; i++ ) {
                            Vector2 position = getRandomInsideSquare( minX, maxX, minY, maxY, omitMinX, omitMaxX, omitMinY, omitMaxY );
                            characters.Add( new GreenSlime( position ) );
                        }
                        break;
                    case "Farm":
                        minX = 250;
                        maxX = 4550;
                        minY = 550;
                        maxY = 3750;

                        for( int i = 0; i < amountOfFarmMobsToSpawn; i++ ) {
                            Vector2 position = getRandomInsideSquare( minX, maxX, minY, maxY );
                            characters.Add( generateRandomEasyMob( position ) );
                        }

                        break;
                    case "Town":

                        break;
                    case "Beach":

                        break;
                    case "Mountain":

                        break;
                    case "Backwoods":

                        break;
                    case "Forest":
                        minX = 650;
                        maxX = 7000;
                        minY = 640;
                        maxY = 6000;

                        for( int i = 0; i < amountOfFarmMobsToSpawn; i++ ) {
                            Vector2 position = getRandomInsideSquare( minX, maxX, minY, maxY );
                            characters.Add( generateRandomEasyMob( position ) );
                        }

                        break;
                }
            }

            createSafeDialogue( "It looks like this old house has some bats... great...", 5000  );
            createSafeDialogue( "Things feel different today...", 20000 );
            createSafeDialogue( "Its as if the world just got a little more strange... odd...", 45000 );
            
            mod.Monitor.Log( "Mobs finished creating" );
            mod.modData.hasMonstersBeenCreated = true;
            Serializer.WriteToXmlFile( mod.modData, Game1.player.name );
        }

        
        /// <summary>
        /// Generates an easy random mob
        /// duggy is great but when it leaves the tile gets fucked up
        /// ghosts auto agro
        /// dustspirit vision code is called only inside mine - throws error
        /// skeleton """""
        /// bug constructor gives null pointer
        /// Crow graphics are allll glitched
        /// METALhEAD contructor code is funky - throws error
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private NPC generateRandomEasyMob( Vector2 position ) {
            int chance = random.Next( 1, 6);
            switch( chance ) {
                case 1:
                    return new GreenSlime( position );
                case 2:
                    return new Bat( position );
                case 3:
                    return new ShadowBrute( position );
                case 4:
                    return new RockCrab( position );
                case 5:
                    mod.Monitor.Log("creating golem");
                    return new RockGolem( position );
                default:
                    return new Bat( position );
            }
        }

        private Vector2 getRandomInsideSquare( int minX, int maxX, int minY, int maxY ) {
            return new Vector2( random.Next( minX, maxX ), random.Next( minY, maxY ) );
        }

        private Vector2 getRandomInsideSquare( int minX, int maxX, int minY, int maxY, int omitMinX, int omitMaxX, int omitMinY, int omitMaxY ) {
            Vector2 position = new Vector2( random.Next( minX, maxX ), random.Next( minY, maxY ) );
            Rectangle rectangle = new Rectangle( omitMinX, omitMinY, omitMaxX - omitMinX, omitMaxY - omitMinY);
            if( isInOmittedSquare( position, rectangle ) ) {
               return getRandomInsideSquare( minX, maxX, minY, maxY, omitMinX, omitMaxX, omitMinY, omitMaxY );
            } else {
                return position;
            }
        }

        private Boolean isInOmittedSquare( Vector2 position, Rectangle rect ) {
            if( rect.Contains( (int) position.X, (int) position.Y ) ) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Creates a dialogue that only shows up if a menu is not open. And retries every second to see if menu has closed
        /// This is needed to stop a bug of disapearing items on forced exits of menu
        /// </summary>
        private void createSafeDialogue( string dialogue, int timer ) {
            Task.Factory.StartNew( () => {
                System.Threading.Thread.Sleep( timer );
                while( true ) {
                    System.Threading.Thread.Sleep( 1000 );
                    if( !(Game1.activeClickableMenu is StardewValley.Menus.GameMenu)  ) {
                        Game1.setDialogue( dialogue, true );
                        return;
                    }                         
                }
            } );
        }

        /// <summary>
        /// Re-sets isGlider property of glider monsters - see onPreRenderEvent( object, EventArgs )
        /// </summary>
        public void onPostRenderEvent(object sender, EventArgs e) {
            if ( currentLocationDrawsGliders() ) {
                return;
            }

            foreach (Monster monster in monsterGlidersToFix) {
                monster.isGlider = true;
            }
            monsterGlidersToFix.Clear();
        }
        
        /// <returns>true if the current location draws gliders correctly already</returns>
        private bool currentLocationDrawsGliders() {
            if ( ( Game1.currentLocation is Farm && Game1.spawnMonstersAtNight ) || Game1.currentLocation is StardewValley.Locations.MineShaft ) {
                return true;
            } else {
                return false;
            }
        }

        
    }
}
