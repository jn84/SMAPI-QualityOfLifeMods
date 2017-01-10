using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Demiacle_SVM.UiMods {
    internal class ExpPointDisplay {
        private float experiencePoints;
        private Vector2 position;
        private int alpha = 100;
        private float yOffset = 0;
       

        public ExpPointDisplay( float experiencePoints, Vector2 position ) {
            this.position = position;
            this.experiencePoints = experiencePoints;
        }

        internal void draw() {
            position.Y -= 0.5f;
            alpha -= 1;
            //insideColor _
            //borderColor *= alpha;
            //borderColor *= 0.5f;

            Color insideColor = Color.PaleTurquoise * ( alpha / 100f );
            Color borderColor = Color.DarkSlateGray * ( alpha / 100f );

            Game1.drawWithBorder( $"Exp {experiencePoints}", borderColor, insideColor, new Vector2( position.X - 28, position.Y - 130 ), 0, 0.8f, 0 );

            // Game1.spriteBatch.DrawString( Game1.smallFont, $"Exp{experiencePoints}", new Vector2( position.X - 28, position.Y - 130 ), borderColor );
        }

        internal bool isInvisible() {
            return ( alpha <= 2 );
        }
    }
}