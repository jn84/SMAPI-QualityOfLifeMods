using StardewValley;
using xTile.Dimensions;
using static DemiacleSvm.OutdoorMonsters.AI.PathFinder;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using xTile.ObjectModel;

namespace DemiacleSvm.OutdoorMonsters.AI {

    /// <summary>
    /// A singleton class holding map data of walkable/unwalkable tiles.
    /// </summary>
    public class PathFinderMap {

        public int width;
        public int height;

        private GameLocation location;
        public Node[,] map { get; set; } //Array of walkable and unwalkable nodes

        private static readonly PathFinderMap instance = new PathFinderMap();
        public static PathFinderMap Instance {
            get {
                return instance;
            }
        }

        private PathFinderMap() { }

        /// <summary>
        /// Updates data needed for pathfinding. This includes a map of [ x, y ] coordinates, the width and height and the GameLocation.
        /// </summary>
        public void updateMapOnChangeLocation( object sender, EventArgsCurrentLocationChanged e ) {
            //ModEntry.Log( "updateing map on Location changed" );
            location = e.NewLocation;
            width = location.map.DisplayWidth / Game1.tileSize; //dependant on Game1.tileSize
            height = location.map.DisplayHeight / Game1.tileSize;
            map = new Node[ width, height ];
            calculateWalkableTiles();
        }

        /// <summary>
        /// Updates the current map with coordinates declaring which coodinates are walkable for the current map
        /// </summary>
        public void updatePassableTilesOnLocationObjectsChanged( object sender, EventArgsLocationObjectsChanged e ) {
            //ModEntry.Log( "updateing map on Objects in map changed" );
            calculateWalkableTiles();
        }
        
