using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Demiacle_SVM {
    internal class MineShaftMod {
        private Boolean hasIntreceptedMineShaft = false;

        public MineShaftMod() {
            
            LocationEvents.CurrentLocationChanged += onLocationChange;
            GraphicsEvents.OnPreRenderGuiEvent += OnPreRenderGuiEvent;
        }

        internal void onLocationChange( object sender, EventArgs e ) {
            /*
            if( !( Game1.currentLocation is MineShaft ) ) {
                return;
            }

            if( hasIntreceptedMineShaft ) {
                hasIntreceptedMineShaft = false;
                return;
            }

            modEntry.Monitor.Log("current location is mineshaft and is being redirected");
            MineShaft mineShaft = ( MineShaft ) Game1.currentLocation;
            int level = mineShaft.mineLevel;
            mineShaft = new MineShaftExtension( modEntry );
            mineShaft.setNextLevel( level );
            Game1.warpFarmer( "UndergroundMine", 6, 6, 2  );

            */
            //MethodInfo dynMethod = mineShaft.GetType().GetMethod( "prepareElevator", BindingFlags.NonPublic | BindingFlags.Instance );
            //dynMethod.Invoke( mineShaft, new object[] {} );

            // after LoadLevel is called invoke prepareelevator
           // mineShaft = new MineShaft();
            //mineShaft.setNextLevel( level  );

            //mineShaft.prepareElevator();
            //Game1.warpFarmer( "UndergroundMine", 6, 6, 2 );

        }

    

    internal void OnPreRenderGuiEvent( object sender, EventArgs e ) {
            if( Game1.activeClickableMenu is MineElevatorMenu) {
                Game1.activeClickableMenu = new ElevatorMenuMod();
            }
        }
        /*
        public class Injection {
            public static void install( int funcNum ) {
                MethodInfo methodToReplace = typeof( MineShaft ).GetMethod( "loadLevel", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
                MethodInfo methodToInject = typeof( Injection ).GetMethod( "loadLevel", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
                RuntimeHelpers.PrepareMethod( methodToReplace.MethodHandle );
                RuntimeHelpers.PrepareMethod( methodToInject.MethodHandle );

                unsafe {
                    if( IntPtr.Size == 4 ) {
                        int* inj = ( int* ) methodToInject.MethodHandle.Value.ToPointer() + 2;
                        int* tar = ( int* ) methodToReplace.MethodHandle.Value.ToPointer() + 2;
                        
                    Console.WriteLine("\nVersion x84 Release\n");
                    *tar = *inj;
                    } else {

                        long* inj = ( long* ) methodToInject.MethodHandle.Value.ToPointer() + 1;
                        long* tar = ( long* ) methodToReplace.MethodHandle.Value.ToPointer() + 1;
                        Console.WriteLine("\nVersion x64 Release\n");
                        *tar = *inj;
                    }
                }
            }

            private void loadLevel( int level ) {
                this.isMonsterArea = false;
                this.isSlimeArea = false;
                this.loadedDarkArea = false;
                this.mineLoader.Unload();
                this.mineLoader.Dispose();
                this.mineLoader = Game1.content.CreateTemporary();
                int num = ( level % 40 % 20 != 0 || level % 40 == 0 ? ( level % 10 == 0 ? 10 : level ) : 20 ) % 40;
                if( level == 120 )
                    num = 120;
                if( this.getMineArea( level ) == 121 ) {
                    num = this.mineRandom.Next( 40 );
                    while( num % 5 == 0 )
                        num = this.mineRandom.Next( 40 );
                }
                this.map = this.mineLoader.Load<Map>( "Maps\\Mines\\" + ( object ) num );
                Random random = new Random( ( int ) Game1.stats.DaysPlayed + level + ( int ) Game1.uniqueIDForThisGame / 2 );
                if( ( !Game1.player.hasBuff( 23 ) || this.getMineArea( -1 ) == 121 ) && ( random.NextDouble() < 0.05 && num % 5 != 0 ) && ( num % 40 > 5 && num % 40 < 30 && num % 40 != 19 ) ) {
                    if( random.NextDouble() < 0.5 )
                        this.isMonsterArea = true;
                    else
                        this.isSlimeArea = true;
                    Game1.showGlobalMessage( Game1.content.LoadString( random.NextDouble() < 0.5 ? "Strings\\Locations:Mines_Infested" : "Strings\\Locations:Mines_Overrun" ) );
                }
                if( this.getMineArea( this.nextLevel ) != this.getMineArea( this.mineLevel ) || this.mineLevel == 120 )
                    Game1.changeMusicTrack( "none" );
                if( this.isSlimeArea ) {
                    this.map.TileSheets[ 0 ].ImageSource = "Maps\\Mines\\mine_slime";
                    this.map.LoadTileSheets( Game1.mapDisplayDevice );
                } else if( this.getMineArea( -1 ) == 0 || this.getMineArea( -1 ) == 10 || this.getMineArea( this.nextLevel ) != 0 && this.getMineArea( this.nextLevel ) != 10 ) {
                    if( this.getMineArea( this.nextLevel ) == 40 ) {
                        this.map.TileSheets[ 0 ].ImageSource = "Maps\\Mines\\mine_frost";
                        if( this.nextLevel >= 70 ) {
                            this.map.TileSheets[ 0 ].ImageSource += "_dark";
                            this.loadedDarkArea = true;
                        }
                        this.map.LoadTileSheets( Game1.mapDisplayDevice );
                    } else if( this.getMineArea( this.nextLevel ) == 80 ) {
                        this.map.TileSheets[ 0 ].ImageSource = "Maps\\Mines\\mine_lava";
                        if( this.nextLevel >= 110 && this.nextLevel != 120 ) {
                            this.map.TileSheets[ 0 ].ImageSource += "_dark";
                            this.loadedDarkArea = true;
                        }
                        this.map.LoadTileSheets( Game1.mapDisplayDevice );
                    } else if( this.getMineArea( this.nextLevel ) == 121 ) {
                        this.map.TileSheets[ 0 ].ImageSource = "Maps\\Mines\\mine_desert";
                        if( num % 40 >= 30 ) {
                            this.map.TileSheets[ 0 ].ImageSource += "_dark";
                            this.loadedDarkArea = true;
                        }
                        this.map.LoadTileSheets( Game1.mapDisplayDevice );
                        if( this.nextLevel >= 145 && Game1.player.hasQuest( 20 ) && !Game1.player.hasOrWillReceiveMail( "QiChallengeComplete" ) ) {
                            Game1.player.completeQuest( 20 );
                            Game1.addMailForTomorrow( "QiChallengeComplete", false, false );
                        }
                    }
                }
                if( !this.map.TileSheets[ 0 ].TileIndexProperties[ 165 ].ContainsKey( "Diggable" ) )
                    this.map.TileSheets[ 0 ].TileIndexProperties[ 165 ].Add( "Diggable", new PropertyValue( "true" ) );
                if( !this.map.TileSheets[ 0 ].TileIndexProperties[ 181 ].ContainsKey( "Diggable" ) )
                    this.map.TileSheets[ 0 ].TileIndexProperties[ 181 ].Add( "Diggable", new PropertyValue( "true" ) );
                if( !this.map.TileSheets[ 0 ].TileIndexProperties[ 183 ].ContainsKey( "Diggable" ) )
                    this.map.TileSheets[ 0 ].TileIndexProperties[ 183 ].Add( "Diggable", new PropertyValue( "true" ) );
                this.mineLevel = this.nextLevel;
                if( this.nextLevel > this.lowestLevelReached ) {
                    this.lowestLevelReached = this.nextLevel;
                    Game1.player.deepestMineLevel = this.nextLevel;
                }
                if( this.mineLevel % 5 != 0 || this.getMineArea( -1 ) == 121 )
                    return;
                this.prepareElevator();
            }
        }

        internal void onLoad( object sender, EventArgsLoadedGameChanged e ) {
             //Injection shit
        }
        */
    }
}