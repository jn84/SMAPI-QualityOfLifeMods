using System;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System.Collections.Generic;

namespace Demiacle.ImprovedQualityOfLife {
    internal class FastForwardHour {
        public FastForwardHour() {
            ControlEvents.KeyPressed += displayFastForwardDialogureOnPressX;
        }

        private void displayFastForwardDialogureOnPressX( object sender, EventArgsKeyPressed e ) {

            if( e.KeyPressed == Keys.C && Game1.activeClickableMenu == null && Game1.eventUp == false ) {

                var responses = new List<Response>();

                responses.Add( new Response( "yes", "yes" ) );
                responses.Add( new Response( "no", "no" ) );
                Game1.drawObjectQuestionDialogue( "Wait an hour?", responses );
                Game1.currentLocation.createQuestionDialogue( "Wait an hour?", responses.ToArray(), fastForwardTime );
                
            }

        }

        private void fastForwardTime( Farmer who, string whichAnswer ) {

            if( whichAnswer == "yes" ) {
                Game1.globalFadeToBlack( removeFadeOut );
            }

        }

        private void removeFadeOut() {

            for( int i = 0; i < 6; i++ ) {
                Game1.performTenMinuteClockUpdate();
            }

            Game1.globalFadeToClear();

        }

    }
}