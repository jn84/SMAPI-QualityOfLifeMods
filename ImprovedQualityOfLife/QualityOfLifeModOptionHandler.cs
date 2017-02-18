﻿using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Demiacle.ImprovedQualityOfLife {
    internal class QualityOfLifeModOptionHandler {

        QualtyOfLifeModOptions modOption = new QualtyOfLifeModOptions();

        public QualityOfLifeModOptionHandler() {
            ControlEvents.KeyPressed += onKeyPress;
        }

        private void onKeyPress( object sender, EventArgsKeyPressed e ) {

            if( Game1.activeClickableMenu is GameMenu ) {
                return;
            }

            if( e.KeyPressed == ModEntry.modConfig.alterTenMinuteKey ) {

                if( Game1.activeClickableMenu == modOption ) {
                    Game1.activeClickableMenu = null;
                    
                } else {
                    modOption.xPositionOnScreen = Game1.viewport.Width / 2 - modOption.width / 2;
                    modOption.yPositionOnScreen = Game1.viewport.Height / 2 - modOption.height / 2;
                    modOption.resetPosition();
                    Game1.activeClickableMenu = modOption;
                }

            }

        }

    }
}