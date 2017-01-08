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

namespace Demiacle_SVM.UiMods {
    class ToolbarMod : Toolbar {

        /// <summary>
        /// Draws the toolbar as well as the modified hover information
        /// </summary>
        public override void draw( SpriteBatch b ) {
            typeof( Toolbar ).GetField( "hoverItem", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( this, null );

            base.draw( b );
            performHoverAction( Game1.getMouseX(), Game1.getMouseY() );

            Item hoverItem = ( Item ) GetType().BaseType.GetField( "hoverItem", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( this );

            if( hoverItem == null ) {
                return;
            }
            
            string sellForAmount = "";
            string totalStackSellsFor = "";
            
            if( hoverItem.salePrice() > 0 ) {
                sellForAmount = "    " + hoverItem.salePrice();

                if( hoverItem.canStackWith( hoverItem ) && hoverItem.getStack() > 1) {
                    sellForAmount += $" ({hoverItem.salePrice() * hoverItem.getStack()})";
                }
            }

            ;
            
            IClickableMenu.drawToolTip( b, hoverItem.getDescription() + totalStackSellsFor, hoverItem.Name + sellForAmount, hoverItem, false, -1, 0, -1, -1, null, -1 );
            

            // Draw coin
            if( sellForAmount != "") {
                b.Draw( Game1.debrisSpriteSheet, new Vector2( Game1.getMousePosition().X + 84 + Game1.dialogueFont.MeasureString( hoverItem.Name ).X, Game1.getMousePosition().Y + 72 ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.debrisSpriteSheet, 8, 16, 16 ) ), Color.White, 0f, new Vector2( 8f, 8f ), ( float ) Game1.pixelZoom, SpriteEffects.None, 0.95f );

            }
        }


        /// <summary>
        /// Copies the data from the supplied SocialPage to this instance.
        /// </summary>
        internal void Copy( Toolbar toolBar ) {
            FieldInfo[] fields = typeof( Toolbar ).GetFields( BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public );

            foreach( FieldInfo field in fields ) {
                var fieldToCopy = field.GetValue( toolBar );
                field.SetValue( this, fieldToCopy );
            }
        }
        
    }    
}
