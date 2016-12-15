using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace Demiacle_SVM {

    //TODO Speed mod interferes with buffs atm... we need to make it work WITH buffs
    internal class SpeedMod {
        private const int fastSpeed = 1;
        private const int fasterSpeed = 2;
        private const int fastestSpeed = 3;
        
        private int roadSpeedIncrease = 0;
        private string bootDurabilityString = ", durability : 20000";
        private int durabilityDecay = 2;
        private string wornOutBootsString = "Worn out boots";

        private const string fast = "Run Fast";
        private const string faster = "Run Faster";
        private const string fastest = "Run Fastest";
        

        public SpeedMod() {
        }

        /// <summary>
        /// Checks the current tile the player and if it is a flooring will increase movement speed
        /// </summary>
        internal void checkTileForRoad( object sender, EventArgs e ) {
            if( Game1.currentLocation == null ) {
                return;
            }
            Farmer player = Game1.player;
            Point tileAtLocation = player.getTileLocationPoint();
            //Game1.currentLocation.getTileIndexAt( locale );
            //foreach( KeyValuePair<Vector2, StardewValley.TerrainFeatures.TerrainFeature> keyPair in Game1.currentLocation.terrainFeatures ) {
            //    modEntry.Monitor.Log( $"Location contains key  X:{keyPair.Key.X}      y:{keyPair.Key.Y}   value:{keyPair.Value}" );
            //}

            //modEntry.Monitor.Log( $"Location of farmer to tilesheet is X:{ tileAtLocation.X} and Y:{ tileAtLocation.Y}" );
            //int speedIncrease = 5;
            if( Game1.currentLocation.terrainFeatures.ContainsKey( new Vector2( tileAtLocation.X, tileAtLocation.Y) ) &&
                Game1.currentLocation.terrainFeatures[ new Vector2( tileAtLocation.X, tileAtLocation.Y ) ] is Flooring  ) {
                
                Flooring flooring = ( Flooring ) Game1.currentLocation.terrainFeatures[ new Vector2( tileAtLocation.X, tileAtLocation.Y ) ];
                //modEntry.Monitor.Log( $"Current floor type is: {flooring.whichFloor}");
                //modEntry.Monitor.Log( $"at X:{tileAtLocation.X} Y: {tileAtLocation.Y}");

                //modEntry.Monitor.Log( $"You are currently on flooring" );
                //currently 10 exist
                switch( flooring.whichFloor ) {
                    case Flooring.wood: //Wood floor
                    case Flooring.stone: //Stone floor
                        roadSpeedIncrease = fasterSpeed;
                        break;

                    case Flooring.colored_cobblestone: //crystal path      
                    case Flooring.iceTile: //Crystal flooring
                        roadSpeedIncrease = fastestSpeed;
                        break;
                    

                    case Flooring.boardwalk: //Wood path
                    case Flooring.ghost: //Weathered floor
                    case Flooring.straw: //straw
                    case Flooring.gravel: //gravel                    
                    case Flooring.cobblestone: //cobblestone
                    case Flooring.steppingStone: //steppingStone
                        roadSpeedIncrease = fastSpeed;
                        break;
                    default:
                        roadSpeedIncrease = 0;
                        break;
                }
            } else {
                roadSpeedIncrease = 0;
            }
            
        }

        /// <summary>
        /// Changes the description of all boots to either Fast, Faster, or Faster
        /// This will be used to calculate speed buff
        /// </summary>
        internal void onInvChange( object sender, EventArgsInventoryChanged e ) {

            // changes description based on boot stats - better stats mean faster boots
            for( int i = 0; i < e.Inventory.Count; i++ ) {
                if( e.Inventory[ i ] is Boots ) {
                    Boots boot = ( Boots ) e.Inventory[ i ];
                    if( boot.description == wornOutBootsString ) {
                        return;
                    }

                    int speedBonus = boot.defenseBonus + boot.immunityBonus;

                    string firstBootWord = boot.description.Split( new string[] { "," }, StringSplitOptions.None )[0];
                    if ( firstBootWord == fast  || firstBootWord  == faster || firstBootWord  == fastest ) {
                        return;
                    }
                    // not implemented yet
                    //boot.defenseBonus = 0;
                    //boot.immunityBonus = 0;
                    
                    string tempBootDescription = "";
                    if( speedBonus < 2 ) {
                        tempBootDescription = fast;
                    } else if( 2 <= speedBonus || speedBonus <= 4 ) {
                        tempBootDescription = faster;
                    } else if( speedBonus > 4 ) {
                        tempBootDescription = fastest;
                    }
                    boot.description = tempBootDescription + bootDurabilityString;
                }
            }
        }

        internal void forcePlayerToSpeed( object sender, EventArgs e ) {
            int addedBootSpeed = getBootSpeed();
            Game1.player.addedSpeed = Math.Min( addedBootSpeed + roadSpeedIncrease, 5 );
            //modEntry.Monitor.Log($"Current speed mod is :{Game1.player.addedSpeed}");
            checkIfBootIsWorn();
        }

        private void checkIfBootIsWorn() {
            Farmer player = Game1.player;

            if( !validateBoots() ) {
                return;
            }

            if( player.isMoving() ) {
                string tempDescription = player.boots.description;
                string currentDurability = player.boots.description.Split( new string[] { " : " }, StringSplitOptions.None )[ 1 ];
                //modEntry.Monitor.Log( $"durability is {currentDurability}" );
                int nextDurability = Int32.Parse( currentDurability );
                nextDurability -= durabilityDecay;
                if( nextDurability < 0 ) {
                    player.boots.description = wornOutBootsString;
                    return;
                }

                player.boots.description = tempDescription.Replace( currentDurability, nextDurability.ToString() );
            }
        }

        private Boolean validateBoots() {
            Farmer player = Game1.player;

            if( player == null || player.boots == null || player.boots.description == wornOutBootsString ) {
                return false;
            }

            string firstBootWord = player.boots.description.Split( new string[] { "," }, StringSplitOptions.None )[ 0 ];
            if( firstBootWord != fast && firstBootWord != faster && firstBootWord != fastest ) {
                return false;
            }

            return true;
        }

        private int getBootSpeed() {
            if ( !validateBoots() ) {
                return 0;
            }

            string firstBootWord = Game1.player.boots.description.Split( new string[] { "," }, StringSplitOptions.None )[ 0 ];

            switch( firstBootWord ) {
                case fast:
                    return 1;
                case faster:
                    return 2;
                case fastest:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}