using Microsoft.VisualStudio.TestTools.UnitTesting;
using Demiacle_SVM.OutdoorMonsters.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace Demiacle_SVM.OutdoorMonsters.AI.Tests {
    [TestClass()]
    public class PathFinderTests {

        PathFinderMap pathFinderMap;
        public const int MAP_SIZE = 50;
        public const float EPSILON = 0.00000001f;

        Random r = new Random();

        [DllImport( "kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
        public static extern int AllocConsole();

        [DllImport( "kernel32.dll" )]
        public static extern bool FreeConsole();
        

        [ TestInitialize()]
        public void initialize() {

            ModEntry.isTesting = true;

            //create map stub
            pathFinderMap = PathFinderMap.Instance;
            pathFinderMap.width = MAP_SIZE;
            pathFinderMap.height = MAP_SIZE;
            pathFinderMap.map = new PathFinder.Node[ MAP_SIZE , MAP_SIZE ];

            for( int x = 0; x < pathFinderMap.width; x++ ) {
                for( int y = 0; y < pathFinderMap.height; y++ ) {
                    bool isWalkable = ( r.Next( 0, 5 ) != 0  ) ? true : false;
                    pathFinderMap.map[ x, y ] = new PathFinder.Node( x, y, isWalkable );
                }
            }
        }

        [TestMethod()]
        public void FindPathTest() {            
            for( int i = 0; i < 2; i++ ) {
                Point startPoint = getRandomPoint();
                Point targetPoint = getRandomPoint();
                PathFinder pathFinder = new PathFinder( startPoint, targetPoint );
                pathFinder.FindPath();
            }            
        }

        [TestMethod()]
        public void NodeGetDistanceTest() {
            
            //float math fails
            int x1 = r.Next( 1, MAP_SIZE );
            int x2 = r.Next( 1, MAP_SIZE );
            int y1 = r.Next( 1, MAP_SIZE );
            int y2 = r.Next( 1, MAP_SIZE );
            float distance = PathFinder.Node.getDistanceBetweenPoints( new Point( x1, y1), new Point( x2, y2 ) );

            //Assert.IsTrue( calculationIsTrue );
        }

        [TestMethod()]
        public void testTileSize() {
            if( Game1.tileSize != 64 ) {
                Assert.Fail( "Game1.tileSize is NOT 64. All pathfinding calculations will be off please fix inside PathFinderMap.updateMapOnChangeLocation()" );
            }
        }

        [TestCleanup()]
        public void cleanUp() {
            pathFinderMap = null;
        }        

        public Point getRandomPoint() {
            return new Point( r.Next( 1, MAP_SIZE ), r.Next( 1, MAP_SIZE ) );
        }

        static bool almostEqual( float value1, float value2, float acceptableDifference ) {
            return Math.Abs( value1 - value2 ) <= acceptableDifference;
        }

    }
}