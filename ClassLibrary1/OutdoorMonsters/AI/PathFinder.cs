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
        //possible refactor
        public Node[,] map; //unchecked nodes that are both walkable and unwalkable
        private Node startNode;
        private Node endNode;
        public List<Point> foundPath; //needs refactoring its far too coupled
        public List<Node> openNodes = new List<Node>();
        public List<Node> closedNodes = new List<Node>();
        Boolean startAndFinishAreSamePoint;
        //private int testCounter = 0;

        /// <summary>
        /// Create a new instance of PathFinder
        /// </summary>
        public PathFinder( Point start, Point target ) {

            //clone the size and nodes that are walkable from the PathFinderMap instance.
            map = PathFinderMap.Instance.clone();


            this.startNode = new Node( start.X, start.Y, true );
            //this.startNode.state = Node.NodeState.open;
            this.startNode.distanceTraveled = 0;
            map[start.X,start.Y] = startNode;

            openNodes.Add( this.startNode );

            this.endNode = new Node( target.X, target.Y, true );
            map[ target.X, target.Y ] = endNode;

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
                foundPath = new List<Point>();
                return;
            }

            // The start node is the first entry in the 'open' list
            List<Point> path = new List<Point>();
            bool success = Search( startNode );
            if( success ) {

                // If a path was found, follow the parents from the end node to build a list of locations
                Node node = this.endNode;
                while( node.parentNode != null ) {
                    path.Add( node.point );
                    node = node.parentNode;
                }

                // Reverse the list so it's in the correct order when returned
                path.Reverse();
            }
            //drawPath( path );
            foundPath = path; ;
        }

        //TODO factor this out
        /// <summary>
        /// Draws a finalized map with a map to the console
        /// </summary>
        private void drawPath( List<Point> path ) {
            Console.WriteLine("PATH IS");
            for( int y = 0; y < PathFinderMap.Instance.height; y++ ) {
                for( int x = 0; x < PathFinderMap.Instance.width; x++ ) {
                    Point checkingPoint = map[ x, y ].point;

                    if( checkingPoint.Equals( startNode.point ) ) {
                        System.Console.Write( "█" );
                        continue;
                    }

                    if( path.Exists( point => point.Equals( checkingPoint ) ) ) {
                        System.Console.Write( "*" );
                        continue;
                    }

                    if( openNodes.Exists( node => node.point.Equals( checkingPoint ) ) ) {
                        System.Console.Write( "$" );
                        continue;
                    }

                    if( map[ x, y ].isWalkable == true ) {
                        System.Console.Write( "-" );
                    } else {
                        System.Console.Write( "0" );
                    }
                }
                Console.Write('\n');
            }
        }

        /// <summary>
        /// Attempts to find a path to the destination node using <paramref name="currentNode"/> as the starting location
        /// </summary>
        /// <param name="currentNode">The node from which to find a path</param>
        /// <returns>True if a path to the destination has been found, otherwise false</returns>
        private bool Search( Node currentNode ) {
            
            // Set the current node to Closed since it cannot be traversed more than once
            //currentNode.state = Node.NodeState.closed;
            openNodes.Remove( currentNode );
            closedNodes.Add( currentNode );
            calculateAdjacentNodes( currentNode );

            // Sort all checked nodes for their estimated totale Distnance (F) so that the shortest possible routes are considered first
            openNodes.Sort( ( node1, node2 ) => node1.estimatedTotalDistance.CompareTo( node2.estimatedTotalDistance ) );
            foreach( var nextNode in openNodes.ToList() ) {

                // Check whether the end node has been reached
                if( nextNode.point.X == endNode.point.X && nextNode.point.Y == endNode.point.Y ) {
                    endNode = nextNode;
                    //Console.WriteLine( "TARGET IS ACHEIVED TARGET IS ACHIEVED" );
                    return true;
                } else {

                    // If not, check the next set of nodes
                    if( Search( nextNode ) ) // recursion
                        return true;
                }
            }

            // The method returns false if this path leads to be a dead end
            return false;
        }

        /// <summary>
        /// Returns any nodes that are adjacent to <paramref name="fromNode"/> and may be considered to form the next step in the path
        /// </summary>
        /// <param name="fromNode">The node from which to return the next possible nodes in the path</param>
        /// <returns>A list of next possible nodes in the path</returns>
        private void calculateAdjacentNodes( Node fromNode ) {

            //Console.WriteLine( $"Point is : {fromNode.point.X}, {fromNode.point.Y} and NodeTravelDistance is :{fromNode.distanceTraveled}" );
            //List<Node> walkableNodes = new List<Node>();
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
                if( x < 0 || x > PathFinderMap.Instance.width - 1 || y < 0 || y > PathFinderMap.Instance.height - 1)
                    continue;

                Node node = map[ x, y ];
                //Console.WriteLine( $"testing point x:{x}  y:{y}" );
                node.distanceFromTarget = Node.getDistanceBetweenPoints( node.point, endNode.point );
                

                //drawMap( location );

                // Ignore non-walkable nodes unless the target point is there
                if( !node.isWalkable && !(node.point.Equals(endNode.point)) )
                    continue;

                // Ignore already-closed nodes
                if( closedNodes.Exists( nodeToCheck => nodeToCheck.point.Equals( node.point ) ) )
                    continue;

                // ignore open nodes - an open node will never be a shorter path than it already is however this check is useful for weighted graphs (diagonals or slower terrain)
                if( openNodes.Exists( nodeToCheck => nodeToCheck.point.Equals( node.point ) ) ) {
                    

                } else {

                    // If it's untested, set the parent and flag it as 'Open' for consideration
                    node.parentNode = fromNode;
                    //node.state = Node.NodeState.open;
                    
                    openNodes.Add( node );

                    // distance between tiles will ALWAYS be 1 - no calculation necessary
                    node.distanceTraveled = node.parentNode.distanceTraveled + 1; 
                }
            }
        }

        /// <summary>
        /// The point used for A* pathfinding
        /// </summary>
        public class Node {
            public float estimatedTotalDistance { get { return this.distanceTraveled + this.distanceFromTarget; } }
            public float distanceTraveled;
            public float distanceFromTarget;
            public Point point;
            public Boolean isWalkable;
            public Node parentNode { get; set; }

            //public NodeState state = NodeState.untested;
            public enum NodeState { open, closed, untested }

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

        
        //TODO factor this out
        /// <summary>
        /// Draws a text map to the console for testing purposed... 
        /// </summary>
        public void drawMap( Point point ) {
            
            Console.WriteLine( "- means passable" );
            Console.WriteLine( "0 means blocked" );
            Console.WriteLine( "" );
            for( int y = 0; y < PathFinderMap.Instance.height; y++ ) {
                for( int x = 0; x < PathFinderMap.Instance.width; x++ ) {

                    Node node = map[ x, y ];

                    if( startNode.point.X == x && startNode.point.Y == y ) {
                        System.Console.Write( "█ " );
                        continue;
                    }

                    if( point.X == x && point.Y == y ) {
                        System.Console.Write( "* " );
                        continue;
                    }

                    if( openNodes.Exists( openNode => openNode.point.Equals( node.point ) ) ) {
                        System.Console.Write( "$ " );
                        continue;
                    }

                    if( closedNodes.Exists( closdedNode => closdedNode.point.Equals( node.point ) ) ) {
                        System.Console.Write( "< " );
                        continue;
                    }

                    if( endNode.point.X == x && endNode.point.Y == y ) {
                        System.Console.Write( "% " );
                        continue;
                    }

                    if( map[ x, y ].isWalkable == true ) {
                        System.Console.Write( "- " );
                    } else {
                        System.Console.Write( "0 " );
                    }
                }
                System.Console.Write( "\n" );
            }
        }

        internal void setNextDirection( OutDoorMonster outDoorMonster ) {

            Boolean isMovingRight = foundPath[ 0 ].X < foundPath[ 1 ].X;
            Boolean isMovingLeft = foundPath[ 0 ].X > foundPath[ 1 ].X;
            Boolean isMovingUp = foundPath[ 0 ].Y > foundPath[ 1 ].Y;
            Boolean isMovingDown = foundPath[ 0 ].Y < foundPath[ 1 ].Y;

            outDoorMonster.SetMovingDown( isMovingDown );
            outDoorMonster.SetMovingUp( isMovingUp );
            outDoorMonster.SetMovingLeft( isMovingLeft );
            outDoorMonster.SetMovingRight( isMovingRight );


        }
    }
}
