using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using static Demiacle_SVM.OutdoorMonsters.AI.PathFinder;
using StardewModdingAPI.Events;
using xTile.Tiles;
using xTile.ObjectModel;
using Microsoft.Xna.Framework;

namespace Demiacle_SVM.OutdoorMonsters.AI {
    public class PathFinderMap {

        public int width;
        public int height;

        private GameLocation location;
        public Node[,] map { get; set; } //contains a list of walkable and unwalkable nodes

        private static readonly PathFinderMap instance = new PathFinderMap();

        private PathFinderMap() { }

        public static PathFinderMap Instance {
            get {
                return instance;
            }
        }

        /// <summary>
        /// Updates data needed for pathfinding. This includes a map of [ x, y ] coordinates, the width and height and the GameLocation.
        /// </summary>
        public void updateMapOnChangeLocation( object sender, EventArgsCurrentLocationChanged e ) {
            ModEntry.Log( "updateing map on Location changed" );
            location = e.NewLocation;
            width = location.map.DisplayWidth / Game1.tileSize; //dependant on Game1.tileSize
            height = location.map.DisplayHeight / Game1.tileSize;
            map = new Node[ width , height  ];
            calculateWalkableTiles();
        }

        /// <summary>
        /// Updates the current map with coordinates declaring which coodinates are walkable for the current map
        /// </summary>
        public void updatePassableTilesOnLocationObjectsChanged( object sender, EventArgsLocationObjectsChanged e ) {
            ModEntry.Log( "updateing map on Objects in map changed" );
            calculateWalkableTiles();
        }

        /// <summary>
        /// Updates the current map with coordinates declaring which coodinates are walkable for the current map
        /// </summary>
        public void calculateWalkableTiles() {
            for( int x = 0; x < width; x++ ) {
                for( int y = 0; y < height; y++ ) {
                    bool isWalkable;

                    //isTilePassable checks for hard objects and 
                    Location tileLocation = new Location( x * Game1.tileSize, y * Game1.tileSize );


                    //still not picking up trees or large rocks? some random stuff too i guess

                    //if( !( location.isTilePassable( new Location( x, y ), Game1.viewport ) ) || location.isObjectAt(  x * Game1.tileSize, y * Game1.tileSize )  ) {
                    //Tree stumps and large rocks are not found here
                    //might have to add ignore code for crop tiles
                    if( location.isTileOccupiedIgnoreFloors( new Vector2( x , y ) ) || !( location.isTilePassable( new Location( x, y ), Game1.viewport ) ) ) { 
                        isWalkable = false;
                    } else {
                        isWalkable = true;
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
    }
}
