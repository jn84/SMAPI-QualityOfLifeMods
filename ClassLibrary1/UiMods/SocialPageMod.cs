using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Reflection;

namespace Demiacle_SVM.UiMods {

    /// <summary>
    /// The SocialPage with added features that fills hearts depending on friendship level and also holds the data to display map location of townsfolk
    /// </summary>
    class SocialPageMod : SocialPage {

        public List<ClickableTextureComponent> friendNames;
        public List<OptionsCheckbox> checkboxes = new List<OptionsCheckbox>();
        public Dictionary<int, bool> savedData = new Dictionary<int, bool>();

        int panelWidth = 180;
        int panel1X = 144;

        public SocialPageMod( int x, int y, int width, int height ) : base( x, y, width, height ) {            

        }

        public override void draw( SpriteBatch b ) {
            base.draw( b );            

            // Keep just in case
            // Draw Tabs
            // Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( this.xPositionOnScreen - panel1X + 60, this.yPositionOnScreen + 20 ), new Rectangle( 1 * 16, 368, 16, 16 ), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f );
            
            // Draw Panel
            Game1.drawDialogueBox( this.xPositionOnScreen - panel1X, this.yPositionOnScreen, panelWidth, this.height, false, true );
            
            // Draw Content
            int slotPosition = ( int ) typeof( SocialPage ).GetField( "slotPosition", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( this );
            int offsetY = 0;
            for( int i = slotPosition; i < slotPosition + 5; i++ ) {

                // Safety check... this exists inside the client so lets reproduce here just in case
                if( i > friendNames.Count ) {
                    return;
                }

                // Set Checkbox position - TODO this should be removed from the drawing method if possible
                checkboxes[ i ].bounds.X = this.xPositionOnScreen - 50;
                checkboxes[ i ].bounds.Y = this.yPositionOnScreen + 130 + offsetY;

                // Draw Checkbox
                checkboxes[ i ].draw( Game1.spriteBatch, 0, 0 );
                offsetY += 112;

                // Set color for magnifying glass
                Color magnifyingGlassColor = Color.Gray;
                if( checkboxes[i].isChecked) {
                    magnifyingGlassColor = Color.White;
                }

                // Draw Magnifying glasses
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( checkboxes[ i ].bounds.X -50, checkboxes[ i ].bounds.Y ), new Rectangle( 80, 0, 16, 16 ), magnifyingGlassColor, 0f, Vector2.Zero, 3, SpriteEffects.None, 1f );

                // Keep just in case
                // Draw Large Heart
                // Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( checkboxes[ i ].bounds.X + heartOffsetX, checkboxes[ i ].bounds.Y ), new Rectangle( 218, 428, 7, 6 ), Color.White, 0f, Vector2.Zero, 8, SpriteEffects.None, 0.88f );
                                              
                // Fill Hearts
                int friendshipPoints = 0;
                int friendshipLevel = 0;

                if( Game1.player.friendships.ContainsKey( friendNames[ i ].name ) ) {
                    friendshipPoints = Game1.player.friendships[ friendNames[ i ].name ][ 0 ] % 250;
                    friendshipLevel = Game1.player.friendships[ friendNames[ i ].name ][ 0 ] / 250;                    
                }

                int heartLevelOffsetX = 32 * friendshipLevel;
                
                drawHeartFill( friendshipLevel, friendshipPoints, checkboxes[i].bounds );

                // Draw line below boxes omitting the last box... Hacky but W/E
                if( offsetY != 560 ) {                    
                    Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( checkboxes[i].bounds.X - 50, checkboxes[ i ].bounds.Y + 72, panelWidth / 2 - 6, 4 ), Color.SaddleBrown );
                    Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( checkboxes[ i ].bounds.X - 50, checkboxes[ i ].bounds.Y + 76, panelWidth / 2 - 6, 4 ), Color.BurlyWood );                    
                }
            }            
        }

        /// <summary>
        /// Draws a fill for hearts based on friendship level.
        /// </summary>
        /// <param name="friendshipLevel">The current heart level. This will shift the heart fill to the right depending on level</param>
        /// <param name="friendshipPoints">This is the amount of friendship points for this level</param>
        /// <param name="bounds">The checkbox bounds. This is just an easier way to draw y axis</param>
        private void drawHeartFill( int friendshipLevel, int friendshipPoints, Rectangle bounds ) {

            // 12 amount of amountOfPixelsToFill is the max
            // Ratios are a tad off so the last point is only half as much... its fine
            int amountOfPixelsToFill = friendshipPoints / 20;
            int heartLevelOffsetX = 32 * friendshipLevel;

            int[,] heartFillArray = {
                 { 1, 1, 0, 1, 1, },
                 { 1, 1, 1, 1, 1, },
                 { 0, 1, 1, 1, 0, },
                 { 0, 0, 1, 0, 0, }
            };
            
            // Draw the squares from bottom to top and left to right
            for( int row = 3; row >= 0; row-- ) {
                for( int column = 0; column < 5; column++ ) {
                    if( amountOfPixelsToFill < 1 ) {
                        return;
                    }

                    if( heartFillArray[ row, column ] == 1) {
                        Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( this.xPositionOnScreen + 316 + heartLevelOffsetX + (column * 4), bounds.Y + 14 + ( row * 4 ), 4, 4 ), Color.Crimson );
                        amountOfPixelsToFill--;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the value of the checkbox when clicked and saves its state to be used with the mapMod
        /// </summary>
        public override void receiveLeftClick( int x, int y, bool playSound = true ) {
            base.receiveLeftClick( x, y, playSound );

            foreach( OptionsCheckbox checkBox in checkboxes ) { 
                if( checkBox.bounds.Contains( x, y ) ) {
                    checkBox.receiveLeftClick( x, y );
                    savedData[ checkBox.whichOption ] = checkBox.isChecked;

                }
            }
        }

        public override void receiveRightClick( int x, int y, bool playSound = true ) {
         
        }        

        /// <summary>
        /// Copies the data from the supplied SocialPage to this instance.
        /// </summary>
        internal void Copy( SocialPage socialPage ) {
            
            FieldInfo[] fields = typeof( SocialPage ).GetFields( BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public );

            foreach( FieldInfo field in fields ) {
                var fieldToCopy = field.GetValue( socialPage );
                field.SetValue( this, fieldToCopy );
            }
        }              

    }
}
