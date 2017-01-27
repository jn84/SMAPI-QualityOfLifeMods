using System;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Threading;

namespace ModToolbox {
    internal class SpriteSheetFinder : IClickableMenu {

        private int positionOfTextureX = 0;
        private int positionOfTextureY = 0;
        private bool isRightClicking = false;
        private bool isLeftClicking = false;
        private List<Texture2D> allTextures = new List<Texture2D>();
        private List<string> contentLocation = new List<string>();
        private int currentTextureIndex = 0;

        private int scale = 4;

        private Texture2D currentTexture = Game1.mouseCursors;

        private int positionToCropX;
        private int positionToCropY;
        private int widthOfCrop;
        private int heightOfCrop;
        private bool isShifting = false;
        private bool isSnappingToTileSize = false;

        private int rightClickStartPositionX;
        private int rightClickStartPositionY;
        private int rightClickDeltaY;
        private int rightClickDeltaX;
        private TextBox clipboardFeedback = new TextBox( Game1.content.Load<Texture2D>( "LooseSprites\\textBox" ), ( Texture2D ) null, Game1.smallFont, Game1.textColor );

        System.Timers.Timer clipboardFadeTimer = new System.Timers.Timer();

        public SpriteSheetFinder(  ) {
            ControlEvents.MouseChanged += releaseRightClick;
            string test = Game1.content.RootDirectory;

            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

            getAllFilesInThisDirectory( exeDirectory + "\\Content" );

            for( int i = contentLocation.Count - 1; i >= 0; i-- ) {
                try {
                    allTextures.Add( Game1.content.Load<Texture2D>( contentLocation[ i ] ) );
                } catch ( ContentLoadException e ) {
                    contentLocation.RemoveAt( i );
                }
            }

            allTextures.Reverse();

            clipboardFeedback.Y = Game1.viewport.Height;
            clipboardFadeTimer.Elapsed += ( sender, e ) => clipboardFeedback.Y = Game1.viewport.Height;

        }

        private void getAllFilesInThisDirectory( string exeDirectory, string pathName = "" ) {
            var directoryInfo = new DirectoryInfo( exeDirectory );

            FileInfo[] xnbFiles = directoryInfo.GetFiles( "*.xnb" );

            foreach( var item in xnbFiles ) {
                contentLocation.Add( pathName + item.Name.Split( '.' )[0] );
            }

            // Search directory if any
            DirectoryInfo[] directoriesInsideThisDirectory = directoryInfo.GetDirectories();
            foreach( var item in directoriesInsideThisDirectory ) {
                getAllFilesInThisDirectory( item.FullName, pathName + item.Name + "\\" );
            }

            
        }

        private void releaseRightClick( object sender, EventArgsMouseStateChanged e ) {
            if ( e.NewState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released ) {
                isRightClicking = false;
            }
        }

        public override void draw( SpriteBatch b ) {
            
            // Clear screen
            Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( 0, 0, Game1.viewport.Width, Game1.viewport.Height ), Color.Black );

            // Draw Texture
            Texture2D currentTexture = allTextures[ currentTextureIndex ];
            Game1.spriteBatch.Draw( currentTexture, new Vector2( positionOfTextureX, positionOfTextureY ), new Rectangle( 0, 0, currentTexture.Width, currentTexture.Height ), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1 );

            // Draw highlighted selection
            Color color;
            if( isShifting ) {
                color = Color.Red;
            } else {
                color = Color.White;
            }

