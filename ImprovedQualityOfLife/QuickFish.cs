using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace Demiacle.ImprovedQualityOfLife {
    internal class QuickFish {
        public QuickFish() {
            GameEvents.UpdateTick += speedTimeWhenFishing;
        }

        private void speedTimeWhenFishing( object sender, EventArgs e ) {
            if( Game1.player.CurrentTool is FishingRod ) {

                var fishingRod = (FishingRod) Game1.player.CurrentTool;

                // Standard 10 minute mark is 7 seconds
                if( fishingRod.timeUntilFishingBite > 7000 && fishingRod.hit == false && fishingRod.isReeling == false && fishingRod.pullingOutOfWater == false && fishingRod.fishCaught == false ) {
                    fishingRod.timeUntilFishingBite -= 7000;
                    Game1.performTenMinuteClockUpdate();
                }
            }
        }
    }
}