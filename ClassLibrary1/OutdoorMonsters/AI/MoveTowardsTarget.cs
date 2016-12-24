using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Demiacle_SVM.OutdoorMonsters.AI {

    /// <summary>
    /// Move in a direction without considering object collision
    /// </summary>
    public class MoveTowardsTarget : MovementType {        
        public override void calculateNextMovement( OutDoorMonster outDoorMonster ) {
            Microsoft.Xna.Framework.Rectangle playerBoundingBox = Game1.player.GetBoundingBox();
            int playerX = playerBoundingBox.Center.X;
            int playerY = playerBoundingBox.Center.Y;

            int monsterX = outDoorMonster.GetBoundingBox().Center.X;
            int monsterY = outDoorMonster.GetBoundingBox().Center.Y;

            int distanceX = Math.Abs( playerX - monsterX );
            int distanceY = Math.Abs( playerY - monsterY );

            if( distanceX + distanceY > Game1.tileSize * outDoorMonster.distanceToFindTarget ) {
                return;
            }

            //decide to move horizontal or vertical based on distance of x and y
            if( distanceX > distanceY ) {
                if( playerX - monsterX < 0 )
                    outDoorMonster.SetMovingLeft( true );
                else
                    outDoorMonster.SetMovingRight( true );
            } else {

                if( playerY - monsterY < 0 )
                    outDoorMonster.SetMovingUp( true );
                else
                    outDoorMonster.SetMovingDown( true );
            }
        }
    }
}
