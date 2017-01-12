using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;

namespace DemiacleSvm.UiMods {

    /// <summary>
    /// Handler that overrides Toolbar and InventoryMenu
    /// </summary>
    class UiModItemRolloverInformation :UiModWithOptions {

        public UiModItemRolloverInformation() {
            GraphicsEvents.OnPostRenderGuiEvent += onPostRenderEvent;
            MenuEvents.MenuChanged += onMenuChange;
        }

        //Change to on menu change
        internal void onPostRenderEvent( object sender, EventArgs e ) {
            
            // Override toolbars
            for( int j = 0; j < Game1.onScreenMenus.Count; j++ ) {
                if( Game1.onScreenMenus[ j ] is Toolbar ) {
                    ToolbarMod toolBarMod = new ToolbarMod();
                    toolBarMod.Copy( ( Toolbar ) Game1.onScreenMenus[ j ] );

                    Game1.onScreenMenus[ j ]  =  toolBarMod;
                }
            }

                     
        }

        internal void onMenuChange( object sender, EventArgsClickableMenuChanged e ) {

            if( Game1.activeClickableMenu is GameMenu ) {
                GameMenu gameMenu = ( GameMenu ) Game1.activeClickableMenu;

                // Get pages from GameMenu            
                List<IClickableMenu> pages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );

                // Overwrite Inventory Menu
                for( int i = 0; i < pages.Count; i++ ) {
                    if( pages[ i ] is InventoryPage ) {
                        InventoryPage inventoryPage = ( InventoryPage ) pages[ i ];
                        InventoryPageMod inventoryPageMod = new InventoryPageMod( inventoryPage.xPositionOnScreen, inventoryPage.yPositionOnScreen, inventoryPage.width, inventoryPage.height );
                        DemiacleUtility.copyFields( inventoryPageMod, inventoryPage );

                        pages[ i ] = inventoryPageMod;
                    }
                }
            }


            if( Game1.activeClickableMenu is ItemGrabMenu && !( Game1.activeClickableMenu is ItemGrabMenuMod ) ) {
                ItemGrabMenu gameMenu = ( ItemGrabMenu ) Game1.activeClickableMenu;

                InventoryMenu inventorMenu = ( InventoryMenu ) typeof( ItemGrabMenu ).GetField( "ItemsToGrabMenu", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public ).GetValue( gameMenu );

                ItemGrabMenuMod itemGrabMenuMod = new ItemGrabMenuMod( inventorMenu.actualInventory );
                DemiacleUtility.copyFields( itemGrabMenuMod, gameMenu );
                Game1.activeClickableMenu = itemGrabMenuMod;
            }
        }

        public void ToggleOption( string theOption, bool setting ) {
            throw new NotImplementedException();
        }
    }
}
