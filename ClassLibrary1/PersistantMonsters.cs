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
using System.Xml;
//using Polenter.Serialization;
using System.IO;
using Demiacle_SVM.OutdoorMonsters;
using Demiacle_SVM.OutdoorMonsters.AI;


//TODO will have to save persistant monsters to its own xml and place inside mod


namespace Demiacle_SVM
{
    class PersistantMonsters {
        List<Monster> monsterGlidersToFix = new List<Monster>();
        public Dictionary<Monster, GameLocation> savedMonsters = new Dictionary<Monster, GameLocation>();
        //SharpSerializer serializer = new SharpSerializer();
        string saveFileLocation;

        private bool isFirstLoad = true;

        /// <summary>
        /// Mod that adds a slew of monsters and allows them to persist between days.
        /// Also makes mines persistant instead of respawn randomized, adds elevator every level of mine, and makes all mines dark.
        /// Also expands the dark forest area.
        /// </summary>
        public PersistantMonsters() { }

        /// <summary>
        /// Creates all the monsters if they havent been created
        /// Moves all the positions of monsters to their home position
        /// </summary>
        public void onNewDay(object sender, EventArgsNewDay e) {
            
            if( !e.IsNewDay ) {
                //this isnt late enough monsters still get saved after this call
                ModEntry.Log("day is starting");

                foreach( GameLocation location in Game1.locations ) {
                    foreach( NPC npc in location.characters ) {
                        if( npc is Monster ) {
                            shiftDefaultLocation( npc, location );
                            npc.position = npc.DefaultPosition;
                        }
                    }
                }

                if( !ModEntry.modData.hasMonstersBeenCreated ) {
                    ModEntry.Log( "creating persistant monsters" );
                    createAllMonsters();

                } else if ( !ModEntry.modData.hasMonstersBeenLoaded ) {
                    ModEntry.Log( "loading persistant monsters" );
                    //savedMonsters = ( Dictionary<Monster, GameLocation> ) serializer.Deserialize( saveFileLocation ); // cant deserialize animatedsprite
                }

                foreach( KeyValuePair<Monster, GameLocation> entry in savedMonsters ) {
                    //entry.Value.addCharacter( entry.Key );
                }
                //savedMonsters.Clear();

            //remove mobs from locations so they dont save and save them in their own file
            } else {
                ModEntry.Log( "day is ending" );
                if( savedMonsters.Count > 0 ) {
                    //putPersistantMonstersInSave();
                }
            }
        }

        /// <summary>
        /// Shifts the DefaultPosition of the monster to any of the 8 adjacent squares if that square is open
        /// </summary>
        public void shiftDefaultLocation( NPC npc, GameLocation location ) {
            int chance1 = Game1.random.Next( -1, 2 );
            int chance2 = Game1.random.Next( -1, 2 );
            float newX = npc.DefaultPosition.X + ( chance1 * Game1.tileSize );
            float newY = npc.DefaultPosition.Y + ( chance2 * Game1.tileSize );

            if( PathFinderMap.isTileWalkable( ( int ) newX / Game1.tileSize, ( int ) newY / Game1.tileSize, location ) ) {
                npc.DefaultPosition = new Vector2( newX, newY );
            }
        }

        internal void onDayChange( object sender, EventArgsIntChanged e ) {
            ModEntry.Log("Day is changed");
        }

        internal void onGameOneSecondTick( object sender, EventArgs e ) {
            
        }

        /// <summary>
        /// Deprecated. Was used to remove monsters from locations and save them in a separate file
        /// </summary>
        private void putPersistantMonstersInSave() {
            foreach( GameLocation location in Game1.locations.ToArray() ) {
                foreach( NPC npc in location.characters.ToArray() ) {
                    if( npc is Monster ) {
                        //ModEntry.Log( "removing" + npc.name );

                        location.characters.Remove( npc );
                        savedMonsters.Add( ( Monster ) npc, location );
                    }
                }
            }

            //serializer.Serialize( savedMonsters, saveFileLocation );
        }

        internal void onLocationChange( object sender, EventArgsCurrentLocationChanged e ) {
            //Game1.globalOutdoorLighting -= 0.1f;
            Game1.outdoorLight = new Color( new Vector3( 220, 220, 220 ) );
            Game1.ambientLight = new Color( new Vector3( 220, 220, 220 ) );
        }

        internal void onLoadedGame( object sender, EventArgsLoadedGameChanged e ) {
            saveFileLocation = AppDomain.CurrentDomain.BaseDirectory + "\\Mods\\Demiacle_SVM\\" + Game1.player.name + "_monsterData.xml";
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

            foreach ( NPC npc in Game1.currentLocation.characters ) {
                if ( npc is Monster && ( ( Monster) npc).isGlider == true ) {
                    ((Monster)npc).isGlider = false;
                    monsterGlidersToFix.Add(((Monster)npc));
                }
            }            
        }


