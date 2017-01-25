using System;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace ModToolbox {
    internal class SpriteSheetFinder : IClickableMenu {

        private int positionOfTextureX = 0;
        private int positionOfTextureY = 0;
        private bool isRightClicking = false;
        private bool isLeftClicking = false;
        private List<Texture2D> allTextures = new List<Texture2D>();
        private List<string> textureLocations = new List<string>();
        private int currentTextureIndex = 0;

        private int scale = 4;

        private Texture2D currentTexture = Game1.mouseCursors;

        public SpriteSheetFinder(  ) {
            ControlEvents.MouseChanged += releaseRightClick;
            string test = Game1.content.RootDirectory;

            string exeDirectory = AppDomain.

            bool ignore = true;
            if ( ignore == false ) { 
            textureLocations.Add( Path.Combine( "Minigames", "Clouds" ) );
            textureLocations.Add( "LooseSprites\\font_colored" );
            textureLocations.Add( "LooseSprites\\font_bold" );
            textureLocations.Add( "LooseSprites\\hoverBox" );
            textureLocations.Add( "TileSheets\\Projectiles" );
            textureLocations.Add( "TileSheets\\weapons" );
            textureLocations.Add( "Animals\\BabySheep" );
            textureLocations.Add( "Animals\\BabyGoat" );
            textureLocations.Add( "Animals\\BabyPig" );
            textureLocations.Add( "Animals\\BabyWhiteBlackCow" );
            textureLocations.Add( "Animals\\Cow" );
            textureLocations.Add( "Animals\\Duck" );
            textureLocations.Add( "Animals\\BabyDuck" );
            textureLocations.Add( "LooseSprites\\Bat" );
            textureLocations.Add( "LooseSprites\\Cursors" );
            textureLocations.Add( "Animals\\Chicken" );
            textureLocations.Add( "Animals\\White Chicken" ); // questionable
            //textureLocations.Add( "Buildings\\" ); need to find names for all buildings 
            textureLocations.Add( "LooseSprites\\robinAtWork" );
            textureLocations.Add( "Animals\\cat" );
            textureLocations.Add( "Animals\\dog" );
            textureLocations.Add( "Animals\\horse" );
            textureLocations.Add( "Characters\\Junimo" );
            textureLocations.Add( "Animals\\Dinosaur" );
            textureLocations.Add( "Animals\\BabyRabbit" );
            textureLocations.Add( "Animals\\BabyBrownChicken" );
            textureLocations.Add( "Animals\\BabyBrownChicken" );
            textureLocations.Add( "Animals\\BabyWhiteChicken" );
            textureLocations.Add( "LooseSprites\\textBox" );
            textureLocations.Add( "Buildings\\houses" );
            textureLocations.Add( "Characters\\Farmer\\farmer_girl_base" );
            textureLocations.Add( "Characters\\Farmer\\farmer_base" );
            textureLocations.Add( "Characters\\Farmer\\hairstyles" );
            textureLocations.Add( "Characters\\Farmer\\shirts" );
            textureLocations.Add( "Characters\\Farmer\\hats" );
            textureLocations.Add( "Characters\\Farmer\\accessories" );

            textureLocations.Add( "Characters\\Farmer\\shoeColors" );
            textureLocations.Add( "Characters\\Farmer\\skinColors" );
            textureLocations.Add( "Portraits\\Gil" );
            textureLocations.Add( "Maps\\CommunityCenter_Ruins" );
            textureLocations.Add( "Characters\\Krobus" );
            textureLocations.Add( "LooseSprites\\Billboard" );
            textureLocations.Add( "LooseSprites\\buildingPlacementTiles" );
            textureLocations.Add( "LooseSprites\\chatBox" );
            textureLocations.Add( "Characters\\farmergirl" );
            textureLocations.Add( "LooseSprites\\farmMap" );
            textureLocations.Add( "Data\\Fish" );
            textureLocations.Add( "Characters\\Clint" );
            //textureLocations.Add();
            foreach( var location in textureLocations ) {
                allTextures.Add( Game1.content.Load<Texture2D>( location) );
            }

            }


            textureLocations.Add( "Characters" );
            //textureLocations.Add();
            foreach( var location in textureLocations ) {
                allTextures.Add( Game1.content.Load<Texture2D>( location ) );
            }

            allTextures.Add( Game1.objectSpriteSheet );
            allTextures.Add( Game1.toolSpriteSheet );
            allTextures.Add( Game1.cropSpriteSheet );
            //allTextures.Add( Game1.mailboxTexture );
            allTextures.Add( Game1.emoteSpriteSheet );
            allTextures.Add( Game1.debrisSpriteSheet );
            //allTextures.Add( Game1.toolIconBox );
            //allTextures.Add( Game1.rainTexture );
            allTextures.Add( Game1.bigCraftableSpriteSheet );
            //allTextures.Add( Game1.swordSwipe );
            //allTextures.Add( Game1.swordSwipeDark );
            allTextures.Add( Game1.buffsIcons );
            //allTextures.Add( Game1.daybg );
            //allTextures.Add( Game1.nightbg );
            //allTextures.Add( Game1.logoScreenTexture );
            //allTextures.Add( Game1.tvStationTexture );
            //allTextures.Add( Game1.cloud );
            allTextures.Add( Game1.menuTexture );
            //allTextures.Add( Game1.lantern );
            //allTextures.Add( Game1.windowLight );
            //allTextures.Add( Game1.sconceLight );
            //allTextures.Add( Game1.cauldronLight );
            //allTextures.Add( Game1.shadowTexture );
            allTextures.Add( Game1.mouseCursors );
            //allTextures.Add( Game1.indoorWindowLight );
            allTextures.Add( Game1.animations );
            //allTextures.Add( Game1.titleScreenBG );
            //allTextures.Add( Game1.logo );
            //allTextures.Add( Game1.fadeToBlackRect );
            //allTextures.Add( Game1.staminaRect );
            //allTextures.Add( Game1.currentCoopTexture );
            //allTextures.Add( Game1.currentBarnTexture );
            //allTextures.Add( Game1.currentHouseTexture );
            //allTextures.Add( Game1.greenhouseTexture );
            //allTextures.Add( Game1.littleEffect );
    }

        private void releaseRightClick( object sender, EventArgsMouseStateChanged e ) {
            if ( e.NewState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released ) {
                isRightClicking = false;
            }
        }

        public override void draw( SpriteBatch b ) {
            
            // Clear screen
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( 0, 0, Game1.viewport.Width, Game1.viewport.Height ), Color.Black );

            // Draw Texture Chooser
            Texture2D currentTexture = allTextures[ currentTextureIndex ];
            Game1.spriteBatch.Draw( currentTexture, new Vector2( positionOfTextureX, positionOfTextureY ), new Rectangle( 0, 0, currentTexture.Width, currentTexture.Height ), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1 );


            int tooltipWidth = 900;
            int tooltipHeight = 100;

            int tooltipPositionX = Game1.viewport.Width / 2 - tooltipWidth / 2;
            int tooltipPositionY = Game1.viewport.Height - 100;

            // Draw tooltips
            IClickableMenu.drawTextureBox( Game1.spriteBatch, tooltipPositionX, tooltipPositionY, tooltipWidth, tooltipHeight, Color.White );
            Game1.spriteBatch.DrawString( Game1.smallFont, "Drag Left Mouse : Select Texture | Drag Right Mouse : Reposition texture", new Vector2( tooltipPositionX + 20, tooltipPositionY + 24 ), Color.Black );
            Game1.spriteBatch.DrawString( Game1.smallFont, "Up Arrow : Increase  Scale | Down Arrow : Decrease Scale", new Vector2( tooltipPositionX + 100, tooltipPositionY + 54 ), Color.Black );

            // Redraw mouse
            Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getOldMouseX(), ( float ) Game1.getOldMouseY() ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16 ) ), Color.White, 0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );
            ;
        }

        public override void update( GameTime time ) {
            base.update( time );
            
            if( isRightClicking ) {
                int deltaX = Game1.getMouseX() - Game1.getOldMouseX();
                int deltaY = Game1.getMouseY() - Game1.getOldMouseY();

                positionOfTextureX += deltaX;
                positionOfTextureY += deltaY;
            }

        }

        public override void receiveScrollWheelAction( int direction ) {
            base.receiveScrollWheelAction( direction );

            if ( direction > 0 ) {
                currentTextureIndex++;

                if ( currentTextureIndex > allTextures.Count - 1 ) {
                    currentTextureIndex = 0;
                }
            }

            if ( direction < 0 ) {
                currentTextureIndex--;
                if ( currentTextureIndex < 0 ) {
                    currentTextureIndex = allTextures.Count - 1;
                }
            }

            positionOfTextureX = 0;
            positionOfTextureY = 0;

        }

        public override void receiveRightClick( int x, int y, bool playSound = true ) {
            isRightClicking = true;
        }

        public override void receiveLeftClick( int x, int y, bool playSound = true ) {
            base.receiveLeftClick( x, y, playSound );
            isLeftClicking = true;
        }

        public override void releaseLeftClick( int x, int y ) {
            base.releaseLeftClick( x, y );

            // post feedback that says text is copied to clipboard
        }

        public override void receiveKeyPress( Keys key ) {
            base.receiveKeyPress( key );

            if( key == Keys.Up ) {
                scale = Math.Min( scale + 1, 4 ); 
            }

            if( key == Keys.Down ) {
                scale = Math.Max( scale - 1, 1 );
            }

        }


    }
}