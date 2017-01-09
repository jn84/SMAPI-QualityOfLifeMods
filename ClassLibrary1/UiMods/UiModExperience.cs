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
using System.Media;
using Microsoft.Xna.Framework.Audio;
using System.Runtime.InteropServices;
using System.IO;

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
    
    /// <summary>
    /// The mod that shows an experienceBar and plays an animation on level up
    /// </summary>
    class UiModExperience {

        private int maxBarWidth = 175;

        private int currentLevelIndex = 4;
        private int levelUpIndex = 0;
        private int levelOfCurrentlyDisplayedExp = 0;
        float currentExp = 0;
        

        private static readonly int timeBeforeExperienceBarFade = 8000;
        private int lengthOfLevelUpPause = 2000;

        // New colors created to allow manipulation here
        Color iconColor =  new Color( Color.White.ToVector4() );
        Color expFillColor = new Color( Color.Azure.ToVector4() );        

        Item previousItem = null;

        private bool shouldDrawExperienceBar = false;
        private bool shouldDrawLevelUp = false;

        SoundEffectInstance se;
        Timer timerToDissapear = new Timer();

        Rectangle levelUpIconRectangle;

        public UiModExperience() {
            Stream soundfile = TitleContainer.OpenStream( @"Mods\\Demiacle_SVM\\LevelUp.wav" );
            SoundEffect soundEffect = SoundEffect.FromStream( soundfile );
            se = soundEffect.CreateInstance();
            se.Volume = 1;
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

            Item currentItem = Game1.player.CurrentItem;

            Rectangle spriteRectangle = new Rectangle( 10, 428, 10, 10 );
            int currentLevel = 0;

            // Display exp type depending on item
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


            // If primary item is not selected
            // Display farming exp or foraging exp depending on current location
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
            int expAlreadyEarnedFromPreviousLevels = 0;

            levelOfCurrentlyDisplayedExp = currentLevel;

            // Sets the exp for next level and the exp that has already been obtained based on current level
            switch( levelOfCurrentlyDisplayedExp ) {
                case 0:
                    expRequiredToLevel = 100;
                    expAlreadyEarnedFromPreviousLevels = 0;
                    break;
                case 1:
                    expRequiredToLevel = 380;
                    expAlreadyEarnedFromPreviousLevels = 100;
                    break;
                case 2:
                    expRequiredToLevel = 770;
                    expAlreadyEarnedFromPreviousLevels = 380;
                    break;
                case 3:
                    expRequiredToLevel = 1300;
                    expAlreadyEarnedFromPreviousLevels = 770;
                    break;
                case 4:
                    expRequiredToLevel = 2150;
                    expAlreadyEarnedFromPreviousLevels = 1300;
                    break;
                case 5:
                    expRequiredToLevel = 3300;
                    expAlreadyEarnedFromPreviousLevels = 2150;
                    break;
                case 6:
                    expRequiredToLevel = 4800;
                    expAlreadyEarnedFromPreviousLevels = 3300;
                    break;
                case 7:
                    expRequiredToLevel = 6900;
                    expAlreadyEarnedFromPreviousLevels = 4800;
                    break;
                case 8:
                    expRequiredToLevel = 10000;
                    expAlreadyEarnedFromPreviousLevels = 6900;
                    break;
                case 9:
                    expRequiredToLevel = 15000;
                    expAlreadyEarnedFromPreviousLevels = 10000;
                    break;

                default:
                case 10:
                    // Max level or bug so disable showing exp
                    return;
            }

            float nextExp = Game1.player.experiencePoints[ currentLevelIndex ] - expAlreadyEarnedFromPreviousLevels;
            
            // If exp is gained or current item is switched then display exp and start dissapearance timer.
            if( currentExp != nextExp || previousItem != currentItem ) {
                timerToDissapear.Interval = timeBeforeExperienceBarFade;
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

            //shift display if game view has black borders
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

            //Level Up text
            if( shouldDrawLevelUp) {
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( Game1.player.getLocalPosition( Game1.viewport ).X - 74, Game1.player.getLocalPosition( Game1.viewport ).Y - 130 ), levelUpIconRectangle, iconColor, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.85f );
                Game1.drawWithBorder( "Level Up", Color.DarkSlateGray, Color.PaleTurquoise, new Vector2( Game1.player.getLocalPosition( Game1.viewport ).X - 28, Game1.player.getLocalPosition( Game1.viewport ).Y - 130 ) );
            }



        }

        /// <summary>
        /// Pauses the game, shows Level Up text and plays a chime, and unpauses after some time;
        /// </summary>
        internal void onLevelUp( object sender, EventArgsLevelUp e ) {
            
            switch( e.Type ) {
                case EventArgsLevelUp.LevelType.Combat:
                    levelUpIconRectangle = new Rectangle( 120, 428, 10, 10 );
                    break;
                case EventArgsLevelUp.LevelType.Farming:
                    levelUpIconRectangle = new Rectangle( 10, 428, 10, 10 );
                    break;
                case EventArgsLevelUp.LevelType.Fishing:
                    levelUpIconRectangle = new Rectangle( 20, 428, 10, 10 );
                    break;
                case EventArgsLevelUp.LevelType.Foraging:
                    levelUpIconRectangle = new Rectangle( 60, 428, 10, 10 );
                    break;
                case EventArgsLevelUp.LevelType.Mining:
                    levelUpIconRectangle = new Rectangle( 30, 428, 10, 10 );
                    break;
            }

            Game1.paused = true;

            shouldDrawLevelUp = true;

            timerToDissapear.Interval = timeBeforeExperienceBarFade;
            timerToDissapear.Start();
            shouldDrawExperienceBar = true;


            float prevAmbientVolume = Game1.options.ambientVolumeLevel;
            float prevMusicVolume = Game1.options.musicVolumeLevel;

            if( prevMusicVolume > 0.01f ) {
                se.Volume = Math.Min( 1, ( prevMusicVolume + 0.3f ) );
            } else {
                se.Volume = 0;
            }
            
            Task.Factory.StartNew( () => {
                System.Threading.Thread.Sleep( 100 );

                Game1.musicCategory.SetVolume( Math.Max( 0, Game1.options.musicVolumeLevel - 0.3f ) );
                Game1.ambientCategory.SetVolume( Math.Max( 0, Game1.options.ambientVolumeLevel - 0.3f ) );
                
                se.Play();
            } );

            Task.Factory.StartNew( () => {
                System.Threading.Thread.Sleep( lengthOfLevelUpPause );
                Game1.paused = false;
                shouldDrawLevelUp = false;


                Game1.musicCategory.SetVolume( prevMusicVolume );
                Game1.ambientCategory.SetVolume( prevAmbientVolume );
            } );
          
        }

        

        /// <summary>
        /// Changes the type of experience to display
        /// </summary>
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
        
    }
}