        /// <summary>
        /// Generates all the world map monsters and places them into savedMonsters
        /// </summary>
        private void createAllMonsters() {
            foreach ( GameLocation location in Game1.locations ) {
                //ModEntry.Log( location.ToString() );
                List<NPC> characters = location.characters;

                int minX = 100;
                int maxX = 600;
                int minY = 80;
                int maxY = 300;

                int omitMinX = 0;
                int omitMaxX = 0;
                int omitMinY = 0;
                int omitMaxY = 0;

                int amountOfFarmMobsToSpawn = 100;
                int amountOfBusMobsToSpawn = 68;
                int amountOfForestMobsToSpawn = 500;
                int amountOfBackwoodsMobsToSpawn = 40;
                int amountOfMountainMobsToSpawn = 10;
                int amountOfTownMobsToSpawn = 10;
                int amountOfBeachMobsToSpawn = 200;

                ModEntry.Log( $"Building {location.name} mobs" );

                switch ( location.name ){
                    case "FarmHouse":

                        characters.Add( new Bat( new Vector2( 300, 200 ) ) );
                        characters.Add( new Bat( new Vector2( 100, 220 ) ) );
                        characters.Add( new Bat( new Vector2( 600, 350 ) ) );
                        break;

                    case "FarmCave":
                        int amountOfBatsToSpawn = 17;
                        minX = 100;
                        maxX = 600;
                        minY = 80;
                        maxY = 300;

                        for ( int i = 0; i < amountOfBatsToSpawn; i++ ) {
                            Vector2 position = new Vector2( Game1.random.Next( minX, maxX ), Game1.random.Next( minY, maxY ) );
                            Bat farmMonster = new Bat( position );
                            farmMonster.currentLocation = location;
                            characters.Add( farmMonster );                            
                        }

                        break;

                    case "BusStop":
                        
                        minX = 300;
                        maxX = 1750;
                        minY = 380;
                        maxY = 1200;

                        omitMinX = 670;
                        omitMaxX = 1200;
                        omitMinY = 410;
                        omitMaxY = 640;

                        for( int i = 0; i < amountOfBusMobsToSpawn; i++ ) {
                            Vector2 position = getValidRandomInsideSquare( minX, maxX, minY, maxY, location );
                            GreenSlime busStopMonster = new GreenSlime( position );
                            busStopMonster.currentLocation = location;
                            characters.Add( busStopMonster  );
                        }
                        break;

                    case "Farm":
                        minX = 250;
                        maxX = 4550;
                        minY = 550;
                        maxY = 3750;

                        //Vector2 positionx = getValidRandomInsideSquare( minX, maxX, minY, maxY, location );
                        //amountOfFarmMobsToSpawn = 1;

                        for( int i = 0; i < amountOfFarmMobsToSpawn; i++ ) {
                            Vector2 position = generateRandomValidPosition( location );
                             //positionx = new Vector2( 3294, 1034 );
                            //ModEntry.Log( $"mob to test is at x:{positionx.X} y:{positionx.Y}" );
                            OutDoorShadowBrute mob = new OutDoorShadowBrute( position );
                            mob.currentLocation = location;
                            characters.Add( mob );
                        }

                        break;

                    case "Town":
                        minX = 250;
                        maxX = 4550;
                        minY = 550;
                        maxY = 3750;

                        for( int i = 0; i < amountOfTownMobsToSpawn; i++ ) {
                            Vector2 position = getValidRandomInsideSquare( minX, maxX, minY, maxY, location );
                            Bat bat = new Bat( position );
                            bat.currentLocation = location;
                            characters.Add( bat );
                        }

                        break;

                    case "Beach":
                        minX = 250;
                        maxX = 4550;
                        minY = 550;
                        maxY = 3750;

                        for( int i = 0; i < amountOfBeachMobsToSpawn; i++ ) {
                            Vector2 position = getValidRandomInsideSquare( minX, maxX, minY, maxY, location );
                            NPC beachMonster = generateRandomEasyMob( position );
                            beachMonster.currentLocation = location;
                            characters.Add( beachMonster );
                        }

                        break;

                    case "Mountain":
                        minX = 350;
                        maxX = 4550;
                        minY = 350;
                        maxY = 3750;

                        for( int i = 0; i < amountOfMountainMobsToSpawn; i++ ) {
                            Vector2 position = getValidRandomInsideSquare( minX, maxX, minY, maxY, location );
                            Bat bat = new Bat( position );
                            bat.currentLocation = location;
                            characters.Add( bat );
                        }

                        break;

                    case "Backwoods":
                        minX = 250;
                        maxX = 3000;
                        minY = 250;
                        maxY = 2300;

                        for( int i = 0; i < amountOfBackwoodsMobsToSpawn; i++ ) {
                            Vector2 position = getValidRandomInsideSquare( minX, maxX, minY, maxY, location );
                            NPC backwoodsMonster = generateRandomMediumMob( position );
                            backwoodsMonster.currentLocation = location;
                            characters.Add( backwoodsMonster );
                        }

                        break;

                    case "Forest":
                        minX = 650;
                        maxX = 7000;
                        minY = 640;
                        maxY = 6000;

                        for( int i = 0; i < amountOfForestMobsToSpawn; i++ ) {
                            Vector2 position = getValidRandomInsideSquare( minX, maxX, minY, maxY, location );
                            NPC forestMonster = generateRandomMediumMob( position ) ;
                            forestMonster.currentLocation = location;
                            characters.Add( forestMonster );
                        }

                        break;
                }
            }

            DemiacleUtility.createSafeDelayedDialogue( "It looks like this old house has some bats... great...", 4000  );
            DemiacleUtility.createSafeDelayedDialogue( "Things feel different today...", 50000 );
            DemiacleUtility.createSafeDelayedDialogue( "Its as if the world is not what it once was... odd...", 100000 );
            
            ModEntry.Log( "Mobs finished creating" );
            ModEntry.modData.hasMonstersBeenCreated = true;
            ModEntry.updateModData();
            //putPersistantMonstersInSave();
            
        }

