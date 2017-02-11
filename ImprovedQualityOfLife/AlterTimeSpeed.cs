using System;
using StardewModdingAPI.Events;
using StardewValley;

namespace Demiacle.ImprovedQualityOfLife {
    internal class AlterTimeSpeed {

        private int amountOfTimeToAlterPerTenMinutes = 1000;
        private int timePassedPerTenMinuteUpdate;
        private int timeOfDayToAlter;

        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        public AlterTimeSpeed() {

            GameEvents.UpdateTick += addRemoveTime;
            timer.Start();
        }

        private void addRemoveTime( object sender, EventArgs e ) {
            amountOfTimeToAlterPerTenMinutes = -1000;
            // Reset counter every 10 minutes
            if( timeOfDayToAlter != Game1.timeOfDay ) {
                ModEntry.Log( $"10 minute length took {timer.ElapsedMilliseconds}" );
                timer.Reset();
                timer.Start();
                timeOfDayToAlter = Game1.timeOfDay;
                timePassedPerTenMinuteUpdate = 0;

                // Fast forward time if we are speeding up the clock
                if( amountOfTimeToAlterPerTenMinutes < 0 ) {
                    Game1.gameTimeInterval -= amountOfTimeToAlterPerTenMinutes;
                }
            }


            // If slowed enough time do nothing
            if( timePassedPerTenMinuteUpdate > amountOfTimeToAlterPerTenMinutes ) {
                return;
            }

            // Slow down time passed
            int timePassedThisTick = Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            timePassedPerTenMinuteUpdate += Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            Game1.gameTimeInterval -= timePassedThisTick;

        }

    }
}