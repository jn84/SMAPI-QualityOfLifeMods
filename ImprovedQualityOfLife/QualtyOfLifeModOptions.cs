using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Demiacle.ImprovedQualityOfLife {
    class QualtyOfLifeModOptions : IClickableMenu {
        private string descriptionText = "";
        private string hoverText = "";
        private List<ClickableComponent> optionSlots = new List<ClickableComponent>();
        private List<OptionsElement> options = new List<OptionsElement>();
        private int optionsSlotHeld = -1;
        public const int itemsPerPage = 7;
        public const int indexOfGraphicsPage = 6;
        public int currentItemIndex;
        private ClickableTextureComponent upArrow;
        private ClickableTextureComponent downArrow;
        private ClickableTextureComponent scrollBar;
        private bool scrolling;
        private Rectangle scrollBarRunner;

        public QualtyOfLifeModOptions( )
      : base( 0, 0, 800, 800, false)
    {
            this.upArrow = new ClickableTextureComponent( new Rectangle( this.xPositionOnScreen + width + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom ), Game1.mouseCursors, new Rectangle( 421, 459, 11, 12 ), ( float ) Game1.pixelZoom, false );
            this.downArrow = new ClickableTextureComponent( new Rectangle( this.xPositionOnScreen + width + Game1.tileSize / 4, this.yPositionOnScreen + height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom ), Game1.mouseCursors, new Rectangle( 421, 472, 11, 12 ), ( float ) Game1.pixelZoom, false );
            this.scrollBar = new ClickableTextureComponent( new Rectangle( this.upArrow.bounds.X + Game1.pixelZoom * 3, this.upArrow.bounds.Y + this.upArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom ), Game1.mouseCursors, new Rectangle( 435, 463, 6, 10 ), ( float ) Game1.pixelZoom, false );
            this.scrollBarRunner = new Rectangle( this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + Game1.pixelZoom, this.scrollBar.bounds.Width, height - Game1.tileSize * 2 - this.upArrow.bounds.Height - Game1.pixelZoom * 2 );
            for( int index = 0; index < 7; ++index )
                this.optionSlots.Add( new ClickableComponent( new Rectangle( this.xPositionOnScreen + Game1.tileSize / 4, this.yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + index * ( ( height - Game1.tileSize * 2 ) / 7 ), width - Game1.tileSize / 2, ( height - Game1.tileSize * 2 ) / 7 + Game1.pixelZoom ), string.Concat( ( object ) index ) ) );
            this.options.Add( new OptionsElement( "Quality of Life Mod v0.1" ) );

            // Time per 10 minute option
            this.options.Add( new OptionsElement( "Additional seconds per 10 minute tick" ) );
            var optionValue = new List<string>();
            optionValue.Add( "-6" );
            optionValue.Add( "-5" );
            optionValue.Add( "-4" );
            optionValue.Add( "-3" );
            optionValue.Add( "-2" );
            optionValue.Add( "-1" );
            optionValue.Add( "0" );
            optionValue.Add( "1" );
            optionValue.Add( "2" );
            optionValue.Add( "3" );
            optionValue.Add( "4" );
            optionValue.Add( "5" );
            optionValue.Add( "6" );
            optionValue.Add( "7" );
            optionValue.Add( "8" );
            optionValue.Add( "9" );
            optionValue.Add( "10" );
            optionValue.Add( "11" );
            optionValue.Add( "12" );
            optionValue.Add( "13" );
            optionValue.Add( "14" );
            optionValue.Add( "15" );
            optionValue.Add( "16" );
            optionValue.Add( "17" );
            optionValue.Add( "18" );
            optionValue.Add( "19" );
            optionValue.Add( "20" );
            optionValue.Add( "30" );
            optionValue.Add( "40" );
            optionValue.Add( "50" );
            optionValue.Add( "60" );
            optionValue.Add( "120" );
            optionValue.Add( "180" );
            optionValue.Add( "300" );

            var optionsPlusMinus = new OptionsPlusMinus( "", 2, optionValue, -1, -1 );
            options.Add( ( OptionsElement ) optionsPlusMinus );

        }

        private void setScrollBarToCurrentIndex() {
            if( this.options.Count <= 0 )
                return;
            this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max( 1, this.options.Count - 7 + 1 ) * this.currentItemIndex + this.upArrow.bounds.Bottom + Game1.pixelZoom;
            if( this.currentItemIndex != this.options.Count - 7 )
                return;
            this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - Game1.pixelZoom;
        }

        public override void leftClickHeld( int x, int y ) {
            if( GameMenu.forcePreventClose )
                return;
            base.leftClickHeld( x, y );
            if( this.scrolling ) {
                int y1 = this.scrollBar.bounds.Y;
                this.scrollBar.bounds.Y = Math.Min( this.yPositionOnScreen + this.height - Game1.tileSize - Game1.pixelZoom * 3 - this.scrollBar.bounds.Height, Math.Max( y, this.yPositionOnScreen + this.upArrow.bounds.Height + Game1.pixelZoom * 5 ) );
                this.currentItemIndex = Math.Min( this.options.Count - 7, Math.Max( 0, ( int ) ( ( double ) this.options.Count * ( double ) ( ( float ) ( y - this.scrollBarRunner.Y ) / ( float ) this.scrollBarRunner.Height ) ) ) );
                this.setScrollBarToCurrentIndex();
                int y2 = this.scrollBar.bounds.Y;
                if( y1 == y2 )
                    return;
                Game1.playSound( "shiny4" );
            } else {
                if( this.optionsSlotHeld == -1 || this.optionsSlotHeld + this.currentItemIndex >= this.options.Count )
                    return;
                this.options[ this.currentItemIndex + this.optionsSlotHeld ].leftClickHeld( x - this.optionSlots[ this.optionsSlotHeld ].bounds.X, y - this.optionSlots[ this.optionsSlotHeld ].bounds.Y );
            }
        }

        public override void receiveKeyPress( Keys key ) {
            if( this.optionsSlotHeld == -1 || this.optionsSlotHeld + this.currentItemIndex >= this.options.Count )
                return;
            this.options[ this.currentItemIndex + this.optionsSlotHeld ].receiveKeyPress( key );
        }

        public override void receiveScrollWheelAction( int direction ) {
            if( GameMenu.forcePreventClose )
                return;
            base.receiveScrollWheelAction( direction );
            if( direction > 0 && this.currentItemIndex > 0 ) {
                this.upArrowPressed();
                Game1.playSound( "shiny4" );
            } else {
                if( direction >= 0 || this.currentItemIndex >= Math.Max( 0, this.options.Count - 7 ) )
                    return;
                this.downArrowPressed();
                Game1.playSound( "shiny4" );
            }
        }

        public override void releaseLeftClick( int x, int y ) {
            if( GameMenu.forcePreventClose )
                return;
            base.releaseLeftClick( x, y );
            if( this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count )
                this.options[ this.currentItemIndex + this.optionsSlotHeld ].leftClickReleased( x - this.optionSlots[ this.optionsSlotHeld ].bounds.X, y - this.optionSlots[ this.optionsSlotHeld ].bounds.Y );
            this.optionsSlotHeld = -1;
            this.scrolling = false;
        }

        private void downArrowPressed() {
            this.downArrow.scale = this.downArrow.baseScale;
            this.currentItemIndex = this.currentItemIndex + 1;
            this.setScrollBarToCurrentIndex();
        }

        private void upArrowPressed() {
            this.upArrow.scale = this.upArrow.baseScale;
            this.currentItemIndex = this.currentItemIndex - 1;
            this.setScrollBarToCurrentIndex();
        }

        public override void receiveLeftClick( int x, int y, bool playSound = true ) {
            if( GameMenu.forcePreventClose )
                return;

            if( this.downArrow.containsPoint( x, y ) && this.currentItemIndex < Math.Max( 0, this.options.Count - 7 ) ) {
                this.downArrowPressed();
                Game1.playSound( "shwip" );
            } else if( this.upArrow.containsPoint( x, y ) && this.currentItemIndex > 0 ) {
                this.upArrowPressed();
                Game1.playSound( "shwip" );
            } else if( this.scrollBar.containsPoint( x, y ) )
                this.scrolling = true;
            else if( !this.downArrow.containsPoint( x, y ) && x > this.xPositionOnScreen + this.width && ( x < this.xPositionOnScreen + this.width + Game1.tileSize * 2 && y > this.yPositionOnScreen ) && y < this.yPositionOnScreen + this.height ) {
                this.scrolling = true;
                this.leftClickHeld( x, y );
                this.releaseLeftClick( x, y );
            }
            this.currentItemIndex = Math.Max( 0, Math.Min( this.options.Count - 7, this.currentItemIndex ) );
            for( int index = 0; index < this.optionSlots.Count; ++index ) {
                if( this.optionSlots[ index ].bounds.Contains( x, y ) && this.currentItemIndex + index < this.options.Count && this.options[ this.currentItemIndex + index ].bounds.Contains( x - this.optionSlots[ index ].bounds.X, y - this.optionSlots[ index ].bounds.Y ) ) {
                    this.options[ this.currentItemIndex + index ].receiveLeftClick( x - this.optionSlots[ index ].bounds.X, y - this.optionSlots[ index ].bounds.Y );
                    this.optionsSlotHeld = index;
                    break;
                }
            }
        }

        public override void receiveRightClick( int x, int y, bool playSound = true ) {
        }

        public override void performHoverAction( int x, int y ) {
            if( GameMenu.forcePreventClose )
                return;
            this.descriptionText = "";
            this.hoverText = "";
            this.upArrow.tryHover( x, y, 0.1f );
            this.downArrow.tryHover( x, y, 0.1f );
            this.scrollBar.tryHover( x, y, 0.1f );
            int num = this.scrolling ? 1 : 0;
        }

        public override void draw( SpriteBatch b ) {
            b.End();
            b.Begin( SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, ( DepthStencilState ) null, ( RasterizerState ) null );
            Game1.drawDialogueBox(0,0, width, height, false, true);
            for( int index = 0; index < this.optionSlots.Count; ++index ) {
                if( this.currentItemIndex >= 0 && this.currentItemIndex + index < this.options.Count )
                    this.options[ this.currentItemIndex + index ].draw( b, this.optionSlots[ index ].bounds.X, this.optionSlots[ index ].bounds.Y );
            }
            b.End();
            b.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, ( DepthStencilState ) null, ( RasterizerState ) null );
            if( !GameMenu.forcePreventClose ) {
                this.upArrow.draw( b );
                this.downArrow.draw( b );
                if( this.options.Count > 7 ) {
                    IClickableMenu.drawTextureBox( b, Game1.mouseCursors, new Rectangle( 403, 383, 6, 6 ), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, ( float ) Game1.pixelZoom, false );
                    this.scrollBar.draw( b );
                }
            }

            if( this.hoverText.Equals( "" ) ) {
                // do nothing
            } else {
                IClickableMenu.drawHoverText( b, this.hoverText, Game1.smallFont, 0, 0, -1, ( string ) null, -1, ( string[] ) null, ( Item ) null, 0, -1, -1, -1, -1, 1f, ( CraftingRecipe ) null );
            }

            if( Game1.options.hardwareCursor == false) {
                b.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getOldMouseX(), ( float ) Game1.getOldMouseY() ), new Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16 ) ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );
            }

        }
    }
}
