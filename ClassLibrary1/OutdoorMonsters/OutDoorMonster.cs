using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Monsters;
using Microsoft.Xna.Framework;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using xTile.Dimensions;
using System.Threading;
using StardewModdingAPI.Events;
using Demiacle_SVM.OutdoorMonsters.AI;

namespace Demiacle_SVM.OutdoorMonsters {

    

    /// <summary>
    /// A layer for easily modifiable properties and types of movement to the monster class.
    /// </summary>
    public class OutDoorMonster : Monster {        
        public Character target = Game1.player;
        public float defaultAlpha = 1;
        public float alpha;
        public int knockbackSpeed = 10;
        public int slideAnimationTimer = 100;
        public MovementType moveType = new HoldStill();
        private MovementType previousMoveType;
        private MovementType nextMoveType;
        public float moveSpeedExtension = MoveSpeedExtension.medium; //allows slower movement speeds
        public float amountToFadePerGameTick = 0;
        public Boolean isGettingKnockedBack = false;
        protected PathFinder pathFinder;
        public int distanceToFindTarget = 20;

        public static OutDoorMonster thisMonster;
        
        //TODO use these enums as an easy means to change behavior of how fast a monster reacts and changes course
        public enum timeToDecideNextMovement { fast, medium, slow } 
        

        public struct MoveSpeedExtension {
            public const float fast = 0.5f;
            public const float medium = 1;
            public const float slow = 5;
            public const float slower = 10; 
        }

        //needed for serialization just leave empty
        public OutDoorMonster() : base() {

        }

        public OutDoorMonster( string name, Vector2 position )
            : base( name, position ) {
            thisMonster = this;
            alpha = defaultAlpha;
        }

        internal void shiftDefaultLocation() {
            int chance1 = Game1.random.Next( -64, 64 );
            int chance2 = Game1.random.Next( -64, 64 );
            float newX = DefaultPosition.X + chance1;
            float newY = DefaultPosition.Y + chance2;

            if( PathFinderMap.isTileWalkable( (int) newX / Game1.tileSize, (int ) newY / Game1.tileSize, currentLocation ) ) {
                position.X = newX;
                position.Y = newY;
                
                DefaultPosition = position;
            } else {
                position = DefaultPosition;
            }
        }

        public virtual void activateMonster() {
            alpha = defaultAlpha;
            isInvisible = false;
        }

        public void changeMoveType( MovementType newMoveType ) {
            previousMoveType = moveType;
            moveType = newMoveType;
        }

        public void changeMoveType( MovementType newMoveType, MovementType afterNewMoveType ) {
            previousMoveType = moveType;
            moveType = newMoveType;
            nextMoveType = afterNewMoveType;
        }

        public void restorePreviousMoveType() {
            moveType = previousMoveType;
        }