        private Vector2 generateRandomValidPosition( GameLocation location ) {
            int borderBuffer = 2;
            int randomX = Game1.random.Next( borderBuffer, location.map.Layers[ 0 ].LayerWidth - borderBuffer );
            int randomY = Game1.random.Next( borderBuffer, location.map.Layers[ 0 ].LayerHeight - borderBuffer );
            if( PathFinderMap.isTileWalkable( randomX, randomY, location ) ) {
                return new Vector2( randomX * Game1.tileSize, randomY * Game1.tileSize );
            } else {
                return generateRandomValidPosition( location );
            }
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

            int chance = Game1.random.Next( 1, 6);
            switch( chance ) {
                case 1:
                    return new GreenSlime( position );
                case 2:
                    return new Bat( position );
                case 3:
                    return new OutDoorShadowBrute( position );
                case 4:
                    return new OutDoorRockCrab( position );
                case 5:
                    //return new RockGolem( position );
                default:
                    return new Bat( position );
            }
        }

        private bool isTileOccupied( Vector2 v, GameLocation l ) {
            int x = (int) v.X / Game1.tileSize;
            int y = (int) v.Y / Game1.tileSize;
            return (l.getTileIndexAt( x, y, "Buildings" ) != -1 && ( l.doesTileHaveProperty( x, y, "Action", "Buildings" ) == null || !l.doesTileHaveProperty( x, y, "Action", "Buildings" ).Contains( "Door" ) && !l.doesTileHaveProperty( x, y, "Action", "Buildings" ).Contains( "Passable" ) ) || l.isTerrainFeatureAt( x, y ));
        }

        private NPC generateRandomMediumMob( Vector2 position ) {
            int chance = Game1.random.Next( 1, 6 );
            switch( chance ) {
                case 1:
                    return new Bat( position );
                case 2:
                    return new Bat( position );
                case 3:
                    //return new ShadowBrute( position );
                case 4:
                    //return new ShadowBrute( position );
                case 5:
                   // return new ShadowBrute( position );
                default:
                    return new Bat( position );
            }
        }

        private Vector2 getRandomInsideSquare( int minX, int maxX, int minY, int maxY ) {
            return new Vector2( Game1.random.Next( minX, maxX ), Game1.random.Next( minY, maxY ) );
        }

        private Vector2 getRandomInsideSquare( int minX, int maxX, int minY, int maxY, int omitMinX, int omitMaxX, int omitMinY, int omitMaxY ) {
            Vector2 position = new Vector2( Game1.random.Next( minX, maxX ), Game1.random.Next( minY, maxY ) );
            Rectangle rectangle = new Rectangle( omitMinX, omitMinY, omitMaxX - omitMinX, omitMaxY - omitMinY);
            if( isInOmittedSquare( position, rectangle ) ) {
               return getRandomInsideSquare( minX, maxX, minY, maxY, omitMinX, omitMaxX, omitMinY, omitMaxY );
            } else {
                return position;
            }
        }

        private Vector2 getValidRandomInsideSquare( int minX, int maxX, int minY, int maxY, GameLocation location ) {
            Vector2 position = new Vector2( Game1.random.Next( minX, maxX ), Game1.random.Next( minY, maxY ) );
            if( isTileOccupied( position, location ) ) {
                return getValidRandomInsideSquare( minX, maxX, minY, maxY, location );
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
