using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Demiacle_SVM.UiMods {
    class UiModLuckOfDay {

        private ClickableTextureComponent icon;
        private string hoverText = "";

        internal void onNewDay( object sender, EventArgsIntChanged e ) {
            /*
            string dialogue = "Nobody is sure whats going to happen today...";
                dialogue = $"You feel like you should go out and buy a lottery ticket... if that was your thing";
                dialogue = $"Good things are going to happen today!... but probably not great things...";
                dialogue = $"You get the feeling you shouldn't take any risks today";
                dialogue = $"You should probably just stay home today...";
            */    
        }

        internal void onPreRender( object sender, EventArgs e ) {
            Color color = new Color( Color.White.ToVector4() );
            if( Game1.dailyLuck > 0.04d ) {
                hoverText = "You're feelin' lucky!!";
                color.B = 155;
                color.R = 155;
            } else if( Game1.dailyLuck < -0.04d ) {
                hoverText = "Maybe you should stay home today...";
                color.B = 155;
                color.G = 155;
            } else if( -0.04d <= Game1.dailyLuck && Game1.dailyLuck < 0 ){
                hoverText = "You're not feeling lucky at all today...";
                color.B = 165;
                color.G = 165;
                color.R = 165;
                color *= 0.8f;
            } else {
                hoverText = "Feelin' lucky... but not too lucky";
            }

            icon.draw( Game1.spriteBatch, color, 1 );
        }

        internal void onLocationChange( object sender, EventArgsCurrentLocationChanged e ) {
            icon = new ClickableTextureComponent( "", new Rectangle( ( int ) DemiacleUtility.getWidthInPlayArea(  ) - 134, 260, 10 * Game1.pixelZoom, 14 * Game1.pixelZoom ), "", "", Game1.mouseCursors, new Rectangle( 50, 428, 10, 14 ), Game1.pixelZoom );
        }

        internal void onPostRender( object sender, EventArgs e ) {
            // If cursor is highlighting dice, display hover text and redraw mouse
            if( icon.containsPoint( Game1.oldMouseState.X, Game1.oldMouseState.Y ) ) {
                IClickableMenu.drawHoverText( Game1.spriteBatch, hoverText, Game1.dialogueFont );
                //Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );
            }
        }
    }
}
