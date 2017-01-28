using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DemiacleSvm.OutdoorMonsters.AI {

    /// <summary>
    /// Finds a path to the target and sets the movement towards that path
    /// </summary>
    public class PathFinding : MovementType {
        [XmlIgnore]
        public PathFinder pathFinder;

        float targetPositionX = 0;
        float targetPositionY = 0;

        float epsilon = 2f;

        // TODO bug - sometimes tiles are found as unwalkable even though they are free
        // TODO bug - sometimes mobs get stuck and are unable to move. 

        // this is fired every few moments
        public override void calculateNextMovement( OutDoorMonster outDoorMonster ) {


            outDoorMonster.target = Game1.player;

            Point monsterPoint = new Point( outDoorMonster.getTileX(), outDoorMonster.getTileY() );
            Point targetPoint = new Point( outDoorMonster.target.getTileX(), outDoorMonster.target.getTileY() );

            // Move in a random direction if player is not within distance
            if( PathFinder.Node.getHardDistanceBetweenPoints( monsterPoint, targetPoint ) > outDoorMonster.distanceToFindTarget ) {
                outDoorMonster.setRandomDirection();
                return;
            }
            
            pathFinder = new PathFinder( monsterPoint, targetPoint );
            pathFinder.FindPath();

            // Move in a random direction if path not found
            if( pathFinder.pathIsFound == false ) {
                outDoorMonster.setRandomDirection();
                return;
            }
            
            // Ignore path and move torwards target if close enough
            if( pathFinder.foundPath.Count <= 1 ) {
                targetPositionX = outDoorMonster.target.getTileX() * Game1.tileSize;
                targetPositionY = outDoorMonster.target.getTileY() * Game1.tileSize;
               
                return;
            }

            setGamePositionCoordinatesOfTargetTile();

            //ModEntry.Log( $"CurrentPosition is :X{outDoorMonster.getTileX()} :y{outDoorMonster.getTileY()}" );
            //ModEntry.Log( $"Attempting to move towards :X{pathFinder.foundPath.Peek().X} :y{pathFinder.foundPath.Peek().Y}" );
            //ModEntry.Log( $"and" );
            //ModEntry.Log( $"CurrentPosition is :X{outDoorMonster.position.X} :y{outDoorMonster.position.Y}" );
            //ModEntry.Log( $"Attempting to move towards :X{pathFinder.foundPath.Peek().X * Game1.tileSize} :y{pathFinder.foundPath.Peek().Y *Game1.tileSize}" );                  
        }

        private void setGamePositionCoordinatesOfTargetTile() {
            // Sometimes the update is fired before a path is found and this will fix... alternative is try catch
            if( pathFinder.foundPath == null || pathFinder.foundPath.Count < 1 ) {
                return;
            }

            targetPositionX = pathFinder.foundPath.Peek().X * Game1.tileSize;
            targetPositionY = pathFinder.foundPath.Peek().Y * Game1.tileSize;
        }

        // Moves the target along a valid path
        public override void updateOnEveryTick( OutDoorMonster outDoorMonster ) {

            // Do nothing if path has not been searched for
            if( pathFinder == null )
                return;
            
            // Do nothing if no path exists
            if( pathFinder.pathIsFound == false )
                return;

            // Do nothing if path is dequeued
            if( pathFinder.foundPath.Count < 1 ) {
                return;
            }

            float distanceFromCenterX = Math.Abs( outDoorMonster.position.X - targetPositionX );
            float distanceFromCenterY = Math.Abs( outDoorMonster.position.Y - targetPositionY );            
            
            // Move outDoorMonster towards center of tile
            // Horizontal axis is always calculated first
            if( distanceFromCenterX > epsilon ) {
                if( targetPositionX > outDoorMonster.position.X ) {
                    outDoorMonster.SetMovingOnlyRight();
                } else {
                    outDoorMonster.SetMovingOnlyLeft();
                }
                return;
            }
            
            if( distanceFromCenterY > epsilon ) {
                if( targetPositionY > outDoorMonster.position.Y ) {
                    outDoorMonster.SetMovingOnlyDown();
                } else {
                    outDoorMonster.SetMovingOnlyUp();
                }
                return;
            }
            
            pathFinder.foundPath.Dequeue();
            setGamePositionCoordinatesOfTargetTile();        
        }

    }
}
