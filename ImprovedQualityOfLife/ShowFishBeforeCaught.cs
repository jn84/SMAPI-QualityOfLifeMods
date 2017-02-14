using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace Demiacle.ImprovedQualityOfLife {
    internal class ShowFishBeforeCaught {

        private string fishThatJustBit = null;

        public ShowFishBeforeCaught() {
            GraphicsEvents.OnPostRenderEvent += drawString;
        }

        private void drawString( object sender, EventArgs e ) {
            if( Game1.player.CurrentTool is FishingRod && ( Game1.player.CurrentTool as FishingRod ).isReeling ) {

                var whichFish = ( int ) typeof( BobberBar ).GetField( "whichFish", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu);

                var x = Game1.activeClickableMenu.xPositionOnScreen;
                var y = Game1.activeClickableMenu.yPositionOnScreen - 50;

                string fishName;
                if( Game1.player.fishCaught.ContainsKey( whichFish ) ) {
                    fishName = Game1.objectInformation[ whichFish ].Split( '/' )[ 0 ];
                } else {
                    fishName = "???";
                }
                Game1.drawWithBorder( fishName, Color.White * 0.7f, Color.Black, new Vector2( x, y ) );

            }
        }

    }
}