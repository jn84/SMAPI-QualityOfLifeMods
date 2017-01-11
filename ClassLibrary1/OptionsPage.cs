using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Demiacle_SVM {
    class OptionsPage {

        private ModData modData;
        private static readonly int WIDTH = 800;
        private static readonly int HEIGHT = 800;

        private ModOptionMenu optionMenu;
        private bool shouldDraw = false;

        public OptionsPage( ModData modData ) {
            ControlEvents.KeyPressed += onKeyPress;
            this.modData = modData;
            GraphicsEvents.OnPreRenderGuiEvent += onPreRenderEvent;
            PlayerEvents.LoadedGame += onLoadedGame;
        }

        private void onLoadedGame( object sender, EventArgsLoadedGameChanged e ) {
            optionMenu = new ModOptionMenu( Game1.viewport.Width / 2 - WIDTH / 2, Game1.viewport.Height / 2 - HEIGHT / 2, WIDTH, HEIGHT );
        }

        private void onPreRenderEvent( object sender, EventArgs e ) {
           // if( shouldDraw ) {
             //   optionMenu.draw( Game1.spriteBatch );
            //}
        }

        private void onKeyPress( object sender, EventArgsKeyPressed e ) {


            if( !Game1.player.canMove && Game1.activeClickableMenu == null ) {
                return;
            }

            if( $"{e.KeyPressed}" == "N" ) {
                if( Game1.activeClickableMenu != null && Game1.activeClickableMenu == optionMenu ) {
                    Game1.activeClickableMenu = null;
                } else {
                    //Game1.paused = !Game1.paused;
                    //shouldDraw = !shouldDraw;
                    Game1.activeClickableMenu = optionMenu;
                }
               // GraphicsEvents.OnPreRenderGuiEvent += onPreRenderEvent;

            }
        }

        internal class ModOptionMenu : IClickableMenu {

            private List<OptionsElement> options = new List<OptionsElement>();
            private readonly int OPTION_HEIGHT = ( int ) Game1.dialogueFont.MeasureString( "t" ).Y + 40;

            internal ModOptionMenu( int x, int y, int width, int height ) : base( x, y, width, height ) {
                int count = 0;
                int positionOfCheckboxesX = this.xPositionOnScreen + 40;
                options.Add( new OptionsCheckbox( "Show luck icon", 9990, positionOfCheckboxesX, this.yPositionOnScreen + count++ * OPTION_HEIGHT ) );
                options.Add( new OptionsCheckbox( "Show experience bar", 9990, positionOfCheckboxesX, this.yPositionOnScreen + count++ * OPTION_HEIGHT  ) );
                options.Add( new OptionsCheckbox( "Experience bar always invisible", 9990, positionOfCheckboxesX, this.yPositionOnScreen + count++ * OPTION_HEIGHT  ) );
                options.Add( new OptionsCheckbox( "Show heart fills", 9990, positionOfCheckboxesX, this.yPositionOnScreen + count++ * OPTION_HEIGHT  ) );
                options.Add( new OptionsCheckbox( "Show npcs on map", 9990, positionOfCheckboxesX, this.yPositionOnScreen + count++ * OPTION_HEIGHT  ) );
                options.Add( new OptionsCheckbox( "Show extra item information", 9990, positionOfCheckboxesX, this.yPositionOnScreen + count++ * OPTION_HEIGHT  ) );
            }

            public override void draw( SpriteBatch b ) {

                for( int i = 0; i < options.Count; i ++ ) {
                    IClickableMenu.drawTextureBox( b, this.xPositionOnScreen, options[i].bounds.Y - 28, this.width, OPTION_HEIGHT, Color.White );
                    options[ i ].draw( b, 0, 0);
                }
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );

                //base.draw( b );
            }

            public override void receiveLeftClick( int x, int y, bool playSound = true ) {
                base.receiveLeftClick( x, y, playSound );

                foreach( OptionsCheckbox checkBox in options ) {
                    if( checkBox.bounds.Contains( x, y ) ) {

                        checkBox.receiveLeftClick( x, y );
                        //mod
                    }
                }
            }

            public override void receiveRightClick( int x, int y, bool playSound = true ) {
            }
        }
    }
}
