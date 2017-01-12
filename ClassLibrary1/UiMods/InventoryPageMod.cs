using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemiacleSvm.UiMods {
    class InventoryPageMod : InventoryPage {

        public InventoryPageMod( int x, int y, int width, int height ) : base( x, y, width, height ) {

        }

        public override void draw( SpriteBatch b ) {
            GetType().BaseType.GetField( "hoverText", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( this, "" );

            base.draw( b );
            performHoverAction( Game1.getMouseX(), Game1.getMouseY() );

            Item hoverItem = (Item) GetType().BaseType.GetField( "hoveredItem", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( this );

            if( hoverItem == null ) {
                return;
            }

            string sellForAmount = "";
            string totalStackSellsFor = "";

            if( hoverItem.salePrice() > 0 ) {
                sellForAmount = "    " + hoverItem.salePrice();

                if( hoverItem.canStackWith( hoverItem ) && hoverItem.getStack() > 1 ) {
                    sellForAmount += $" ({hoverItem.salePrice() * hoverItem.getStack()})";
                }
            }

            ;

            IClickableMenu.drawToolTip( b, hoverItem.getDescription() + totalStackSellsFor, hoverItem.Name + sellForAmount, hoverItem, false, -1, 0, -1, -1, null, -1 );


            // Draw coin
            if( sellForAmount != "" ) {
                b.Draw( Game1.debrisSpriteSheet, new Vector2( Game1.getMousePosition().X + 84 + Game1.dialogueFont.MeasureString( hoverItem.Name ).X, Game1.getMousePosition().Y + 72 ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.debrisSpriteSheet, 8, 16, 16 ) ), Color.White, 0f, new Vector2( 8f, 8f ), ( float ) Game1.pixelZoom, SpriteEffects.None, 0.95f );

            }
        }

    }
}
