using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demiacle_SVM {
    public class ElevatorMenuMod : IClickableMenu {

        public List<ClickableComponent> elevators = new List<ClickableComponent>();

        public ElevatorMenuMod() 
          : base( 0, 0, 0, 0, true ) {
            int num = Math.Min( Game1.mine.lowestLevelReached, 120 );
            this.width = num > 50 ? ( Game1.tileSize * 3 / 4 - 4 ) * 11 + IClickableMenu.borderWidth * 2 : Math.Min( ( Game1.tileSize * 3 / 4 - 4 ) * 5 + IClickableMenu.borderWidth * 2, num * ( Game1.tileSize * 3 / 4 - 4 ) + IClickableMenu.borderWidth * 2 );
            this.height = Math.Max( Game1.tileSize + IClickableMenu.borderWidth * 3, num * ( Game1.tileSize * 3 / 4 - 4 ) / ( this.width - IClickableMenu.borderWidth ) * ( Game1.tileSize * 3 / 4 - 4 ) + Game1.tileSize + IClickableMenu.borderWidth * 3 );
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;
            Game1.playSound( "crystal" );
            int x1 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
            int y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.borderWidth / 3;
            this.elevators.Add( new ClickableComponent( new Rectangle( x1, y, Game1.tileSize * 3 / 4 - 4, Game1.tileSize * 3 / 4 - 4 ), string.Concat( ( object ) 0 ) ) );
            int x2 = x1 + Game1.tileSize - 20;
            if( x2 > this.xPositionOnScreen + this.width - IClickableMenu.borderWidth ) {
                x2 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
                y += Game1.tileSize - 20;
            }
            for( int index = 1; index <= num; ++index ) {
                this.elevators.Add( new ClickableComponent( new Rectangle( x2, y, Game1.tileSize * 3 / 4 - 4, Game1.tileSize * 3 / 4 - 4 ), string.Concat( ( object ) ( index  ) ) ) );
                x2 = x2 + Game1.tileSize - 20;
                if( x2 > this.xPositionOnScreen + this.width - IClickableMenu.borderWidth ) {
                    x2 = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder * 3 / 4;
                    y += Game1.tileSize - 20;
                }
            }
            this.initializeUpperRightCloseButton();
        }

        public override void receiveLeftClick( int x, int y, bool playSound = true ) {
            if( this.isWithinBounds( x, y ) ) {
                foreach( ClickableComponent elevator in this.elevators ) {
                    if( elevator.containsPoint( x, y ) ) {
                        Game1.playSound( "smallSelect" );
                        if( Convert.ToInt32( elevator.name ) == 0 ) {
                            if( !Game1.currentLocation.Equals( ( object ) Game1.mine ) )
                                return;
                            Game1.warpFarmer( "Mine", 17, 4, true );
                            Game1.exitActiveMenu();
                            Game1.changeMusicTrack( "none" );
                        } else {
                            if( Game1.currentLocation.Equals( ( object ) Game1.mine ) && Convert.ToInt32( elevator.name ) == Game1.mine.mineLevel )
                                return;
                            Game1.player.ridingMineElevator = true;
                            enterMine( Convert.ToInt32( elevator.name ) );
                            Game1.exitActiveMenu();
                        }
                    }
                }
                base.receiveLeftClick( x, y, true );
            } else
                Game1.exitActiveMenu();
        }

        private void enterMine( int level ) {
            Game1.inMine = true;
            ModEntry.Log( "before hijack" );
            if( true ) {
                ModEntry.Log( "Creating hijack" );
                Game1.mine = new MineShaftExtension();
            }

            Game1.mine.setNextLevel( level );
            Game1.warpFarmer( "UndergroundMine", 6, 6, 2 );
        }

        public override void receiveRightClick( int x, int y, bool playSound = true ) {
        }

        public override void performHoverAction( int x, int y ) {
            base.performHoverAction( x, y );
            foreach( ClickableComponent elevator in this.elevators )
                elevator.scale = !elevator.containsPoint( x, y ) ? 1f : 2f;
        }

        public override void draw( SpriteBatch b ) {
            b.Draw( Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f );
            Game1.drawDialogueBox( this.xPositionOnScreen, this.yPositionOnScreen - Game1.tileSize + Game1.tileSize / 8, this.width + Game1.tileSize / 3, this.height + Game1.tileSize, false, true, ( string ) null, false );
            foreach( ClickableComponent elevator in this.elevators ) {
                b.Draw( Game1.mouseCursors, new Vector2( ( float ) ( elevator.bounds.X - Game1.pixelZoom ), ( float ) ( elevator.bounds.Y + Game1.pixelZoom ) ), new Rectangle?( new Rectangle( ( double ) elevator.scale > 1.0 ? 267 : 256, 256, 10, 10 ) ), Color.Black * 0.5f, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.865f );
                b.Draw( Game1.mouseCursors, new Vector2( ( float ) elevator.bounds.X, ( float ) elevator.bounds.Y ), new Rectangle?( new Rectangle( ( double ) elevator.scale > 1.0 ? 267 : 256, 256, 10, 10 ) ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.868f );
                Vector2 position = new Vector2( ( float ) ( elevator.bounds.X + 16 + NumberSprite.numberOfDigits( Convert.ToInt32( elevator.name ) ) * 6 ), ( float ) ( elevator.bounds.Y + Game1.pixelZoom * 6 - NumberSprite.getHeight() / 4 ) );
                NumberSprite.draw( Convert.ToInt32( elevator.name ), b, position, Game1.mine.mineLevel == Convert.ToInt32( elevator.name ) && Game1.currentLocation.Equals( ( object ) Game1.mine ) || Convert.ToInt32( elevator.name ) == 0 && !Game1.currentLocation.Equals( ( object ) Game1.mine ) ? Color.Gray * 0.75f : Color.Gold, 0.5f, 0.86f, 1f, 0, 0 );
            }
            this.drawMouse( b );
            base.draw( b );
        }
    }
}

