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

namespace Demiacle_SVM.OutdoorMonsters {
    /// <summary>
    /// Adds a layer for easily modifiable properties and types of movement to the monster class.
    /// </summary>
    public class OutDoorMonster : Monster {

        public int slideAnimationTimer = 100;
        public Boolean ignoresCollissions = false;
        private MoveType moveType;
        private MoveType previousMoveType;
        public float moveSpeedExtension = MoveSpeedExtension.medium;

        public OutDoorMonster() : base() {

        }

        public OutDoorMonster( string name, Vector2 position )
            : base( name, position ) {
            
        }

        public enum MoveType {
            noCollisions,
            knockback,
            pathFinding
        }

        public struct MoveSpeedExtension {
            public const float fast = 0.5f;
            public const float medium = 1;
            public const float slow = 5;
            public const float slower = 10; 
        }

        public enum timeToDecideNextMovement { fast, medium, slow }

        public override void MovePosition( GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation ) {
            switch( this.moveType) {
                case MoveType.noCollisions:
                    MoveWithoutCollision( time, viewport, currentLocation);
                    break;
                case MoveType.knockback:
                    getKnockedBacked( time, viewport, currentLocation );
                    break;
                case MoveType.pathFinding:
                    MoveWithPathFinding( time, viewport, currentLocation );
                    break;
                default:
                    ModEntry.Log( "ruh roh this guy has no movement type...." );
                    break;
            }
        }

        public void changeMoveType( MoveType newMoveType ) {
            previousMoveType = moveType;
            moveType = newMoveType;
        }

        public void restorePreviousMoveType() {
            moveType = previousMoveType;
        }

        private void getKnockedBacked( GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation ) {

            if( this.xVelocity <= 0 && this.yVelocity <= 0) {
                restorePreviousMoveType();
            }

            this.lastPosition = this.position;
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

        }
        

