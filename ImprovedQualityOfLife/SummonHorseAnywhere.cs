using System;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Demiacle.ImprovedQualityOfLife {
    internal class SummonHorseAnywhere {

        private Vector2 positionToRunTo;
        private Horse horse;
        private float currentRunSpeed;

        private float initialRunSpeed = 19;
        private float speedToSlowPerRender = 0.3f;

        public SummonHorseAnywhere() {
            ControlEvents.KeyPressed += callHorseOnZPress;
            GraphicsEvents.OnPostRenderEvent += animateHorse;
        }

        private void animateHorse( object sender, EventArgs e ) {
            initialRunSpeed = 19;
            if( horse != null && horse.position.X < positionToRunTo.X ) {
                
                horse.position.X += currentRunSpeed;
                horse.SetMovingOnlyRight();

                if( horse.position.X + 600 > positionToRunTo.X && currentRunSpeed > 3 ) {
                    currentRunSpeed -= speedToSlowPerRender;
                }

                if( horse.sprite.currentAnimation == null ) {
                    horse.sprite.setCurrentAnimation( new List<FarmerSprite.AnimationFrame>() {

                    //These animate left
                    // new FarmerSprite.AnimationFrame(8, 70, false, true, null, false),
                    // new FarmerSprite.AnimationFrame(9, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    // new FarmerSprite.AnimationFrame(10, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    // new FarmerSprite.AnimationFrame(11, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    // new FarmerSprite.AnimationFrame(12, 70, false, true, null, false),
                    // new FarmerSprite.AnimationFrame(13, 70, false, true, null, false),

                    new FarmerSprite.AnimationFrame(8, 70),
                    new FarmerSprite.AnimationFrame(9, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    new FarmerSprite.AnimationFrame(10, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    new FarmerSprite.AnimationFrame(11, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    new FarmerSprite.AnimationFrame(12, 70),
                    new FarmerSprite.AnimationFrame(13, 70)
                    } );
                }

            } else if( horse != null ) {

                // If horse position is at a fraction the game draws glitches
                horse.position.X = (float) Math.Floor( horse.position.X );

                horse.sprite.setCurrentAnimation( new List<FarmerSprite.AnimationFrame>() {
                    new FarmerSprite.AnimationFrame(21, 600),
                    new FarmerSprite.AnimationFrame(22, 700),
                    new FarmerSprite.AnimationFrame(21, 5600)
                } );

                horse = null;

            }

        }

        private void callHorseOnZPress( object sender, EventArgsKeyPressed e ) {

            var x = Game1.player.getMount();

            // Only call horse outdoors
            if( e.KeyPressed == Keys.Z && Game1.currentLocation.isOutdoors == true ) {

                foreach( var location in Game1.locations ) {

                    // Ignore if horse is in current location
                    if( location == Game1.currentLocation ) {
                        continue;
                    }

                    // Find horse
                    for( int i = 0; i < location.characters.Count; i++ ) {

                        if( location.characters[ i ] is Horse ) {

                            horse = ( Horse ) location.characters[ i ];

                            // Change horse location
                            location.characters.Remove( horse );
                            Game1.currentLocation.characters.Add( horse );

                            // Set horse start position
                            positionToRunTo = Game1.player.position;
                            horse.position.Y = positionToRunTo.Y;
                            horse.position.X = Game1.viewport.X - horse.sprite.getWidth() * Game1.pixelZoom;

                            // Set horse moving right
                            horse.facingDirection = 1;

                            // Set starting variables
                            currentRunSpeed = initialRunSpeed;
                            horse.sprite.currentAnimation = null;

                            return;
                        }

                    }

                }

            }

        }

    }
}