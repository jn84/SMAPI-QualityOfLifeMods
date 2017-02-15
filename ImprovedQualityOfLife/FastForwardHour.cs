using System;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Demiacle.ImprovedQualityOfLife {
    internal class FastForwardHour {
        public FastForwardHour() {
            ControlEvents.KeyPressed += displayFastForwardDialogureOnPressX;
        }

        private void displayFastForwardDialogureOnPressX( object sender, EventArgsKeyPressed e ) {

            if( e.KeyPressed == Keys.V && Game1.activeClickableMenu == null && Game1.eventUp == false ) {

                var responses = new List<Response>();

                responses.Add( new Response( "yes", "yes" ) );
                responses.Add( new Response( "no", "no" ) );
                //Game1.drawObjectQuestionDialogue( "Wait an hour?", responses );

      //          var fastForwardMethod = typeof( FastForwardHour ).GetMethod( "removeFadeOut" );
    //            object[] parametersForCreateQuestionDialogue = { "Wait an hour?", responses.ToArray(), fastForwardMethod };

                // Must use generic method from game location
  //              Type[] paramTypes = new Type[] { typeof( string ), typeof( Response[] ), typeof( GameLocation.afterQuestionBehavior ) };


//                var gameLocationCreatueQuestion = typeof( GameLocation ).GetMethod( "createQuestionDialogue", paramTypes );

                //var ftn = gameLocationCreatueQuestion.MethodHandle.GetFunctionPointer();
                //var func = ( Action<string, Response[], GameLocation.afterQuestionBehavior> ) Activator.CreateInstance( typeof(Action<string,Response[],GameLocation.afterQuestionBehavior>), Game1.currentLocation, ftn );
                //func( "Wait an hour?", responses.ToArray(), fastForwardTime );



                //gameLocationCreatueQuestion.Invoke( (Game1.currentLocation as GameLocation), parametersForCreateQuestionDialogue );
                
                Game1.currentLocation.createQuestionDialogue( "Wait an hour?", responses.ToArray(), fastForwardTime );
                Game1.currentLocation.lastQuestionKey = "";
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