        //this chooses the type of movement
        public void MoveWithoutCollision( GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation ) {
            this.lastPosition = this.position;

            float actualSpeed = ( float ) ( this.speed + this.addedSpeed ) / moveSpeedExtension;
            //ModEntry.Log(" actual spee dis :" + actualSpeed);

            if( this.moveUp ) {

                // next position takes a direction
                //if something in next spot dont do nothin
                if( !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) || this.isCharging ) {
                    this.position.Y -= actualSpeed;
                    if( !this.ignoreMovementAnimations )
                        this.sprite.AnimateUp( time, 0, "" );
                    this.facingDirection = 0;
                    this.faceDirection( 0 );

                    //else move
                } else {
                    Microsoft.Xna.Framework.Rectangle nextPosition = this.nextPosition( 0 );
                    nextPosition.Width /= 4;
                    bool flag1 = currentLocation.isCollidingPosition( nextPosition, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    nextPosition.X += nextPosition.Width * 3;
                    bool flag2 = currentLocation.isCollidingPosition( nextPosition, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    TimeSpan elapsedGameTime;
                    if( flag1 && !flag2 && !currentLocation.isCollidingPosition( this.nextPosition( 1 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {

                        float local = this.position.X;

                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 + num3;

                        local = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 3 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {

                        float local = this.position.X;

                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;

                        local = ( float ) num4;
                    }
                    if( !currentLocation.isTilePassable( this.nextPosition( 0 ), viewport ) || !this.willDestroyObjectsUnderfoot )
                        this.Halt();


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
                        local = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        float local = this.position.Y;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        local = ( float ) num4;
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
                        local = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 3 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        float local = this.position.X;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        local = ( float ) num4;
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
                        local = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        
                        float local = this.position.Y;
                        
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        local = ( float ) num4;
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

        public void MoveWithPathFinding( GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation ) {
            this.lastPosition = this.position;

            float actualSpeed = ( float ) ( this.speed + this.addedSpeed ) / moveSpeedExtension;

            if( this.moveUp ) {

                // next position takes a direction
                //if something in next spot dont do nothin
                if( !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) || this.isCharging ) {
                    this.position.Y -= actualSpeed;
                    if( !this.ignoreMovementAnimations )
                        this.sprite.AnimateUp( time, 0, "" );
                    this.facingDirection = 0;
                    this.faceDirection( 0 );

                    //else move
                } else {
                    Microsoft.Xna.Framework.Rectangle nextPosition = this.nextPosition( 0 );
                    nextPosition.Width /= 4;
                    bool flag1 = currentLocation.isCollidingPosition( nextPosition, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    nextPosition.X += nextPosition.Width * 3;
                    bool flag2 = currentLocation.isCollidingPosition( nextPosition, viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this );
                    TimeSpan elapsedGameTime;
                    if( flag1 && !flag2 && !currentLocation.isCollidingPosition( this.nextPosition( 1 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {

                        float local = this.position.X;

                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 + num3;

                        local = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 3 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {

                        float local = this.position.X;

                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;

                        local = ( float ) num4;
                    }
                    if( !currentLocation.isTilePassable( this.nextPosition( 0 ), viewport ) || !this.willDestroyObjectsUnderfoot )
                        this.Halt();


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
                        local = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        float local = this.position.Y;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        local = ( float ) num4;
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
                        local = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 3 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {
                        float local = this.position.X;
                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        local = ( float ) num4;
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
                        local = ( float ) num4;
                    } else if( flag2 && !flag1 && !currentLocation.isCollidingPosition( this.nextPosition( 0 ), viewport, false, this.damageToFarmer, this.isGlider, ( Character ) this ) ) {

                        float local = this.position.Y;

                        double num1 = ( double ) local;
                        double speed = ( double ) this.speed;
                        elapsedGameTime = time.ElapsedGameTime;
                        double num2 = ( double ) elapsedGameTime.Milliseconds / 64.0;
                        double num3 = speed * num2;
                        double num4 = num1 - num3;
                        local = ( float ) num4;
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

        //this actually moves the target
        public override void behaviorAtGameTick( GameTime time ) {

            if( ( double ) this.timeBeforeAIMovementAgain > 0.0 ) {
                this.timeBeforeAIMovementAgain = this.timeBeforeAIMovementAgain - ( ( float ) time.ElapsedGameTime.Milliseconds / 300 );
                return;
            }
            this.timeBeforeAIMovementAgain = 1;
            Microsoft.Xna.Framework.Rectangle boundingBox = Game1.player.GetBoundingBox();
            int playerX = boundingBox.Center.X;
            int playerY = boundingBox.Center.Y;
            boundingBox = this.GetBoundingBox();
            int monsterX = boundingBox.Center.X;
            int monsterY = boundingBox.Center.Y;


            int distanceX = Math.Abs( playerX - monsterX );
            int distanceY = Math.Abs( playerY - monsterY );


            this.Halt();

            if( distanceX + distanceY > Game1.tileSize * 10 ) {
                return; // or do idle behavior
            }

            ModEntry.modEntry.Monitor.Log( "speed is :" + this.speed + " and added is :" + this.addedSpeed );

            //decide to move horizontal or vertical
            if( distanceX > distanceY ) {
                if( playerX - monsterX < 0 )
                    this.SetMovingLeft( true );
                else
                    this.SetMovingRight( true );
            } else {

                if( playerY - monsterY < 0 )
                    this.SetMovingUp( true );
                else
                    this.SetMovingDown( true );
            }
            this.MovePosition( time, Game1.viewport, Game1.currentLocation );


            /*
            base.behaviorAtGameTick( time );
            if( this.wasHitCounter >= 0 )
                this.wasHitCounter = this.wasHitCounter - time.ElapsedGameTime.Milliseconds;
            if( double.IsNaN( ( double ) this.xVelocity ) || double.IsNaN( ( double ) this.yVelocity ) || ( ( double ) this.position.X < -2000.0 || ( double ) this.position.Y < -2000.0 ) )
                this.health = -500;
            if( ( double ) this.position.X <= -640.0 || ( double ) this.position.Y <= -640.0 || ( ( double ) this.position.X >= ( double ) ( Game1.currentLocation.Map.Layers[ 0 ].LayerWidth * Game1.tileSize + 640 ) || ( double ) this.position.Y >= ( double ) ( Game1.currentLocation.Map.Layers[ 0 ].LayerHeight * Game1.tileSize + 640 ) ) )
                this.health = -500;
            if( this.focusedOnFarmers || this.withinPlayerThreshold( 6 ) || this.seenPlayer ) {
                this.seenPlayer = true;
                this.sprite.Animate( time, 0, 4, 80f );

                if( this.invincibleCountdown > 0 ) {
                    if( !this.name.Equals( "Lava Bat" ) )
                        return;
                    this.glowingColor = Color.Cyan;
                } else {
                    float distancFromPlayerX = ( float ) -( Game1.player.GetBoundingBox().Center.X - this.GetBoundingBox().Center.X );
                    float distanceFromPlayerY = ( float ) ( Game1.player.GetBoundingBox().Center.Y - this.GetBoundingBox().Center.Y );
                    float num3 = Math.Max( 1f, Math.Abs( distancFromPlayerX ) + Math.Abs( distanceFromPlayerY ) );
                    if( ( double ) num3 < ( double ) Game1.tileSize ) {
                        this.xVelocity = Math.Max( -5f, Math.Min( 5f, this.xVelocity * 1.05f ) );
                        this.yVelocity = Math.Max( -5f, Math.Min( 5f, this.yVelocity * 1.05f ) );
                    }
                    float num4 = distancFromPlayerX / num3;
                    float num5 = distanceFromPlayerY / num3;
                    
                    if( this.wasHitCounter <= 0 ) {
                        this.targetRotation = ( float ) Math.Atan2( -( double ) num5, ( double ) num4 ) - 1.570796f;
                        if( ( double ) Math.Abs( this.targetRotation ) - ( double ) Math.Abs( this.rotation ) > 7.0 * Math.PI / 8.0 && Game1.random.NextDouble() < 0.5 )
                            this.turningRight = true;
                        else if( ( double ) Math.Abs( this.targetRotation ) - ( double ) Math.Abs( this.rotation ) < Math.PI / 8.0 )
                            this.turningRight = false;
                        if( this.turningRight )
                            this.rotation = this.rotation - ( float ) Math.Sign( this.targetRotation - this.rotation ) * ( ( float ) Math.PI / 64f );
                        else
                            this.rotation = this.rotation + ( float ) Math.Sign( this.targetRotation - this.rotation ) * ( ( float ) Math.PI / 64f );
                        this.rotation = this.rotation % 6.283185f;
                        this.wasHitCounter = 0;
                    }
                    
                    float num6 = Math.Min( 5f, Math.Max( 1f, ( float ) ( 5.0 - ( double ) num3 / ( double ) Game1.tileSize / 2.0 ) ) );
                    float num7 = ( float ) Math.Cos( ( double ) this.rotation + Math.PI / 2.0 );
                    float num8 = -( float ) Math.Sin( ( double ) this.rotation + Math.PI / 2.0 );
                    this.xVelocity = this.xVelocity + ( float ) ( -( double ) num7 * ( double ) num6 / 6.0 + ( double ) Game1.random.Next( -10, 10 ) / 100.0 );
                    this.yVelocity = this.yVelocity + ( float ) ( -( double ) num8 * ( double ) num6 / 6.0 + ( double ) Game1.random.Next( -10, 10 ) / 100.0 );
                    if( ( double ) Math.Abs( this.xVelocity ) > ( double ) Math.Abs( ( float ) ( -( double ) num7 * 5.0 ) ) )
                        this.xVelocity = this.xVelocity - ( float ) ( -( double ) num7 * ( double ) num6 / 6.0 );
                    if( ( double ) Math.Abs( this.yVelocity ) <= ( double ) Math.Abs( ( float ) ( -( double ) num8 * 5.0 ) ) )
                        return;
                    this.yVelocity = this.yVelocity - ( float ) ( -( double ) num8 * ( double ) num6 / 6.0 );
                }
            } else {
                this.sprite.CurrentFrame = 4;
                this.Halt();
            }
            */
        }
    }
}
