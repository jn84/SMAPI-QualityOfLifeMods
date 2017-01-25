using System;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ModToolbox {
    internal class SpriteSheetFinder : IClickableMenu {

        private int positionOfTextureX = 0;
        private int positionOfTextureY = 0;
        private bool isRightClicking = false;
        private bool isLeftClicking = false;
        private IClickableMenu titleMenu;
        private List<Texture2D> allTextures = new List<Texture2D>();
        private int currentTextureIndex = 0;

        private Texture2D currentTexture = Game1.mouseCursors;

        public SpriteSheetFinder( IClickableMenu titleMenu ) {
            this.titleMenu = titleMenu;
            ControlEvents.MouseChanged += releaseRightClick;

            allTextures.Add( Game1.objectSpriteSheet );
            allTextures.Add( Game1.toolSpriteSheet );
            allTextures.Add( Game1.cropSpriteSheet );
            allTextures.Add( Game1.mailboxTexture );
            allTextures.Add( Game1.emoteSpriteSheet );
            allTextures.Add( Game1.debrisSpriteSheet );
            allTextures.Add( Game1.toolIconBox );
            allTextures.Add( Game1.rainTexture );
            allTextures.Add( Game1.bigCraftableSpriteSheet );
            allTextures.Add( Game1.swordSwipe );
            allTextures.Add( Game1.swordSwipeDark );
            allTextures.Add( Game1.buffsIcons );
            allTextures.Add( Game1.daybg );
            allTextures.Add( Game1.nightbg );
            allTextures.Add( Game1.logoScreenTexture );
            allTextures.Add( Game1.tvStationTexture );
            allTextures.Add( Game1.cloud );
            allTextures.Add( Game1.menuTexture );
            allTextures.Add( Game1.lantern );
            allTextures.Add( Game1.windowLight );
            allTextures.Add( Game1.sconceLight );
            allTextures.Add( Game1.cauldronLight );
            allTextures.Add( Game1.shadowTexture );
            allTextures.Add( Game1.mouseCursors );
            allTextures.Add( Game1.indoorWindowLight );
            allTextures.Add( Game1.animations );
            allTextures.Add( Game1.titleScreenBG );
            allTextures.Add( Game1.logo );
            allTextures.Add( Game1.fadeToBlackRect );
            allTextures.Add( Game1.staminaRect );
            allTextures.Add( Game1.currentCoopTexture );
            allTextures.Add( Game1.currentBarnTexture );
            allTextures.Add( Game1.currentHouseTexture );
            allTextures.Add( Game1.greenhouseTexture );
            allTextures.Add( Game1.littleEffect );
    }

        private void releaseRightClick( object sender, EventArgsMouseStateChanged e ) {
            if ( e.NewState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released ) {
                isRightClicking = false;
            }
        }

        public override void draw( SpriteBatch b ) {

            // Clear screen
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( 0, 0, Game1.viewport.Width, Game1.viewport.Height ), Color.Black );

            // Draw Texture Chooser
            Game1.spriteBatch.Draw( allTextures[ currentTextureIndex ] != null ? allTextures[ currentTextureIndex ]: allTextures[ currentTextureIndex + 1], Vector2.Zero, new Rectangle( positionOfTextureX, positionOfTextureY, Game1.mouseCursors.Width, Game1.mouseCursors.Height ), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1 );
             
            // Redraw mouse
            Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getOldMouseX(), ( float ) Game1.getOldMouseY() ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16 ) ), Color.White, 0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );
            ;
        }

        public override void update( GameTime time ) {
            base.update( time );
            
            if( isRightClicking ) {
                int deltaX = Game1.getMouseX() - Game1.getOldMouseX();
                int deltaY = Game1.getMouseY() - Game1.getOldMouseY();

                positionOfTextureX -= deltaX / 3;
                positionOfTextureY -= deltaY / 3;
            }

        }

        public override void receiveScrollWheelAction( int direction ) {
            base.receiveScrollWheelAction( direction );

            if ( direction > 0 ) {
                currentTextureIndex++;

                if ( currentTextureIndex > allTextures.Count - 1 ) {
                    currentTextureIndex = 0;
                }
            }

            if ( direction < 0 ) {
                currentTextureIndex--;
                if ( currentTextureIndex < 0 ) {
                    currentTextureIndex = allTextures.Count - 1;
                }
            }

            positionOfTextureX = 0;
            positionOfTextureY = 0;

        }

        public override void receiveRightClick( int x, int y, bool playSound = true ) {
            isRightClicking = true;
        }

        public override void receiveLeftClick( int x, int y, bool playSound = true ) {
            base.receiveLeftClick( x, y, playSound );
            isLeftClicking = true;
        }

        public override void releaseLeftClick( int x, int y ) {
            base.releaseLeftClick( x, y );
        }

        public override void receiveKeyPress( Keys key ) {
            base.receiveKeyPress( key );

            if( key.Equals( Keys.Escape ) ) {

            }
        }


    }
}