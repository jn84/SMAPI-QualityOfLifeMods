using System;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace Demiacle.ImprovedQualityOfLife {
    internal class AlterTimeSpeed {

        private int amountOfTimeToAlterPerTenMinutes = 0;
        private int timePassedPerTenMinuteUpdate;
        private int timeOfDayToAlter;
        private List<int> optionTable = new List<int>();

        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        public AlterTimeSpeed() {

            GameEvents.UpdateTick += addRemoveTime;
            timer.Start();

            //TODO write test to check vs option
            optionTable.Add( -6000 );
            optionTable.Add( -5000 );
            optionTable.Add( -4000 );
            optionTable.Add( -3000 );
            optionTable.Add( -2000 );
            optionTable.Add( -1000 );
            optionTable.Add( 0000 );
            optionTable.Add( 1000 );
            optionTable.Add( 2000 );
            optionTable.Add( 3000 );
            optionTable.Add( 4000 );
            optionTable.Add( 5000 );
            optionTable.Add( 6000 );
            optionTable.Add( 7000 );
            optionTable.Add( 8000 );
            optionTable.Add( 9000 );
            optionTable.Add( 10000 );
            optionTable.Add( 11000 );
            optionTable.Add( 12000 );
            optionTable.Add( 13000 );
            optionTable.Add( 14000 );
            optionTable.Add( 15000 );
            optionTable.Add( 16000 );
            optionTable.Add( 17000 );
            optionTable.Add( 18000 );
            optionTable.Add( 19000 );
            optionTable.Add( 20000 );
            optionTable.Add( 30000 );
            optionTable.Add( 40000 );
            optionTable.Add( 50000 );
            optionTable.Add( 60000 );
        }

        private void addRemoveTime( object sender, EventArgs e ) {

            int option = ModEntry.modData.intOptions[ QualtyOfLifeModOptions.TIME_PER_TEN_MINUTE_OPTION ];
            amountOfTimeToAlterPerTenMinutes = optionTable[ option ];
            
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