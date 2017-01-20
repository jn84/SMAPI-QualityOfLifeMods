using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace DemiacleSvm.UiMods {
    class OptionsPage {

        private static readonly int WIDTH = 800;
        private static readonly int HEIGHT = 800;

        private ModOptionMenu optionMenu;
        private readonly List<UiModWithOptions> optionMods = new List<UiModWithOptions>();

        public OptionsPage( List<UiModWithOptions> optionMods ) {
            this.optionMods = optionMods;

            PlayerEvents.LoadedGame += initialize;
        }

        private void initialize( object sender, EventArgsLoadedGameChanged e ) {
            optionMenu = new ModOptionMenu( Game1.viewport.Width / 2 - WIDTH / 2, Game1.viewport.Height / 2 - HEIGHT / 2, WIDTH, HEIGHT, optionMods );
            ControlEvents.KeyPressed += onKeyPress;
            TimeEvents.OnNewDay += saveModData;

            // Set each option to its value
            foreach( UiModWithOptions mod in optionMods ) {
                foreach( var option in mod.options ) {
                    Action<bool> method = option.Value;
                    SerializableDictionary<string, bool> uiOptions = ModEntry.modData.uiOptions;

                    // if options are set, initialize ToggleOption delegates
                    method.Invoke( uiOptions[ option.Key ] );
                }
            }
        }

        private void saveModData( object sender, EventArgsNewDay e ) {
            ModEntry.updateModData();
        }

        private void onKeyPress( object sender, EventArgsKeyPressed e ) {
            if( !Game1.player.canMove ) {
                return;
            }

            if( $"{e.KeyPressed}" == "N" ) {
                if( Game1.activeClickableMenu != null && Game1.activeClickableMenu == optionMenu ) {
                    Game1.activeClickableMenu = null;
                } else {
                    Game1.activeClickableMenu = optionMenu;
                }
            }

        }

        internal class ModOptionMenu : IClickableMenu {

            private List<OptionsElement> optionDisplay = new List<OptionsElement>();
            private readonly int OPTION_HEIGHT = ( int ) Game1.dialogueFont.MeasureString( "t" ).Y + 40;
            private List<UiModWithOptions> optionMods;

            internal ModOptionMenu( int x, int y, int width, int height, List<UiModWithOptions> optionMods ) : base( x, y, width, height ) {

                int count = 0;
                int positionOfCheckboxesX = this.xPositionOnScreen + 40;

                this.optionMods = optionMods;

                // Creates a checkbox for each mods option
                foreach( UiModWithOptions mod in optionMods ) {
                    foreach( var option in mod.options ) {
                        var box = new OptionsCheckbox( option.Key, option.Key.GetHashCode(), positionOfCheckboxesX, this.yPositionOnScreen + count++ * OPTION_HEIGHT );
                        optionDisplay.Add( box );
                        box.isChecked = ModEntry.modData.uiOptions[ option.Key ];
                    }
                }

            }

            public override void draw( SpriteBatch b ) {

                for( int i = 0; i < optionDisplay.Count; i ++ ) {
                    IClickableMenu.drawTextureBox( b, this.xPositionOnScreen, optionDisplay[i].bounds.Y - 28, this.width, OPTION_HEIGHT, Color.White );
                    optionDisplay[ i ].draw( b, 0, 0);
                }

                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );

            }

            public override void receiveLeftClick( int x, int y, bool playSound = true ) {
                base.receiveLeftClick( x, y, playSound );

                foreach( OptionsCheckbox checkBox in optionDisplay ) {
                    if( checkBox.bounds.Contains( x, y ) ) {
                        checkBox.receiveLeftClick( x, y );
                        ModEntry.modData.uiOptions[ checkBox.label ] = checkBox.isChecked;
                        
                        // Good enough
                        foreach( UiModWithOptions mod in optionMods ) {
                            if( mod.options.ContainsKey( checkBox.label ) ) {
                                mod.options[ checkBox.label ].Invoke( checkBox.isChecked );
                            }
                        }
                    }
                }

            }

            public override void receiveRightClick( int x, int y, bool playSound = true ) {
            }
        }
    }
}