            if( isLeftClicking ) {
                Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( positionToCropX + positionOfTextureX - 1, positionToCropY + positionOfTextureY - 1, widthOfCrop + 1, 1 ), color );
                Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( positionToCropX + positionOfTextureX - 1, positionToCropY + positionOfTextureY - 1, 1, heightOfCrop + 1 ), color );
                Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( positionToCropX + positionOfTextureX + widthOfCrop, positionToCropY + positionOfTextureY - 1, 1, heightOfCrop + 1 ), color );
                Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( positionToCropX + positionOfTextureX - 1, positionToCropY + positionOfTextureY + heightOfCrop , widthOfCrop + 1, 1 ), color );
            }

            int tooltipWidth = 900;
            int tooltipHeight = 100;

            int tooltipPositionX = Game1.viewport.Width / 2 - tooltipWidth / 2;
            int tooltipPositionY = Game1.viewport.Height - 100;

            // Draw bottom tooltip
            IClickableMenu.drawTextureBox( Game1.spriteBatch, tooltipPositionX, tooltipPositionY, tooltipWidth, tooltipHeight, Color.White );
            Game1.spriteBatch.DrawString( Game1.smallFont, "Drag Left Mouse : Select Texture | Drag Right Mouse : Reposition texture", new Vector2( tooltipPositionX + 20, tooltipPositionY + 24 ), Color.Black );
            Game1.spriteBatch.DrawString( Game1.smallFont, "1, 2, 3, 4 : Rescale texture | up, down, left, right : Rescale selection", new Vector2( tooltipPositionX + 100, tooltipPositionY + 54 ), Color.Black );
            
            // Draw top right toolotip
            string currentTextureFileDisplay = "Current Texture: " + contentLocation[ currentTextureIndex ].Replace( @"\", @"-" );
            
            int tooltip2Width = (int) Game1.smallFont.MeasureString( currentTextureFileDisplay ).X + 40;
            int tooltip2Height = 70;

            int tooltip2PositionX = Game1.viewport.Width - tooltip2Width;
            int tooltip2PositionY = 20;

            IClickableMenu.drawTextureBox( Game1.spriteBatch, tooltip2PositionX, tooltip2PositionY, tooltip2Width, tooltip2Height, Color.White );
            Game1.spriteBatch.DrawString( Game1.smallFont, currentTextureFileDisplay, new Vector2( tooltip2PositionX + 20, tooltip2PositionY + 24 ), Color.Black );

            // Draw feedback
            if( isSnappingToTileSize ) {
                string text = "Snapping";
                int snappingWidth = (int) Game1.smallFont.MeasureString( text ).X + 40;
                int snappingHeight = (int) Game1.smallFont.MeasureString( text ).Y + 24;

                int snappingPositionX = Game1.viewport.Width / 2 - snappingWidth / 2;
                int snappingPositionY = Game1.viewport.Height - 140;
                
                IClickableMenu.drawTextureBox( Game1.spriteBatch, snappingPositionX, snappingPositionY, snappingWidth, snappingHeight, Color.White );
                Game1.spriteBatch.DrawString( Game1.smallFont, text, new Vector2( snappingPositionX + 16, snappingPositionY + 16), Color.Black );
            }
            // Draw feedback when copied to clipboard
            clipboardFeedback.Draw( b );

            // Redraw mouse
            Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getOldMouseX(), ( float ) Game1.getOldMouseY() ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16 ) ), Color.White, 0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );
        }

        public override void update( GameTime time ) {
            base.update( time );
            
            if( isRightClicking ) {


                int deltaX = Game1.getMouseX() - Game1.getOldMouseX();
                int deltaY = Game1.getMouseY() - Game1.getOldMouseY();

                if( isSnappingToTileSize ) {
                    rightClickDeltaX += deltaX;
                    rightClickDeltaY += deltaY;

                    positionOfTextureX = ( rightClickDeltaX / Game1.tileSize ) * 64;
                    positionOfTextureY = ( rightClickDeltaY / Game1.tileSize ) * 64;
                } else {
                    positionOfTextureX += deltaX;
                    positionOfTextureY += deltaY;
                }

            }

        }

        public override void receiveScrollWheelAction( int direction ) {
            base.receiveScrollWheelAction( direction );


            rightClickDeltaX = 0;
            rightClickDeltaY = 0;

            if ( direction < 0 ) {
                currentTextureIndex++;
                if ( currentTextureIndex > allTextures.Count - 1 ) {
                    currentTextureIndex = 0;
                }
            }

            if ( direction > 0 ) {
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
            rightClickStartPositionX = Game1.getMouseX();
            rightClickStartPositionY = Game1.getMouseY();
        }

        public override void receiveLeftClick( int x, int y, bool playSound = true ) {
            base.receiveLeftClick( x, y, playSound );
            isLeftClicking = true;

            // Snap to tiles
            if( isSnappingToTileSize ) {
                snapSelectionToTiles( ref x, ref y );
            }

            positionToCropX = x - positionOfTextureX;
            positionToCropY = y - positionOfTextureY;
            widthOfCrop = 0;
            heightOfCrop = 0;


        }

        public override void leftClickHeld( int x, int y ) {
            base.leftClickHeld( x, y );
            setCropSize( x, y );
        }


        private void snapSelectionToTiles( ref int x, ref int y ) {

            int xRemainder = x % Game1.tileSize;
            int yRemainder = y % Game1.tileSize;

            int amountOfTilesX = x / Game1.tileSize;
            int amountOfTilesY = y / Game1.tileSize;

            if( xRemainder > Game1.tileSize / 2 ) {
                amountOfTilesX++;
            }

            if( yRemainder > Game1.tileSize / 2 ) {
                amountOfTilesY++;
            }

            x = amountOfTilesX * Game1.tileSize;
            y = amountOfTilesY * Game1.tileSize;

        }

        private void setCropSize( int x, int y ) {

            if( x < positionToCropX || y < positionToCropY ) {
                return;
            }

            // Snap to tiles
            if( isSnappingToTileSize ) {
                snapSelectionToTiles( ref x, ref y );
            }

            widthOfCrop = x - positionOfTextureX - positionToCropX;
            heightOfCrop = y - positionOfTextureY - positionToCropY;

        }

        public override void releaseLeftClick( int x, int y ) {
            base.releaseLeftClick( x, y );
            setCropSize( x, y );
        }

        public override void receiveKeyPress( Keys key ) {
            base.receiveKeyPress( key );

            if( key == Keys.D1 ) {
                scale = 1; 
            }

            if( key == Keys.D2 ) {
                scale = 2;
            }

            if( key == Keys.D3 ) {
                scale = 3;
            }
            
            if( key == Keys.D4 ) {
                scale = 4;
            }

            if( key == Keys.LeftShift || key == Keys.RightShift ) {
                isShifting = !isShifting;
            }

            if( key == Keys.Down && isShifting ) {
                positionToCropY++;
            }

            if( key == Keys.Up && isShifting ) {
                positionToCropY--;
            }

            if( key == Keys.Left && isShifting ) {
                positionToCropX--;
            }

            if( key == Keys.Right && isShifting) {
                positionToCropX++;
            }

            if( key == Keys.Down ) {
                heightOfCrop++;
            }

            if( key == Keys.Up ) {
                heightOfCrop--;
            }

            if( key == Keys.Left ) {
                widthOfCrop--;
            }
            
            if( key == Keys.Right ) {
                widthOfCrop++;
            }

            if ( key == Keys.Space ) {
                isSnappingToTileSize = !isSnappingToTileSize;
            }

            // Copy to clipboard and show feedback
            if( key == Keys.Enter ) {

                // use predefined load for textures that are already in game from loadContent() in game1
                string copiedToClipboard = "Copied to clipboard";
                clipboardFeedback.Width = (int) Game1.smallFont.MeasureString( copiedToClipboard ).X + 30;
                clipboardFeedback.Height = (int) Game1.smallFont.MeasureString( copiedToClipboard ).Y + 10;
                clipboardFeedback.X = Game1.viewport.Width / 2 - clipboardFeedback.Width / 2;
                clipboardFeedback.Y = Game1.viewport.Height / 2 - clipboardFeedback.Height / 2;
                clipboardFeedback.Text = copiedToClipboard;

                clipboardFadeTimer.Interval = 2000;
                clipboardFadeTimer.Start();

                string stringToClipboard = $"ClickableTextureComponent clickableTextureComponent = new ClickableTextureComponent( new Rectangle( 0, 0, {widthOfCrop}, {heightOfCrop}), Game1.content.Load<Texture2D>( \"{contentLocation[ currentTextureIndex ].Replace( @"\", @"\\" )}\" ), new Rectangle( {positionToCropX}, {positionToCropY}, {widthOfCrop}, {heightOfCrop} ), {scale} );";
                // Windows only
                var thread = new Thread( () => System.Windows.Forms.Clipboard.SetText( stringToClipboard ) );
                thread.SetApartmentState( ApartmentState.STA ); //Set the thread to STA
                thread.Start();
                thread.Join(); //Wait for the thread to end


            }

        }


    }
}