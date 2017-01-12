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
using DemiacleSvm.UiMods;

namespace DemiacleSvm {
    class OptionsPage {

        private ModData modData;

        private static readonly int WIDTH = 800;
        private static readonly int HEIGHT = 800;

        private ModOptionMenu optionMenu;
        private readonly List<UiModWithOptions> optionMods = new List<UiModWithOptions>();

        public OptionsPage( ModData modData ) {
            this.modData = modData;
            PlayerEvents.LoadedGame += initialize;
        }

        public OptionsPage( ModData modData, UiModAccurateHearts uiModAccurateHearts, UiModLocationOfTownsfolk uiModLocationOfTownsfolk, UiModItemRolloverInformation uiModItemrolloverInformation, UiModExperience uiModExperience, UiModLuckOfDay uiModluckOfDay ) : this( modData ) {
            optionMods.Add( uiModAccurateHearts );
            optionMods.Add( uiModLocationOfTownsfolk );
            optionMods.Add( uiModItemrolloverInformation);
            optionMods.Add( uiModExperience );
            optionMods.Add( uiModluckOfDay );

            // Set each option to its value
            foreach( UiModWithOptions mod in optionMods ) {
                foreach( var option in mod.options ) {
                    Action<bool> method = option.Value;
                    SerializableDictionary<string, bool> uiOptions = ModEntry.modData.uiOptions;

                    method.Invoke( uiOptions[ option.Key ] );
                }
            }

        }

        private void initialize( object sender, EventArgsLoadedGameChanged e ) {
            optionMenu = new ModOptionMenu( Game1.viewport.Width / 2 - WIDTH / 2, Game1.viewport.Height / 2 - HEIGHT / 2, WIDTH, HEIGHT, optionMods );
            ControlEvents.KeyPressed += onKeyPress;
        }


        private void onKeyPress( object sender, EventArgsKeyPressed e ) {

            if( !Game1.player.canMove ) {
                return;
            }

            if( $"{e.KeyPressed}" == "N" ) {
                if( Game1.activeClickableMenu != null && Game1.activeClickableMenu == optionMenu ) {
                    Game1.activeClickableMenu = null;
                    ModEntry.updateModData();
                } else {
                    Game1.activeClickableMenu = optionMenu;
                }
            }

        }

        internal class ModOptionMenu : IClickableMenu {

            private List<OptionsElement> options = new List<OptionsElement>();
            private readonly int OPTION_HEIGHT = ( int ) Game1.dialogueFont.MeasureString( "t" ).Y + 40;
            private List<UiModWithOptions> optionMods = new List<UiModWithOptions>();


            internal ModOptionMenu( int x, int y, int width, int height, List<UiModWithOptions> optionMods ) : base( x, y, width, height ) {

                int count = 0;
                int positionOfCheckboxesX = this.xPositionOnScreen + 40;

                foreach( KeyValuePair<string,bool> uiOptionData in ModEntry.modData.uiOptions ) {
                    OptionsCheckbox box = new OptionsCheckbox( uiOptionData.Key, uiOptionData.Key.GetHashCode(), positionOfCheckboxesX, this.yPositionOnScreen + count++ * OPTION_HEIGHT );
                    options.Add( box );
                    box.isChecked = uiOptionData.Value;
                }

            }

            public override void draw( SpriteBatch b ) {

                for( int i = 0; i < options.Count; i ++ ) {
                    IClickableMenu.drawTextureBox( b, this.xPositionOnScreen, options[i].bounds.Y - 28, this.width, OPTION_HEIGHT, Color.White );
                    options[ i ].draw( b, 0, 0);
                }
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );

            }

            public override void receiveLeftClick( int x, int y, bool playSound = true ) {
                base.receiveLeftClick( x, y, playSound );

                foreach( OptionsCheckbox checkBox in options ) {
                    if( checkBox.bounds.Contains( x, y ) ) {
                        checkBox.receiveLeftClick( x, y );
                        ModEntry.modData.uiOptions[ checkBox.label ] = checkBox.isChecked;
                        foreach( UiModWithOptions mod in optionMods ) {
                            mod.ToggleOption( checkBox.label, checkBox.isChecked );
                        }
                    }
                }

            }

            public override void receiveRightClick( int x, int y, bool playSound = true ) {
            }
        }
    }
}
