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
    public class OutDoorShadowBrute : OutDoorMonster {
        private bool seenPlayer;

        public OutDoorShadowBrute() : base() {

        }

        public OutDoorShadowBrute( Vector2 position )
            : base("Shadow Brute", position) {
            this.speed = 1;
            this.addedSpeed = 0;

            reloadSprite();
            
            //this.slipperiness = 8;
            this.moveSpeedExtension = MoveSpeedExtension.slower;
            //.sprite.spriteHeight = 32;
            //this.sprite.UpdateSourceRect();
        }

        public override void draw( SpriteBatch b ) {
            //draw Transparently
            base.draw( b, 0.5f );
        }

        public override void reloadSprite() {
            this.sprite = new AnimatedSprite( Game1.content.Load<Texture2D>( "Characters\\Monsters\\Shadow Brute" ) );
            this.sprite.spriteHeight = 32;
            this.sprite.UpdateSourceRect();
        }

        public override int takeDamage( int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision ) {
            this.changeMoveType( MoveType.knockback );
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
    }
}

