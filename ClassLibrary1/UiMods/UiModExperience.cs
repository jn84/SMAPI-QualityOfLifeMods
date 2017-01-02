using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using System.Timers;

namespace Demiacle_SVM.UiMods {

    /* Experience point indexes
     * 
     * Farming = 0
     * Fishing = 1
     * Foraging = 2
     * Mining = 3
     * Combat = 4
     * Luck = 5 
    */
    
    class UiModExperience {

        private int maxBarWidth = 175;
        private int currentLevelIndex = 4;
        private int levelOfCurrentlyDisplayedExp = 0;
        private static readonly int timeBeforeFade = 8000;
        private int timer = timeBeforeFade; //TODO on item change and on item use - set timer - when timer is 0 render nothing
        float currentExp = 0;
        Color iconColor = Color.White;
        Color expFillColor = Color.Azure;
        System.Timers.Timer timerToDissapear = new System.Timers.Timer();
        private bool shouldDrawExperienceBar = true;

        Item previousItem = null;

        public UiModExperience() {
            timerToDissapear.Elapsed += stopTimerAndFadeBarOut;
        }

        private void stopTimerAndFadeBarOut( object sender, ElapsedEventArgs e ) {
            timerToDissapear.Stop();
            shouldDrawExperienceBar = false;
        }

        internal void onPreRenderEvent( object sender, EventArgs e ) {

            
            

            //Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( 10, 10 ), new Rectangle( 0, 0, 10, 10 ), Color.Aqua ); DRAW A MOUSE CURSOR
            //Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( 0, 0, 64, 64 ), Color.Azure ); DRAW A RECTANGLE
            //Game1.drawWithBorder( "test", Color.Bisque, Color.Aquamarine, new Vector2( 64,64) ); TEXT WITH BORDER


            //Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) ( num5 + num113 - Game1.pixelZoom  ), ( float ) ( num4 ) ), new Rectangle?( new Rectangle( 145, 338, 14, 9 ) ), Color.Black * 0.35f, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.87f );
            //Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 200, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom -52 ), new Rectangle?( new Rectangle( 159, 338, 14, 9 ) ), Color.White * ( 1f ), 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.87f );
            //ClickableTextureComponent expBar = new ClickableTextureComponent( "T", new Rectangle( 100, 100, 14 * Game1.pixelZoom, 9 * Game1.pixelZoom ), ( string ) null, "blurb", Game1.mouseCursors, new Rectangle( 159, 338, 14, 9 ), ( float ) Game1.pixelZoom, true ) ;
            //expBar.draw( Game1.spriteBatch );

            

            //fill

            
            

            Item currentItem = Game1.player.CurrentItem;

            Rectangle spriteRectangle = new Rectangle( 10, 428, 10, 10 );
            int currentLevel = 0;

            if( currentItem is FishingRod ) {
                currentLevelIndex = 1;
                spriteRectangle = new Rectangle( 20, 428, 10, 10 );
                currentLevel = Game1.player.fishingLevel;

            } else if( currentItem is Axe ) {
                currentLevelIndex = 2;
                spriteRectangle = new Rectangle( 60, 428, 10, 10 );
                currentLevel = Game1.player.foragingLevel;

            } else if( currentItem is Pickaxe ) {
                currentLevelIndex = 3;
                spriteRectangle = new Rectangle( 30, 428, 10, 10 );
                currentLevel = Game1.player.miningLevel;

            } else if( currentItem is MeleeWeapon ) {
                currentLevelIndex = 4;
                spriteRectangle = new Rectangle( 120, 428, 10, 10 );
                currentLevel = Game1.player.combatLevel;


            // display farming exp or foraging exp depending on current location
            } else {

                if( Game1.currentLocation is Farm) {
                    currentLevelIndex = 0;
                    spriteRectangle = new Rectangle( 10, 428, 10, 10 );
                    currentLevel = Game1.player.farmingLevel;
                } else {
                    currentLevelIndex = 2;
                    spriteRectangle = new Rectangle( 60, 428, 10, 10 );
                    currentLevel = Game1.player.foragingLevel;
                }
            }

            float expRequiredToLevel = 1;
            int expToSubtract = 0;

            levelOfCurrentlyDisplayedExp = currentLevel;

            switch( levelOfCurrentlyDisplayedExp ) {
                case 0:
                    expRequiredToLevel = 100;
                    expToSubtract = 0;
                    break;
                case 1:
                    expRequiredToLevel = 380;
                    expToSubtract = 100;
                    break;
                case 2:
                    expRequiredToLevel = 770;
                    expToSubtract = 380;
                    break;
                case 3:
                    expRequiredToLevel = 1300;
                    expToSubtract = 770;
                    break;
                case 4:
                    expRequiredToLevel = 2150;
                    expToSubtract = 1300;
                    break;
                case 5:
                    expRequiredToLevel = 3300;
                    expToSubtract = 2150;
                    break;
                case 6:
                    expRequiredToLevel = 4800;
                    expToSubtract = 3300;
                    break;
                case 7:
                    expRequiredToLevel = 6900;
                    expToSubtract = 4800;
                    break;
                case 8:
                    expRequiredToLevel = 10000;
                    expToSubtract = 6900;
                    break;
                case 9:
                    expRequiredToLevel = 15000;
                    expToSubtract = 10000;
                    break;

                default:
                case 10:
                    // max level or bug so disable showing exp
                    return;
            }

            float nextExp = Game1.player.experiencePoints[ currentLevelIndex ] - expToSubtract;

            

            // If exp is gained or current item is switched then display exp and start timer.
            if( currentExp != nextExp || previousItem != currentItem ) {
                timerToDissapear.Interval = timeBeforeFade;
                timerToDissapear.Start();
                shouldDrawExperienceBar = true;
            }

            previousItem = currentItem;
            currentExp = nextExp;

            if( shouldDrawExperienceBar == false ) {
                return;
            }


            int barWidth = Convert.ToInt32( ( currentExp / expRequiredToLevel ) * maxBarWidth );

            float positionX = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 340;
            if( Game1.isOutdoorMapSmallerThanViewport() ) {
                int currentMapSize = ( Game1.currentLocation.map.Layers[ 0 ].LayerWidth * Game1.tileSize );
                float blackSpace = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - currentMapSize;
                positionX = positionX - ( blackSpace / 2 );
            }

            //border
            Game1.drawDialogueBox( (int) positionX, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 160, 240, 160, false, true );

            // Icon
            Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( (int) positionX - 32, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 68 ), spriteRectangle, iconColor , 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.85f );

            // Experience fill
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( (int) positionX + 32, Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63, barWidth, 30 ), expFillColor );
        }

        public void changeExperienceToDisplay( int index ) {
            Farmer farmer = Game1.player;
            currentLevelIndex = index;
            switch( index ) {
                case 0:
                    levelOfCurrentlyDisplayedExp = farmer.farmingLevel;
                    break;
                case 1:
                    levelOfCurrentlyDisplayedExp = farmer.fishingLevel;
                    break;
                case 2:
                    levelOfCurrentlyDisplayedExp = farmer.foragingLevel;
                    break;
                case 3:
                    levelOfCurrentlyDisplayedExp = farmer.miningLevel;
                    break;
                case 4:
                    levelOfCurrentlyDisplayedExp = farmer.combatLevel;
                    break;
            }
        }

        internal void onPostRenderEvent( object sender, EventArgs e ) {
        }
        
    }
}
