using System;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;

namespace DemiacleSvm {
    internal class SpriteSheetFinder : IClickableMenu {

        private int positionOfTextureX = 0;
        private int positionOfTextureY = 0;
        private bool isRightClicking = false;
        private bool isLeftClicking = false;

        public override void draw( SpriteBatch b ) {

            // Clear screen
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( 0, 0, Game1.viewport.Width, Game1.viewport.Height ), Color.Black );

            // Draw Texture Chooser
            Game1.spriteBatch.Draw( Game1.mouseCursors, Vector2.Zero, new Rectangle( positionOfTextureX, positionOfTextureY, Game1.mouseCursors.Width, Game1.mouseCursors.Height ), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1 );

            // Redraw mouse
            Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getOldMouseX(), ( float ) Game1.getOldMouseY() ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16 ) ), Color.White, 0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );
            ;
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



    }
}