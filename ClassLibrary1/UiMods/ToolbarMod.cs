using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
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

            Item hoverItem = GetType().BaseType.GetField( "hoverItem", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( this ) as Item;

            if( hoverItem == null ) {
                return;
            }
            
            string sellForAmount = "";
            string harvestPrice = "";
            
            if( hoverItem.salePrice() > 0 ) {
                sellForAmount = "\n  " + hoverItem.salePrice() / 2;

                if( hoverItem.canStackWith( hoverItem ) && hoverItem.getStack() > 1) {
                    sellForAmount += $" ({ hoverItem.salePrice() / 2 * hoverItem.getStack() })";
                }
            }

            bool isDrawingHarvestPrice = false;

            // Adds the price of the fully grown crop to the display text only if it is a seed
            if( hoverItem is StardewValley.Object && ( (StardewValley.Object) hoverItem).type == "Seeds" && sellForAmount != "" ) {
                if( hoverItem.Name != "Mixed Seeds" || hoverItem.Name != "Winter Seeds" ) {
                    Crop crop = new Crop( hoverItem.parentSheetIndex, 0, 0 );
                    Debris debris = new Debris( crop.indexOfHarvest, Game1.player.position, Game1.player.position );
                    //StardewValley.Object item = new StardewValley.Object( Vector2.Zero, debris.chunkType, false );
                    StardewValley.Object item = new StardewValley.Object( debris.chunkType, 1);
                    //string str;
                    //Game1.bigCraftablesInformation.TryGetValue( debris.chunkType, out str );
                    //string[] strArray = str.Split( '/' );
                    harvestPrice += $"    { item.price }";
                    isDrawingHarvestPrice = true;
                }
            }
            
            IClickableMenu.drawToolTip( b, hoverItem.getDescription(), hoverItem.Name + sellForAmount + harvestPrice, hoverItem, false, -1, 0, -1, -1, null, -1 );
            string test = hoverItem.getDescription();
            
            // Draw coin
            if( sellForAmount != "") {

                // yPositionOnScreen is a new private field for this class... who knows why
                int yPositionOnScreen = (int) typeof( Toolbar ).GetField( "yPositionOnScreen", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( this );
                int yOffsetForBottom = ( yPositionOnScreen > 200 ) ? ( int ) Game1.smallFont.MeasureString( hoverItem.getDescription() ).Y + Game1.tileSize : 0;

                float iconPositionX = Game1.getMousePosition().X + 78;
                float iconPositionY = 0;

                float fixTopX = 0;

                // If Toolbar is on the bottom
                if( yPositionOnScreen > 200 ) {
                    int offsetAdditionalItemInfo = 0;

                    // Offset health and stamina display
                    if( ( hoverItem as StardewValley.Object ).edibility != -300 ) {
                        offsetAdditionalItemInfo += 36 * 2;

                        // Offset displayed buffs
                        if( Game1.objectInformation[ ( hoverItem as StardewValley.Object ).parentSheetIndex ].Split( '/' ).Length >= 7 ) {
                            string[] buffIconsToDisplay = Game1.objectInformation[ ( hoverItem as StardewValley.Object ).parentSheetIndex ].Split( '/' )[ 6 ].Split( ' ' );
                            for( int i = 0; i < buffIconsToDisplay.Count(); i++ ) {
                                if( buffIconsToDisplay[ i ] != "0" ) {
                                    offsetAdditionalItemInfo += 36;
                                }
                            }
                        }
                    }

                    iconPositionY = yPositionOnScreen - Game1.smallFont.MeasureString( hoverItem.getDescription() ).Y - 89 - offsetAdditionalItemInfo;

                // If Toolbar is on the Top
                } else {
                    iconPositionY = Game1.getMousePosition().Y + 112;
                    fixTopX = 16;

                }

                b.Draw( Game1.debrisSpriteSheet, new Vector2( iconPositionX - fixTopX, iconPositionY ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.debrisSpriteSheet, 8, 16, 16 ) ), Color.White, 0f, new Vector2( 8f, 8f ), ( float ) Game1.pixelZoom, SpriteEffects.None, 0.95f );


                // Draw harvest icon
                if( isDrawingHarvestPrice ) {
                    Rectangle spriteRectangle = new Rectangle( 60, 428, 10, 10 );
                    Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( iconPositionX + Game1.dialogueFont.MeasureString( sellForAmount ).X - 10 - fixTopX, iconPositionY - 20 ), spriteRectangle, Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.85f );
                }
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