        // TODO still a tiny bit buggy 
        /// <summary>
        /// Calculates a coordinate with the given location and determines if the tile can be walked over.
        /// </summary>
        public static Boolean isTileWalkable( int x, int y, GameLocation location ) {

            // Ignore the 3 worms
            StardewValley.Object obj = null;
            location.objects.TryGetValue( new Vector2( x, y ), out obj );
            if( obj != null && obj.name == "Artifact Spot" ) {
                return true;
            }

            if( !location.isTileLocationTotallyClearAndPlaceable( new Vector2( x, y ) ) ) {

                // Ignore grass tiles
                if( !( location.terrainFeatures.ContainsKey( new Vector2( x, y ) ) &&
                    location.terrainFeatures[ new Vector2( x, y ) ] is StardewValley.TerrainFeatures.Grass ) ) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates the current map with coordinates declaring which coodinates are walkable for the current map
        /// </summary>
        public void calculateWalkableTiles() {
            //int counter = 0;
            //foreach ( KeyValuePair<Vector2,StardewValley.Object> entry in location.objects ){
                
            //}
            //return;

            for( int x = 0; x < width; x++ ) {
                for( int y = 0; y < height; y++ ) {
                    bool isWalkable = true;

                    //isTilePassable checks for hard objects and 
                    Location tileLocation = new Location( x * Game1.tileSize, y * Game1.tileSize );


                    //still not picking up trees or large rocks? some random stuff too i guess

                    //Tree stumps and large rocks are not found here
                    //might have to add ignore code for crop tiles

                    //currently considers worm dig tiles as occupied

                    //PropertyValue propertyValue = ( PropertyValue ) null;
                    //location.map.GetLayer( "Buildings" ).Tiles[ x, y ].TileIndexProperties.TryGetValue( "TreeStump", out propertyValue );
                    string str = location.doesTileHaveProperty( x, y, "TreeStump", "Back" );
                    string str1 = location.doesTileHaveProperty( x, y, "Water", "Back" );
                    Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle( x  , y, 1, 1 );
                    int test = rect.Left;
                    int tesst = rect.Right;
                    int tessts = rect.Top;

                    int tesstss = rect.Bottom;


                    //if( location.isTileOccupiedIgnoreFloors( new Vector2( x , y ) ) || !( location.isTilePassable( new Location( x, y ), Game1.viewport ) )  ) { 
                    //if( !location.isAreaClear(rect) ) { works
                    //if( location.terrainFeatures.ContainsKey( new Vector2( x, y ) )   ) {

                    //}
                    /*
                    

                    if( !location.isTileLocationTotallyClearAndPlaceable( new Vector2(x,y) ) ) {
                        if( !( location.terrainFeatures.ContainsKey( new Vector2( x, y ) ) &&
                            location.terrainFeatures[ new Vector2( x, y ) ] is StardewValley.TerrainFeatures.Grass )   ) {
                            isWalkable = false;
                        }                        
                    }*/

                    if( !isTileWalkable( x, y, location ) ) {
                        isWalkable = false;
                    }

                    map[ x, y ] = new Node( x, y, isWalkable );
                }
            }
        }

        public Node[,] clone() {
            Node[,] newMap = new Node[ PathFinderMap.Instance.width, PathFinderMap.Instance.height ];
            for( int width = 0; width < newMap.GetLength( 0 ); width++ ) {
                for( int height = 0; height < newMap.GetLength( 1 ); height++ ) {
                    bool isWalkable = PathFinderMap.Instance.map[ width, height ].isWalkable;
                    Node node = new PathFinder.Node( width, height, isWalkable );
                    newMap[ width, height ] = node;
                }
            }
            return newMap;
        }

        /// <summary>
        /// Draws a text map to the console. Used for testing purposes
        /// </summary>
        public static void drawMapWithMinimalDataToConsole( PathFinder pathFinder ) {
            List<Node> openNodes = pathFinder.openNodes;
            List<Node> closedNodes = pathFinder.closedNodes;
            Node[,] map = pathFinder.map;
            Node startNode = pathFinder.startNode;
            Node endNode = pathFinder.endNode;

            Console.WriteLine( "- means passable" );
            Console.WriteLine( "0 means blocked" );
            Console.WriteLine( "" );
            for( int y = 0; y < pathFinder.height; y++ ) {
                for( int x = 0; x < pathFinder.width; x++ ) {

                    Node node = map[ x, y ];

                    if( startNode.point.X == x && startNode.point.Y == y ) {
                        System.Console.Write( "█" );
                        continue;
                    }

                    if( openNodes.Exists( openNode => openNode.point.Equals( node.point ) ) ) {
                        System.Console.Write( "$" );
                        continue;
                    }

                    if( closedNodes.Exists( closdedNode => closdedNode.point.Equals( node.point ) ) ) {
                        System.Console.Write( "<" );
                        continue;
                    }

                    if( endNode.point.X == x && endNode.point.Y == y ) {
                        System.Console.Write( "%" );
                        continue;
                    }

                    if( map[ x, y ].isWalkable == true ) {
                        System.Console.Write( "-" );
                    } else {
                        System.Console.Write( "0" );
                    }
                }
                System.Console.Write( "\n" );
            }
        }
        
        /// <summary>
        /// Draws a finalized map with a path and search data to the console. Used for testing purposes
        /// </summary>
        public static void drawPathToConsole( List<Point> path, PathFinder pathFinder ) {
            Console.WriteLine( "PATH IS" ); 
            for( int y = 0; y < pathFinder.height; y++ ) {
                for( int x = 0; x < pathFinder.width; x++ ) {
                    Point checkingPoint = pathFinder.map[ x, y ].point;

                    if( checkingPoint.Equals( pathFinder.startNode.point ) ) {
                        System.Console.Write( "█" );
                        continue;
                    }

                    if( path.Exists( point => point.Equals( checkingPoint ) ) ) {
                        System.Console.Write( "*" );
                        continue;
                    }

                    if( pathFinder.openNodes.Exists( node => node.point.Equals( checkingPoint ) ) ) {
                        System.Console.Write( "$" );
                        continue;
                    }

                    if( checkingPoint.Equals( pathFinder.endNode.point ) ) {
                        System.Console.Write( "." );
                        continue;
                    }

                    if( pathFinder.map[ x, y ].isWalkable == true ) {
                        System.Console.Write( "-" );
                    } else {
                        System.Console.Write( "0" );
                    }
                }
                Console.Write( '\n' );
            }
        }
    }
}
