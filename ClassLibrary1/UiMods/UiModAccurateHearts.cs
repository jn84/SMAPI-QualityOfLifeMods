using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using System.Reflection;

namespace Demiacle_SVM.UiMods {

    /// <summary>
    /// SocialPage overwriting handler. Necessary because the SocialPageMod cannot instantiate itself... maybe with a static method... hmmm
    /// </summary>
    class UiModAccurateHearts {
        
        SocialPageMod socialPage;

        public Dictionary<int, bool> savedData = new Dictionary<int, bool>();
        
        /// <summary>
        /// Overwrites the SocialPage with my SocialPageMod and creates and initiates checkboxes.
        /// </summary>
        internal void onMenuChange( object sender, EventArgsClickableMenuChanged e ) {

            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }
            
            // Get pages from GameMenu            
            List<IClickableMenu> pages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );
            
            // Override socialPage with a fresh socialPageMod
            for( int k = 0; k < pages.Count; k++ ) {

                // Page[k] looks to be created every time the menu is opened so this needs to override the page every time
                if( pages[k] is SocialPage ) {

                    socialPage = new SocialPageMod( pages[k].xPositionOnScreen, pages[ k ] .yPositionOnScreen, pages[ k ].width, pages[ k ].height );

                    socialPage.Copy( ( SocialPage ) pages[ k ] );

                    socialPage.savedData = this.savedData;
                    
                    socialPage.friendNames = ( List<ClickableTextureComponent> ) typeof( SocialPage ).GetField( "friendNames", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( socialPage );
                    
                    pages[ k ] = socialPage;

                }
            }

            socialPage.checkboxes.Clear();

            foreach( ClickableTextureComponent friend in socialPage.friendNames ) {

                int optionIndex = friend.name.GetHashCode();

                OptionsCheckbox checkbox = new OptionsCheckbox( "", optionIndex );
                socialPage.checkboxes.Add( checkbox );

                if( savedData.ContainsKey( optionIndex ) ) {
                    checkbox.isChecked = savedData[ optionIndex ];
                }

            }
        }
    }
}
