using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace Demiacle.ImprovedQualityOfLife {

    //TODO Speed mod interferes with buffs atm... we need to make it work WITH buffs
    internal class SpeedModOnRoads {
        private const int fastSpeed = 1;
        private const int fasterSpeed = 2;
        private const int fastestSpeed = 3;

        private bool isOnFloor = false;
        private int roadSpeedIncrease = 0;

        public SpeedModOnRoads() {
            GameEvents.SecondUpdateTick += checkTileForRoad;
            GameEvents.QuarterSecondTick += forcePlayerToSpeed;
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

            if( Game1.currentLocation.terrainFeatures.ContainsKey( new Vector2( tileAtLocation.X, tileAtLocation.Y) ) &&
                Game1.currentLocation.terrainFeatures[ new Vector2( tileAtLocation.X, tileAtLocation.Y ) ] is Flooring  ) {
                
                var flooring = ( Flooring ) Game1.currentLocation.terrainFeatures[ new Vector2( tileAtLocation.X, tileAtLocation.Y ) ];

                //currently 10 exist
                switch( flooring.whichFloor ) {

                    case Flooring.wood: //Wood floor
                    case Flooring.stone: //Stone floor
                    case Flooring.colored_cobblestone: //crystal path      
                    case Flooring.iceTile: //Crystal flooring
                    case Flooring.boardwalk: //Wood path
                    case Flooring.ghost: //Weathered floor
                    case Flooring.straw: //straw
                    case Flooring.gravel: //gravel                    
                    case Flooring.cobblestone: //cobblestone
                    case Flooring.steppingStone: //steppingStone
                        roadSpeedIncrease = 7;
                        break;

                    default:
                        roadSpeedIncrease = 0;
                        break;

                }
            } else {
                roadSpeedIncrease = 0;
            }

            if( roadSpeedIncrease > 0 ) {
                isOnFloor = true;
            }
            
        }

        internal void forcePlayerToSpeed( object sender, EventArgs e ) {
            if( isOnFloor && Game1.CurrentEvent == null) {
                Game1.player.speed = 7;
            } else {
                Game1.player.speed = 5;
            }
        }

    }
}