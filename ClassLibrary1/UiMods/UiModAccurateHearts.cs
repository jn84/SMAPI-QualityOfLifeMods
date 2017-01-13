using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using System.Reflection;

namespace DemiacleSvm.UiMods {

    /// <summary>
    /// SocialPage overwriting handler. Also holds data between menu calls
    /// </summary>
    class UiModAccurateHearts : UiModWithOptions {
        
        SocialPageMod socialPage;
        public const string SHOW_HEART_FILLS = "Show heart fills and npc locations";

        public UiModAccurateHearts() {
            addOption( SHOW_HEART_FILLS, toggleVisibleHearts );
        }
        
        /// <summary>
        /// Overwrites the SocialPage with my SocialPageMod and creates and initiates checkboxes.
        /// </summary>
        internal void OnMenuChange( object sender, EventArgsClickableMenuChanged e ) {

            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }
            
            // Get pages from GameMenu            
            var pages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );
            
            // Override socialPage with a fresh socialPageMod
            for( int k = 0; k < pages.Count; k++ ) {

                // Page[k] looks to be created every time the menu is opened so this needs to override the page every time
                if( pages[k] is SocialPage ) {

                    socialPage = new SocialPageMod( pages[k].xPositionOnScreen, pages[ k ] .yPositionOnScreen, pages[ k ].width, pages[ k ].height );

                    socialPage.Copy( ( SocialPage ) pages[ k ] );

                    socialPage.friendNames = ( List<ClickableTextureComponent> ) typeof( SocialPage ).GetField( "friendNames", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( socialPage );
                    
                    pages[ k ] = socialPage;

                }
                
            }

            socialPage.checkboxes.Clear();

            foreach( ClickableTextureComponent friend in socialPage.friendNames ) {

                int optionIndex = friend.name.GetHashCode();

                var checkbox = new OptionsCheckbox( "", optionIndex );
                socialPage.checkboxes.Add( checkbox );

                // Disable checkbox if player has not talked to npc yet
                if( !(Game1.player.friendships.ContainsKey( friend.name )) ) {
                    checkbox.greyedOut = true;
                    checkbox.isChecked = false;
                }

                // Ensure an entry exists
                if(  ModEntry.modData.locationOfTownsfolkOptions.ContainsKey( optionIndex  ) == false ) {
                    ModEntry.modData.locationOfTownsfolkOptions.Add( optionIndex, false );
                }

                checkbox.isChecked = ModEntry.modData.locationOfTownsfolkOptions[ optionIndex ];
            }
        }

        public void toggleVisibleHearts( bool setting ) {

            if( setting ) {
                MenuEvents.MenuChanged -= OnMenuChange;
                MenuEvents.MenuChanged += OnMenuChange;
            } else {
                MenuEvents.MenuChanged -= OnMenuChange;
            }

        }

    }
}
