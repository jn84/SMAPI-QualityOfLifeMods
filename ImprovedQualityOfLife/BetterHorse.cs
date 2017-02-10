using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;

namespace Demiacle.ImprovedQualityOfLife {
    internal class BetterHorse : Horse {
        public BetterHorse() : base( 0, 0 ) {
            speed = 9;
        }

        public override Rectangle GetBoundingBox() {
            Rectangle boundingBox = base.GetBoundingBox();

            if( mounting ) {
                return boundingBox;
            }

            boundingBox.Inflate( -14 - Game1.pixelZoom, 0);
            return boundingBox;
        }


    }
}