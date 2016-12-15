using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Monsters;
using Microsoft.Xna.Framework;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;

namespace Demiacle_SVM.OutdoorMonsters {
    public class OutDoorShadowBrute : Monster {
        private int wasHitCounter;
        private float targetRotation;
        private bool turningRight;
        private bool seenPlayer;

        public OutDoorShadowBrute() {
            //this.sprite.spriteHeight = 32;
        }

        public OutDoorShadowBrute( Vector2 position )

      : base("Shadow Brute", position) {

            //TODO current speed is somehow 2 and need to find a way to make slower than 1
            this.speed = 1;
            this.addedSpeed = 0;
            reloadSprite();
            //.sprite.spriteHeight = 32;
            //this.sprite.UpdateSourceRect();
        }

        public override void reloadSprite() {
            this.sprite = new AnimatedSprite( Game1.content.Load<Texture2D>( "Characters\\Monsters\\Shadow Brute" ) );
            this.sprite.spriteHeight = 32;
            this.sprite.UpdateSourceRect();
        }

        public override int takeDamage( int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision ) {
            Game1.playSound( "shadowHit" );
            return base.takeDamage( damage, xTrajectory, yTrajectory, isBomb, addedPrecision );
        }

        public override void deathAnimation() {
            Utility.makeTemporarySpriteJuicier( new TemporaryAnimatedSprite( 45, this.position, Color.White, 10, false, 100f, 0, -1, -1f, -1, 0 ), Game1.currentLocation, 4, 64, 64 );
            for( int index = 1; index < 3; ++index ) {
                Game1.currentLocation.temporarySprites.Add( new TemporaryAnimatedSprite( 6, this.position + new Vector2( 0.0f, 1f ) * ( float ) Game1.tileSize * ( float ) index, Color.Gray * 0.75f, 10, false, 100f, 0, -1, -1f, -1, 0 ) {
                    delayBeforeAnimationStart = index * 159
                } );
                Game1.currentLocation.temporarySprites.Add( new TemporaryAnimatedSprite( 6, this.position + new Vector2( 0.0f, -1f ) * ( float ) Game1.tileSize * ( float ) index, Color.Gray * 0.75f, 10, false, 100f, 0, -1, -1f, -1, 0 ) {
                    delayBeforeAnimationStart = index * 159
                } );
                Game1.currentLocation.temporarySprites.Add( new TemporaryAnimatedSprite( 6, this.position + new Vector2( 1f, 0.0f ) * ( float ) Game1.tileSize * ( float ) index, Color.Gray * 0.75f, 10, false, 100f, 0, -1, -1f, -1, 0 ) {
                    delayBeforeAnimationStart = index * 159
                } );
                Game1.currentLocation.temporarySprites.Add( new TemporaryAnimatedSprite( 6, this.position + new Vector2( -1f, 0.0f ) * ( float ) Game1.tileSize * ( float ) index, Color.Gray * 0.75f, 10, false, 100f, 0, -1, -1f, -1, 0 ) {
                    delayBeforeAnimationStart = index * 159
                } );
            }
            Game1.playSound( "shadowDie" );
            Game1.createRadialDebris( Game1.currentLocation, this.sprite.Texture, new Rectangle( this.sprite.SourceRect.X, this.sprite.SourceRect.Y, 16, 5 ), 16, this.getStandingX(), this.getStandingY() - Game1.tileSize / 2, 1, this.getStandingY() / Game1.tileSize, Color.White, ( float ) Game1.pixelZoom );
            Game1.createRadialDebris( Game1.currentLocation, this.sprite.Texture, new Rectangle( this.sprite.SourceRect.X + 2, this.sprite.SourceRect.Y + 5, 16, 5 ), 10, this.getStandingX(), this.getStandingY() - Game1.tileSize / 2, 1, this.getStandingY() / Game1.tileSize, Color.White, ( float ) Game1.pixelZoom );
        }

        public override void behaviorAtGameTick( GameTime time ) {

            if( ( double ) this.timeBeforeAIMovementAgain > 0.0 )
                this.timeBeforeAIMovementAgain = this.timeBeforeAIMovementAgain - ( float ) time.ElapsedGameTime.Milliseconds;
            
            Rectangle boundingBox = Game1.player.GetBoundingBox();
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

            ModEntry.modEntry.Monitor.Log("speed is :" + this.speed+" and added is :" +this.addedSpeed);
            


            //do horizontal movement

            if( distanceX > distanceY ) {
                //ModEntry.modEntry.Monitor.Log( " distanceX : " + distanceX + " and distanceY : " + distanceY );
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

