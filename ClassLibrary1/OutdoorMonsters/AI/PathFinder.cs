using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace Demiacle_SVM.OutdoorMonsters.AI {

    public class PathFinder {
        
        public Node[,] map; //unchecked nodes that are both walkable and unwalkable
        public Node startNode;
        public Node endNode;

        public Queue<Point> foundPath; //needs refactoring its far too coupled
        public List<Node> openNodes = new List<Node>();
        public List<Node> closedNodes = new List<Node>();

        Boolean startAndFinishAreSamePoint;

        public int width;
        public int height;

        public int searchLimit = 40;

        public Boolean pathIsFound = false;

        public PathFinder() { }

        /// <summary>
        /// Create a new instance of PathFinder
        /// </summary>
        public PathFinder( Point start, Point target ) { //TODO ADD A LIMITER TO HOW FAR A PATH CAN SEARCH

            //clone the size and nodes that are walkable from the PathFinderMap instance.
            map = PathFinderMap.Instance.clone();
            width = PathFinderMap.Instance.width;
            height = PathFinderMap.Instance.height;

            this.startNode = new Node( start.X, start.Y, true );
            this.startNode.distanceTraveled = 0;

            map[ start.X, start.Y ] = startNode;

            openNodes.Add( this.startNode );

            this.endNode = new Node( target.X , target.Y, true );
            map[ target.X , target.Y  ] = endNode;

            if( start.Equals( target ) ) {
                startAndFinishAreSamePoint = true;
            }
        }

        /// <summary>
        /// Attempts to find a path from the start location to the end location based on the supplied SearchParameters
        /// </summary>
        /// <returns>A List of Points representing the path. If no path was found, the returned list is empty.</returns>
        public void FindPath() {
            if( startAndFinishAreSamePoint ) {
                foundPath = new Queue<Point>();
                return;
            }

            Queue<Point> path = new Queue<Point>();

            //becin recusive search
            bool success = Search( startNode );
            if( success ) {

                pathIsFound = true;

                // If a path was found, follow the parents from the end node to build a list of points
                Node node = this.endNode;
                while( node.parentNode != null ) {
                    //node.point.X += 1; // shift for 0 indexing
                    //node.point.Y += 1; // shift for 0 indexing
                    path.Enqueue( node.point );
                    node = node.parentNode;
                }
                //path.Enqueue( startNode.point );
                path = new Queue<Point>( path.Reverse() );
            }
            //PathFinderMap.drawPathToConsole( path.ToList(), this );
            foundPath = path;
            PathFinderMap.drawMapWithMinimalDataToConsole( this );
        }

        /// <summary>
        /// Attempts to find a path to the destination node using <paramref name="currentNode"/> as the starting location
        /// </summary>
        /// <param name="currentNode">The node from which to find a path</param>
        /// <returns>True if a path to the destination has been found, otherwise false</returns>
        private bool Search( Node currentNode ) {

            // fail search if it goes beyond the limit
            if( currentNode.distanceTraveled > searchLimit ) {
                Console.Write("this should only fire once per find");
                return false;
            }

            // Move the current node to Closed
            openNodes.Remove( currentNode );
            closedNodes.Add( currentNode );

            calculateAdjacentNodes( currentNode );

            // Sort all checked nodes for their estimated total Distnance (F) so that the shortest possible routes are considered first
            openNodes.Sort( ( node1, node2 ) => node1.estimatedTotalDistance.CompareTo( node2.estimatedTotalDistance ) );
            foreach( var nextNode in openNodes.ToList() ) {

                // Check whether the end node has been reached
                if( nextNode.point.X == endNode.point.X && nextNode.point.Y == endNode.point.Y ) {
                    endNode = nextNode;
                    return true;


                // If not, check the next set of nodes
                } else {
                    if( Search( nextNode ) ) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }

            // Return false if no path is possible
            return false; // compiler complains about this .... why???
        }

        /// <summary>
        /// Returns any nodes that are adjacent to <paramref name="fromNode"/> and may be considered to form the next step in the path
        /// </summary>
        /// <param name="fromNode">The node from which to return the next possible nodes in the path</param>
        /// <returns>A list of next possible nodes in the path</returns>
        private void calculateAdjacentNodes( Node fromNode ) {

            //Console.WriteLine( $"Point is : {fromNode.point.X}, {fromNode.point.Y} and NodeTravelDistance is :{fromNode.distanceTraveled}" );

            Point fromLocation = fromNode.point;

            IEnumerable<Point> nextLocations = new Point[]
            {
            new Point(fromLocation.X, fromLocation.Y-1),
            new Point(fromLocation.X, fromLocation.Y+1),
            new Point(fromLocation.X+1, fromLocation.Y),
            new Point(fromLocation.X-1, fromLocation.Y)
            };

            foreach( var location in nextLocations ) {

                int x = location.X;
                int y = location.Y;

                // Don't check paths that are out of bounds
                if( x < 0 || x > width - 1 || y < 0 || y > height - 1 )
                    continue;

                Node node = map[ x, y ];                
                node.distanceFromTarget = Node.getDistanceBetweenPoints( node.point, endNode.point );

                // Ignore non-walkable nodes unless the target point is there
                if( !node.isWalkable && !( node.point.Equals( endNode.point ) ) )
                    continue;

                // Ignore already-closed nodes
                if( closedNodes.Exists( nodeToCheck => nodeToCheck.point.Equals( node.point ) ) )
                    continue;

                // Ignore open nodes 
                // An open node will never provide a shorter path than is already calculated
                // This check is useful for weighted graphs ( diagonals or costly terrain )
                if( openNodes.Exists( nodeToCheck => nodeToCheck.point.Equals( node.point ) ) ) {

                // If node is valid
                } else {

                    // Set the parent and move to openNodes
                    node.parentNode = fromNode;
                    openNodes.Add( node );

                    // distance between tiles will ALWAYS be 1. No calculation is necessary
                    node.distanceTraveled = node.parentNode.distanceTraveled + 1;
                }
            }
        }

        /// <summary>
        /// The point used for the PathFinder class.
        /// </summary>
        public class Node {
            public float estimatedTotalDistance { get { return this.distanceTraveled + this.distanceFromTarget; } }
            public float distanceTraveled;
            public float distanceFromTarget;

            public Point point;
            public Boolean isWalkable;
            public Node parentNode { get; set; }

            public Node( int x, int y, Boolean isWalkable ) {
                this.point = new Point( x, y );
                this.isWalkable = isWalkable;
            }

            /// <summary>
            /// Calculates the total distance score between two points giving a lower score the closer deltaX is to deltaY
            /// </summary>
            /// <returns></returns>
            public static float getDistanceBetweenPoints( Point location, Point otherLocation ) {
                int deltaX = Math.Abs( otherLocation.X - location.X );
                int deltaY = Math.Abs( otherLocation.Y - location.Y );
                return ( float ) Math.Sqrt( deltaX * deltaX + deltaY * deltaY );
            }

            /// <summary>
            /// Calculates and returns the total actual points between these two location 
            /// </summary>
            public static float getHardDistanceBetweenPoints( Point location, Point otherLocation ) {
                int deltaX = Math.Abs( otherLocation.X - location.X );
                int deltaY = Math.Abs( otherLocation.Y - location.Y );
                return deltaX + deltaY;
            }
        }
        
        /// <summary>
        /// Sets the movement direction of a monster.
        /// </summary>
        /// <param name="outDoorMonster">The monster to alter movement</param>
        internal void setNextDirection( OutDoorMonster outDoorMonster ) {
            
        }

        

    }
}
