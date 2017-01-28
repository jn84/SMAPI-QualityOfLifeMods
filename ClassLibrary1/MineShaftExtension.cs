using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.ObjectModel;

namespace DemiacleSvm {
    class MineShaftExtension : MineShaft{
        FieldInfo isMonsterArea = typeof( MineShaftExtension ).BaseType.GetField( "isMonsterArea", BindingFlags.Instance | BindingFlags.NonPublic );
        FieldInfo isSlimeArea = typeof( MineShaftExtension ).BaseType.GetField( "isSlimeArea", BindingFlags.Instance | BindingFlags.NonPublic );
        FieldInfo loadedDarkArea = typeof( MineShaftExtension ).BaseType.GetField( "loadedDarkArea", BindingFlags.Instance | BindingFlags.NonPublic );
        FieldInfo mineLoaderField = typeof( MineShaftExtension ).BaseType.GetField( "mineLoader", BindingFlags.Instance | BindingFlags.NonPublic );
        FieldInfo mineRandom = typeof( MineShaftExtension ).BaseType.GetField( "mineRandom", BindingFlags.Instance | BindingFlags.NonPublic );
        FieldInfo ElevatorLightSpot = typeof( MineShaftExtension ).BaseType.GetField( "ElevatorLightSpot", BindingFlags.Instance | BindingFlags.NonPublic );
        FieldInfo timeUntilElevatorLightUp = typeof( MineShaftExtension ).BaseType.GetField( "timeUntilElevatorLightUp", BindingFlags.Instance | BindingFlags.NonPublic );

        public MineShaftExtension() {

            ModEntry.Log( "creating hijacker" );
            //intercept this method
            //MethodInfo methodToReplace = typeof( MineShaft ).GetMethod( "loadLevel", BindingFlags.Instance | BindingFlags.Public );
            // MethodInfo methodToInject = typeof( MineShaftExtension ).GetMethod( "loadLevel", BindingFlags.Instance | BindingFlags.Public );
            //RuntimeHelpers.PrepareMethod( methodToReplace.MethodHandle );
            //RuntimeHelpers.PrepareMethod( methodToInject.MethodHandle );
        }

        //will have to inject this
        public void loadLevel( int level ) {
            ModEntry.Log("this shit is hijacked aww yea");
            this.isMonsterArea.SetValue( this, false );
            this.isSlimeArea.SetValue( this, false );
            this.loadedDarkArea.SetValue( this, false );


            LocalizedContentManager mineLoader = ( LocalizedContentManager ) mineLoaderField.GetValue( this );
            Random mineRandom = ( Random ) this.mineRandom.GetValue( this );
            mineLoader.Unload();
            mineLoader.Dispose();
            mineLoader = Game1.content.CreateTemporary();
            int num = ( level % 40 % 20 != 0 || level % 40 == 0 ? ( level % 10 == 0 ? 10 : level ) : 20 ) % 40;
            if( level == 120 )
                num = 120;
            if( this.getMineArea( level ) == 121 ) {
                num = ( mineRandom.Next( 40 ) );
                while( num % 5 == 0 )
                    num = mineRandom.Next( 40 );
            }
            this.map = mineLoader.Load<Map>( "Maps\\Mines\\" + ( object ) num );
            Random random = new Random( ( int ) Game1.stats.DaysPlayed + level + ( int ) Game1.uniqueIDForThisGame / 2 );
            if( ( !Game1.player.hasBuff( 23 ) || this.getMineArea( -1 ) == 121 ) && ( random.NextDouble() < 0.05 && num % 5 != 0 ) && ( num % 40 > 5 && num % 40 < 30 && num % 40 != 19 ) ) {
                if( random.NextDouble() < 0.5 )
                    this.isMonsterArea.SetValue( this, true );
                else
                    this.isSlimeArea.SetValue( this, false );
                Game1.showGlobalMessage( Game1.content.LoadString( random.NextDouble() < 0.5 ? "Strings\\Locations:Mines_Infested" : "Strings\\Locations:Mines_Overrun" ) );
            }
            if( this.getMineArea( this.nextLevel ) != this.getMineArea( this.mineLevel ) || this.mineLevel == 120 )
                Game1.changeMusicTrack( "none" );
            if( ( Boolean ) this.isSlimeArea.GetValue( this ) ) {
                this.map.TileSheets[ 0 ].ImageSource = "Maps\\Mines\\mine_slime";
                this.map.LoadTileSheets( Game1.mapDisplayDevice );
            } else if( this.getMineArea( -1 ) == 0 || this.getMineArea( -1 ) == 10 || this.getMineArea( this.nextLevel ) != 0 && this.getMineArea( this.nextLevel ) != 10 ) {
                if( this.getMineArea( this.nextLevel ) == 40 ) {
                    this.map.TileSheets[ 0 ].ImageSource = "Maps\\Mines\\mine_frost";
                    if( this.nextLevel >= 70 ) {
                        this.map.TileSheets[ 0 ].ImageSource += "_dark";
                        this.loadedDarkArea.SetValue( this, true );
                    }
                    this.map.LoadTileSheets( Game1.mapDisplayDevice );
                } else if( this.getMineArea( this.nextLevel ) == 80 ) {
                    this.map.TileSheets[ 0 ].ImageSource = "Maps\\Mines\\mine_lava";
                    if( this.nextLevel >= 110 && this.nextLevel != 120 ) {
                        this.map.TileSheets[ 0 ].ImageSource += "_dark";
                        this.loadedDarkArea.SetValue( this, true );
                    }
                    this.map.LoadTileSheets( Game1.mapDisplayDevice );
                } else if( this.getMineArea( this.nextLevel ) == 121 ) {
                    this.map.TileSheets[ 0 ].ImageSource = "Maps\\Mines\\mine_desert";
                    if( num % 40 >= 30 ) {
                        this.map.TileSheets[ 0 ].ImageSource += "_dark";
                        this.loadedDarkArea.SetValue( this, true );
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

        private void prepareElevator() {
            Point tile = Utility.findTile( ( GameLocation ) this, 80, "Buildings" );
            this.ElevatorLightSpot.SetValue( this, tile );
            if( tile.X < 0 )
                return;
            if( this.canAdd( 3, 0 ) ) {
                this.timeUntilElevatorLightUp.SetValue( this, 1500 );
                this.updateMineLevelData( 3, 1 );
            } else
                this.setMapTileIndex( tile.X, tile.Y, 48, "Buildings", 0 );
        }

        private bool canAdd( int typeOfFeature, int numberSoFar ) {
            if( this.permanentMineChanges.ContainsKey( this.mineLevel ) ) {
                switch( typeOfFeature ) {
                    case 0:
                        return this.permanentMineChanges[ this.mineLevel ].platformContainersLeft > numberSoFar;
                    case 1:
                        return this.permanentMineChanges[ this.mineLevel ].chestsLeft > numberSoFar;
                    case 2:
                        return this.permanentMineChanges[ this.mineLevel ].coalCartsLeft > numberSoFar;
                    case 3:
                        return this.permanentMineChanges[ this.mineLevel ].elevator == 0;
                }
            }
            return true;
        }
    }
}