        /// <summary>
        /// Moves the monster in a line away from the player
        /// </summary>
        private void calculateAndGetKnockedBacked() {
            xTile.Dimensions.Rectangle viewport  = Game1.viewport;
            GameTime time = Game1.currentGameTime;
            GameLocation currentLocation = Game1.currentLocation;
            if( ( double ) this.xVelocity != 0.0 || ( double ) this.yVelocity != 0.0 ) {
                if( double.IsNaN( ( double ) this.xVelocity ) || double.IsNaN( ( double ) this.yVelocity ) ) {
                    this.xVelocity = 0.0f;
                    this.yVelocity = 0.0f;
                }
                Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
                boundingBox.X += ( int ) this.xVelocity;
                boundingBox.Y -= ( int ) this.yVelocity;
                if( !currentLocation.isCollidingPosition( boundingBox, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                    this.position.X += this.xVelocity;
                    this.position.Y -= this.yVelocity;
                    if( this.slipperiness < 1000 ) {
                        this.xVelocity = this.xVelocity - this.xVelocity / ( float ) this.slipperiness;
                        this.yVelocity = this.yVelocity - this.yVelocity / ( float ) this.slipperiness;
                        if( ( double ) Math.Abs( this.xVelocity ) <= 0.0500000007450581 )
                            this.xVelocity = 0.0f;
                        if( ( double ) Math.Abs( this.yVelocity ) <= 0.0500000007450581 )
                            this.yVelocity = 0.0f;
                    }
                    if( !this.isGlider && this.invincibleCountdown > 0 ) {
                        this.slideAnimationTimer = this.slideAnimationTimer - time.ElapsedGameTime.Milliseconds;
                        if( this.slideAnimationTimer < 0 && ( ( double ) Math.Abs( this.xVelocity ) >= 3.0 || ( double ) Math.Abs( this.yVelocity ) >= 3.0 ) ) {
                            this.slideAnimationTimer = 100 - ( int ) ( ( double ) Math.Abs( this.xVelocity ) * 2.0 + ( double ) Math.Abs( this.yVelocity ) * 2.0 );
                            currentLocation.temporarySprites.Add( new TemporaryAnimatedSprite( 6, this.getStandingPosition() + new Vector2( ( float ) ( -Game1.tileSize / 2 ), ( float ) ( -Game1.tileSize / 2 ) ), Color.White * 0.75f, 8, Game1.random.NextDouble() < 0.5, 20f, 0, -1, -1f, -1, 0 ) {
                                scale = 0.75f
                            } );
                        }
                    }
                } else if( this.isGlider || this.slipperiness >= 8 ) {
                    bool[] flagArray = Utility.horizontalOrVerticalCollisionDirections( boundingBox, ( Character ) this, false );
                    int index1 = 0;
                    if( flagArray[ index1 ] ) {
                        this.xVelocity = -this.xVelocity;
                        this.position.X += ( float ) Math.Sign( this.xVelocity );
                        this.rotation = this.rotation + ( float ) ( Math.PI + ( double ) Game1.random.Next( -10, 11 ) * Math.PI / 500.0 );
                    }
                    int index2 = 1;
                    if( flagArray[ index2 ] ) {
                        this.yVelocity = -this.yVelocity;
                        this.position.Y -= ( float ) Math.Sign( this.yVelocity );
                        this.rotation = this.rotation + ( float ) ( Math.PI + ( double ) Game1.random.Next( -10, 11 ) * Math.PI / 500.0 );
                    }
                    if( this.slipperiness < 1000 ) {
                        this.xVelocity = this.xVelocity - ( float ) ( ( double ) this.xVelocity / ( double ) this.slipperiness / 4.0 );
                        this.yVelocity = this.yVelocity - ( float ) ( ( double ) this.yVelocity / ( double ) this.slipperiness / 4.0 );
                        if( ( double ) Math.Abs( this.xVelocity ) <= 0.0500000007450581 )
                            this.xVelocity = 0.0f;
                        if( ( double ) Math.Abs( this.yVelocity ) <= 0.0509999990463257 )
                            this.yVelocity = 0.0f;
                    }
                } else {
                    this.xVelocity = this.xVelocity - this.xVelocity / ( float ) this.slipperiness;
                    this.yVelocity = this.yVelocity - this.yVelocity / ( float ) this.slipperiness;
                    if( ( double ) Math.Abs( this.xVelocity ) <= 0.0500000007450581 )
                        this.xVelocity = 0.0f;
                    if( ( double ) Math.Abs( this.yVelocity ) <= 0.0500000007450581 )
                        this.yVelocity = 0.0f;
                }
            }

            if( xVelocity <= 0 && yVelocity <= 0) {
                isGettingKnockedBack = false;
            }
        }

        /// <summary>
        /// Finds the next position. Used for boundary checks
        /// </summary>
        public override Microsoft.Xna.Framework.Rectangle nextPosition( int direction ) {
            Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
            int speed = Convert.ToInt32( ( this.speed + this.addedSpeed )  ); //TESTINGILAKSDJFLASDFJSFK
            switch( direction ) {
                case 0:
                    boundingBox.Y -= speed; 
                    break;
                case 1:
                    boundingBox.X += speed;
                    break;
                case 2:
                    boundingBox.Y += speed;
                    break;
                case 3:
                    boundingBox.X -= speed;
                    break;
            }
            return boundingBox;
        }
                
        /// <summary>
        /// Moves the monster in a direction.
        /// If the monster is barely clipping an edge it will shift him over before moving in that direction.
        /// If the monster is getting knocked back it will perform a knockback motion instead.
        /// </summary>
        public override void MovePosition( GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation ) {

            this.lastPosition = this.position;

            //TODO make into object oriented
            if( isGettingKnockedBack ) {
                calculateAndGetKnockedBacked();
                return;
            }

            int actualSpeed = Convert.ToInt32( ( this.speed + this.addedSpeed )  );  ///TESTSINGIGAN

            if( this.moveUp ) {
                
                //if next spot is open then move

                if( !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) || this.isCharging ) {
                    //ModEntry.Log( "next location has collision" );
                    this.position.Y -= actualSpeed;
                    if( !this.ignoreMovementAnimations )
                        this.sprite.AnimateUp( time, 0, "" );
                    this.facingDirection = 0;
                    this.faceDirection( 0 );

                //else slide a little left or right dependin on box collision
                } else {
                    Microsoft.Xna.Framework.Rectangle nextPosition = this.nextPosition( 0 );
                    nextPosition.Width /= 4;
                    bool flag1 = currentLocation.isCollidingPosition( nextPosition, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    nextPosition.X += nextPosition.Width * 3;
                    bool flag2 = currentLocation.isCollidingPosition( nextPosition, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    TimeSpan elapsedGameTime;

                    //if you are on the edge of a block you will slide over
                    if( flag1 && !flag2 && !currentLocation.isCollidingPosition( this.nextPosition( 1 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {

                        //ModEntry.Log( "next location has near collision is now shifting to center" );
                        float local = this.position.X;

                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 + num3;

                        this.position.X = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 3 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        //ModEntry.Log( "next location has near collision is now shifting to center" );
                        float local = this.position.X;

                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;

                        this.position.X = ( float ) num4;
                    }
                    if( !currentLocation.isTilePassable( this.nextPosition( 0 ), viewport ) || !this.willDestroyObjectsUnderfoot ) {
                        this.Halt();
                        //ModEntry.Log( "next location is unpassable and do nothing" );
                    }
                    else if( this.willDestroyObjectsUnderfoot ) {
                        Vector2 vector2 = new Vector2( ( float ) ( this.getStandingX() / Game1.tileSize ), ( float ) ( this.getStandingY() / Game1.tileSize - 1 ) );
                        if( currentLocation.characterDestroyObjectWithinRectangle( this.nextPosition( 0 ), true ) ) {
                            Game1.playSound( "stoneCrack" );
                            this.position.Y -= ( float ) ( this.speed + this.addedSpeed );
                        } else {
                            int blockedInterval = this.blockedInterval;
                            elapsedGameTime = time.ElapsedGameTime;
                            int milliseconds = elapsedGameTime.Milliseconds;
                            this.blockedInterval = blockedInterval + milliseconds;
                        }
                    }
                    if( this.onCollision != null )
                        this.onCollision( currentLocation );
                }



            } else if( this.moveRight ) {
                if( !currentLocation.isCollidingPosition( this.nextPosition( 1 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) || this.isCharging ) {
                    this.position.X += actualSpeed;
                    if( !this.ignoreMovementAnimations )
                        this.sprite.AnimateRight( time, 0, "" );
                    this.facingDirection = 1;
                    this.faceDirection( 1 );
                } else {
                    Microsoft.Xna.Framework.Rectangle position = this.nextPosition( 1 );
                    position.Height /= 4;
                    bool flag1 = currentLocation.isCollidingPosition( position, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    position.Y += position.Height * 3;
                    bool flag2 = currentLocation.isCollidingPosition( position, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    TimeSpan elapsedGameTime;
                    if( flag1 && !flag2 && !currentLocation.isCollidingPosition( this.nextPosition( 2 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {

                        float local = this.position.Y;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 + num3;
                        this.position.Y = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        float local = this.position.Y;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        this.position.Y = ( float ) num4;
                    }
                    if( !currentLocation.isTilePassable( this.nextPosition( 1 ), viewport ) || !this.willDestroyObjectsUnderfoot )
                        this.Halt();
                    else if( this.willDestroyObjectsUnderfoot ) {
                        Vector2 vector2 = new Vector2( ( float ) ( this.getStandingX() / Game1.tileSize + 1 ), ( float ) ( this.getStandingY() / Game1.tileSize ) );
                        if( currentLocation.characterDestroyObjectWithinRectangle( this.nextPosition( 1 ), true ) ) {
                            Game1.playSound( "stoneCrack" );
                            this.position.X += ( float ) ( this.speed + this.addedSpeed );
                        } else {
                            int blockedInterval = this.blockedInterval;
                            elapsedGameTime = time.ElapsedGameTime;
                            int milliseconds = elapsedGameTime.Milliseconds;
                            this.blockedInterval = blockedInterval + milliseconds;
                        }
                    }
                    if( this.onCollision != null )
                        this.onCollision( currentLocation );
                }



            } else if( this.moveDown ) {
                if( !currentLocation.isCollidingPosition( this.nextPosition( 2 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) || this.isCharging ) {
                    this.position.Y += actualSpeed;
                    if( !this.ignoreMovementAnimations )
                        this.sprite.AnimateDown( time, 0, "" );
                    this.facingDirection = 2;
                    this.faceDirection( 2 );
                } else {
                    Microsoft.Xna.Framework.Rectangle position = this.nextPosition( 2 );
                    position.Width /= 4;
                    bool flag1 = currentLocation.isCollidingPosition( position, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    position.X += position.Width * 3;
                    bool flag2 = currentLocation.isCollidingPosition( position, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    TimeSpan elapsedGameTime;
                    if( flag1 && !flag2 && !currentLocation.isCollidingPosition( this.nextPosition( 1 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        float local = this.position.X;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 + num3;
                        this.position.X = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 3 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        float local = this.position.X;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        this.position.X = ( float ) num4;
                    }
                    if( !currentLocation.isTilePassable( this.nextPosition( 2 ), viewport ) || !this.willDestroyObjectsUnderfoot )
                        this.Halt();
                    else if( this.willDestroyObjectsUnderfoot ) {
                        Vector2 vector2 = new Vector2( ( float ) ( this.getStandingX() / Game1.tileSize ), ( float ) ( this.getStandingY() / Game1.tileSize + 1 ) );
                        if( currentLocation.characterDestroyObjectWithinRectangle( this.nextPosition( 2 ), true ) ) {
                            Game1.playSound( "stoneCrack" );
                            this.position.Y += ( float ) ( this.speed + this.addedSpeed );
                        } else {
                            int blockedInterval = this.blockedInterval;
                            elapsedGameTime = time.ElapsedGameTime;
                            int milliseconds = elapsedGameTime.Milliseconds;
                            this.blockedInterval = blockedInterval + milliseconds;
                        }
                    }
                    if( this.onCollision != null )
                        this.onCollision( currentLocation );
                }



            } else if( this.moveLeft ) {
                if( !currentLocation.isCollidingPosition( this.nextPosition( 3 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) || this.isCharging ) {
                    this.position.X -= actualSpeed;
                    this.facingDirection = 3;
                    if( !this.ignoreMovementAnimations )
                        this.sprite.AnimateLeft( time, 0, "" );
                    this.faceDirection( 3 );
                } else {
                    Microsoft.Xna.Framework.Rectangle position = this.nextPosition( 3 );
                    position.Height /= 4;
                    bool flag1 = currentLocation.isCollidingPosition( position, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    position.Y += position.Height * 3;
                    bool flag2 = currentLocation.isCollidingPosition( position, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    TimeSpan elapsedGameTime;
                    if( flag1 && !flag2 && !currentLocation.isCollidingPosition( this.nextPosition( 2 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        float local = this.position.Y;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 + num3;
                        this.position.Y = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        
                        float local = this.position.Y;
                        
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        this.position.Y = ( float ) num4;
                    }
                    if( !currentLocation.isTilePassable( this.nextPosition( 3 ), viewport ) || !this.willDestroyObjectsUnderfoot )
                        this.Halt();
                    else if( this.willDestroyObjectsUnderfoot ) {
                        Vector2 vector2 = new Vector2( ( float ) ( this.getStandingX() / Game1.tileSize - 1 ), ( float ) ( this.getStandingY() / Game1.tileSize ) );
                        if( currentLocation.characterDestroyObjectWithinRectangle( this.nextPosition( 3 ), true ) ) {
                            Game1.playSound( "stoneCrack" );
                            this.position.X -= ( float ) ( this.speed + this.addedSpeed );
                        } else {
                            int blockedInterval = this.blockedInterval;
                            elapsedGameTime = time.ElapsedGameTime;
                            int milliseconds = elapsedGameTime.Milliseconds;
                            this.blockedInterval = blockedInterval + milliseconds;
                        }
                    }
                    if( this.onCollision != null )
                        this.onCollision( currentLocation );
                }
            } else if( !this.ignoreMovementAnimations ) {
                if( this.moveUp )
                    this.sprite.AnimateUp( time, 0, "" );
                else if( this.moveRight )
                    this.sprite.AnimateRight( time, 0, "" );
                else if( this.moveDown )
                    this.sprite.AnimateDown( time, 0, "" );
                else if( this.moveLeft )
                    this.sprite.AnimateLeft( time, 0, "" );
            }
            if( !this.ignoreMovementAnimations )
                this.sprite.interval = ( float ) this.defaultAnimationInterval - ( float ) ( this.speed + this.addedSpeed - 2 ) * 20f;
            if( ( this.blockedInterval < 3000 || ( double ) this.blockedInterval > 3750.0 ) && this.blockedInterval >= 5000 ) {
                this.speed = 4;
                this.isCharging = true;
                this.blockedInterval = 0;
            }


            //make a noise if hits the farmer
            if( this.damageToFarmer <= 0 || Game1.random.NextDouble() >= 0.000333333333333333 )
                return;
            if( this.name.Equals( "Shadow Guy" ) && Game1.random.NextDouble() < 0.3 ) {
                if( Game1.random.NextDouble() < 0.5 )
                    Game1.playSound( "grunt" );
                else
                    Game1.playSound( "shadowpeep" );
            } else {
                if( this.name.Equals( "Shadow Girl" ) )
                    return;
                if( this.name.Equals( "Ghost" ) ) {
                    Game1.playSound( "ghost" );
                } else {
                    if( this.name.Contains( "Slime" ) )
                        return;
                    this.name.Contains( "Jelly" );
                }
            }
        }
        
        /// <summary>
        /// Decides which way a monster should move for next update
        /// </summary>
        public override void behaviorAtGameTick( GameTime time ) { //TODO put behaviors at different distances OBJECT ORIENTED
            moveType.updateOnEveryTick( this );

            //TODO this needs to be refactored
            alpha -= amountToFadePerGameTick * time.ElapsedGameTime.Milliseconds / 1000;
            if( alpha <= 0 ) {
                isInvisible = true;
            }

            if( ( double ) this.timeBeforeAIMovementAgain > 0.0 ) {
                this.timeBeforeAIMovementAgain = this.timeBeforeAIMovementAgain - ( ( float ) time.ElapsedGameTime.Milliseconds / 300 );
                return;
            }

            this.timeBeforeAIMovementAgain = 1; //default is 1

            Halt(); // reset movement for next movement 
            moveType.calculateNextMovement( this );      
        }

        /// <summary>
        /// Sets a random direction to move in
        /// </summary>
        public void setRandomDirection() {
            int chance = Game1.random.Next( 1, 5 );
            switch( chance ) {
                case 1:
                    SetMovingOnlyLeft();
                    break;
                case 2:
                    SetMovingOnlyRight();
                    break;
                case 3:
                    SetMovingOnlyUp();
                    break;
                case 4:
                    SetMovingOnlyDown();
                    break;
            }
        }
        

    }
}
