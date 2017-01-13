using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DemiacleSvm.UiMods {

    /// <summary>
    /// Handler that overrides Toolbar and InventoryMenu
    /// </summary>
    class UiModItemRolloverInformation :UiModWithOptions {


        public const string SHOW_EXTRA_ITEM_INFORMATION = "Show extra tooltip information";

        Item hoverItem;

        public UiModItemRolloverInformation() {
            addOption( SHOW_EXTRA_ITEM_INFORMATION, toggleOption );
        }

        private void toggleOption( bool setting ) {

            if ( setting ) {
                GraphicsEvents.OnPreRenderEvent -= removeDefaultHoverItems;
                GraphicsEvents.OnPostRenderEvent -= drawAdvancedToolip;

                GraphicsEvents.OnPostRenderEvent += drawAdvancedToolip;
                GraphicsEvents.OnPreRenderEvent += removeDefaultHoverItems;
            } else {
                GraphicsEvents.OnPostRenderEvent -= drawAdvancedToolip;
                GraphicsEvents.OnPreRenderEvent -= removeDefaultHoverItems;
            }

        }

        private void drawAdvancedToolip( object sender, EventArgs e ) {
            if( hoverItem == null ) {
                return;
            }

            string sellForAmount = "";
            string harvestPrice = "";

            if( hoverItem.salePrice() > 0 ) {
                sellForAmount = "\n  " + hoverItem.salePrice() / 2;

                if( hoverItem.canStackWith( hoverItem ) && hoverItem.getStack() > 1 ) {
                    sellForAmount += $" ({ hoverItem.salePrice() / 2 * hoverItem.getStack() })";
                }
            }

            bool isDrawingHarvestPrice = false;

            // Adds the price of the fully grown crop to the display text only if it is a seed
            if( hoverItem is StardewValley.Object && ( ( StardewValley.Object ) hoverItem ).type == "Seeds" && sellForAmount != "" ) {

                if( hoverItem.Name != "Mixed Seeds" || hoverItem.Name != "Winter Seeds" ) {
                    var crop = new Crop( hoverItem.parentSheetIndex, 0, 0 );
                    var debris = new Debris( crop.indexOfHarvest, Game1.player.position, Game1.player.position );
                    var item = new StardewValley.Object( debris.chunkType, 1 );
                    harvestPrice += $"    { item.price }";
                    isDrawingHarvestPrice = true;
                }

            }

            IClickableMenu.drawToolTip( Game1.spriteBatch, hoverItem.getDescription(), hoverItem.Name + sellForAmount + harvestPrice, hoverItem, false, -1, 0, -1, -1, null, -1 );
            string test = hoverItem.getDescription();

            // Draw coin
            if( sellForAmount != "" ) {

                float borderOffsetYForIcon = 16;
                float offsetHeight = Game1.smallFont.MeasureString( hoverItem.getDescription() + "\n objectType" ).Y + Game1.dialogueFont.MeasureString( "The height of 1 line" ).Y + borderOffsetYForIcon;

                // Offset by health and stamina gained text
                if( ( hoverItem as StardewValley.Object ).edibility != -300 ) {
                    offsetHeight += 38 * 2;

                    // Offset displayed buffs text
                    if( Game1.objectInformation[ ( hoverItem as StardewValley.Object ).parentSheetIndex ].Split( '/' ).Length >= 7 ) {
                        string[] buffIconsToDisplay = Game1.objectInformation[ ( hoverItem as StardewValley.Object ).parentSheetIndex ].Split( '/' )[ 6 ].Split( ' ' );
                        for( int i = 0; i < buffIconsToDisplay.Count(); i++ ) {
                            if( buffIconsToDisplay[ i ] != "0" ) {
                                offsetHeight += 38;
                            }
                        }
                    }
                }

                // compensates for the inacurate measure of text and borders
                float offsetYHoverBackground = 61;

                float heightThatMeasuresHoverBackground = Game1.smallFont.MeasureString( hoverItem.getDescription() + "\n objectType" ).Y + Game1.dialogueFont.MeasureString( "The height of \n 2 lines" ).Y +  offsetHeight - offsetYHoverBackground;

                float iconPositionY = 0;
                float fixTopX = 0;
                if( Game1.getMouseY() + heightThatMeasuresHoverBackground > Game1.viewport.Height ) {
                    iconPositionY = Game1.viewport.Height - offsetHeight;
                } else {
                    iconPositionY = Game1.getMousePosition().Y + 112;
                    fixTopX = 16;
                }

                float iconPositionX = Game1.getMousePosition().X + 78;

                Game1.spriteBatch.Draw( Game1.debrisSpriteSheet, new Vector2( iconPositionX - fixTopX, iconPositionY ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.debrisSpriteSheet, 8, 16, 16 ) ), Color.White, 0f, new Vector2( 8f, 8f ), ( float ) Game1.pixelZoom, SpriteEffects.None, 0.95f );
               
                // Draw harvest icon
                if( isDrawingHarvestPrice ) {
                    var spriteRectangle = new Rectangle( 60, 428, 10, 10 );
                    Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( iconPositionX + Game1.dialogueFont.MeasureString( sellForAmount ).X - 10 - fixTopX, iconPositionY - 20 ), spriteRectangle, Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.85f );
                }

            }

        }

        internal void removeDefaultHoverItems( object sender, EventArgs e ) {

            // Remove hovers from toolbar
            for( int j = 0; j < Game1.onScreenMenus.Count; j++ ) {
                if( Game1.onScreenMenus[ j ] is Toolbar ) {
                    var menu = Game1.onScreenMenus[ j ] as Toolbar;

                    hoverItem = ( Item ) typeof( Toolbar ).GetField( "hoverItem", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( menu );
                    typeof( Toolbar ).GetField( "hoverItem", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( menu, null );
                }
            }

            // Remove hovers from inventory
            if( Game1.activeClickableMenu is GameMenu ) {

                // Get pages from GameMenu            
                var pages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );

                // Overwrite Inventory Menu
                for( int i = 0; i < pages.Count; i++ ) {
                    if( pages[ i ] is InventoryPage ) {
                        var inventoryPage = ( InventoryPage ) pages[ i ];
                        hoverItem = ( Item ) typeof( InventoryPage ).GetField( "hoveredItem", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( inventoryPage );
                        typeof( InventoryPage ).GetField( "hoveredItem", BindingFlags.NonPublic | BindingFlags.Instance ).SetValue( inventoryPage, null );
                    }
                }
            }

            // Remove hovers from chests and shipping bin
            if( Game1.activeClickableMenu is ItemGrabMenu ) {
                var itemGrabMenu  = ( ItemGrabMenu ) Game1.activeClickableMenu;
                hoverItem = ( Item ) typeof( ItemGrabMenu ).GetField( "hoveredItem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public ).GetValue( itemGrabMenu );
                typeof( ItemGrabMenu ).GetField( "hoveredItem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public ).SetValue( itemGrabMenu, null );
            }

        }

    }
}
