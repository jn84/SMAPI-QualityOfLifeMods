using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using StardewValley;

namespace Demiacle_SVM.UiMods {
    class UiModLuckOfDay {

        internal void onNewDay( object sender, EventArgsNewDay e ) {
            if( e.IsNewDay ) {
                string dialogue = "Nobody is sure whats going to happen today...";

                if( Game1.dailyLuck < 1d ) {
                    dialogue = $"You feel like you should go out and buy a lottery ticket... if that was your thing";
                }

                if( Game1.dailyLuck < 0.04d ) {
                    dialogue = $"Good things are going to happen today!... but probably not great things...";
                }

                if( Game1.dailyLuck < 0d ) {
                    dialogue = $"You get the feeling you shouldn't take any risks today";
                }

                if( Game1.dailyLuck < 0.04d ) {
                    dialogue = $"You should probably just stay home today...";
                }
                
                Game1.setDialogue( dialogue, true );
            }
        }

        internal void onNewDayC( object sender, EventArgsIntChanged e ) {
                string dialogue = "Nobody is sure whats going to happen today...";

                if( Game1.dailyLuck < 1d ) {
                    dialogue = $"You feel like you should go out and buy a lottery ticket... if that was your thing";
                }

                if( Game1.dailyLuck < 0.04d ) {
                    dialogue = $"Good things are going to happen today!... but probably not great things...";
                }

                if( Game1.dailyLuck < 0d ) {
                    dialogue = $"You get the feeling you shouldn't take any risks today";
                }

                if( Game1.dailyLuck < -0.04d ) {
                    dialogue = $"You should probably just stay home today...";
                }

                Game1.setDialogue( dialogue, true );
            }
        
    }
}
