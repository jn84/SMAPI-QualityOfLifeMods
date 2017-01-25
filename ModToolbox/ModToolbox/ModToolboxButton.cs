using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace ModToolbox {
    class ModToolboxButton : IClickableMenu {

        public Rectangle bounds = new Rectangle();

        private void setDefaultVariables() {
            width = 196;
            height = 70;
            xPositionOnScreen = 20;
            yPositionOnScreen = Game1.viewport.Height - 100;

            bounds.Width = width;
            bounds.Height = height;
            bounds.X = xPositionOnScreen;
            bounds.Y = yPositionOnScreen;
        }

        public override void receiveRightClick( int x, int y, bool playSound = true ) {
        }

        public override void receiveLeftClick( int x, int y, bool playSound = true ) {
            base.receiveLeftClick( x, y, playSound );

            if ( bounds.Contains( x, y ) ) {
                Game1.activeClickableMenu = new SpriteSheetFinder( );
            }

        }

        public override void draw( SpriteBatch b ) {
            base.draw( b );

            setDefaultVariables();

            IClickableMenu.drawTextureBox( Game1.spriteBatch, xPositionOnScreen, yPositionOnScreen, width, height, Color.White );
            if ( bounds.Contains( Game1.getMouseX(), Game1.getMouseY() ) ) {
                IClickableMenu.drawTextureBox( Game1.spriteBatch, xPositionOnScreen - 8, yPositionOnScreen - 8, width + 16, height + 16, Color.White );
            }
            Game1.spriteBatch.DrawString( Game1.smallFont, "Mod Toolbox", new Vector2( xPositionOnScreen + 20, yPositionOnScreen + 24 ), Color.Black );
            drawMouse( b );
        }
    }
}
