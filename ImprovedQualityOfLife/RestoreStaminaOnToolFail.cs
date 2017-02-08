using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using Microsoft.Xna.Framework;

namespace Demiacle.ImprovedQualityOfLife {
    internal class RestoreStaminaOnToolFail {

        private bool foundToolEvent = false;
        private float restoreAmount = 0;

        private uint dirtHoed;
        private int toolPower;

        public RestoreStaminaOnToolFail() {
            GameEvents.UpdateTick += checkForToolAction;
        }

        private void checkForToolAction( object sender, EventArgs e ) {
            Farmer player = Game1.player;

            if( player.usingTool ) {

                Tool index = player.CurrentTool;
                foundToolEvent = true;
                toolPower = Math.Max ( player.toolPower, 1);

                if( index is Hoe ) {
                }

                if( index is Axe ) {

                    int i = 0;
                }

                if( index is Pickaxe ) {
                    int i = 0;

                }

                if( index is WateringCan ) {
                    int i = 0;

                }

                if( index is FishingRod ) {
                    int i = 0;

                }
                

            } else if( foundToolEvent ) {

                // Restore if dirt was never hoed
                // Fires if hoe is used on any tile with diggable property
                if( dirtHoed != Game1.stats.dirtHoed ) {
                    dirtHoed = Game1.stats.dirtHoed;
                } else {
                    restoreAmount = ( ( float ) ( 2 * toolPower ) - ( float ) player.FarmingLevel * 0.1f );
                }

                // Restore if pickaxe doesn't hit a rock
                // Fires if the object name is not Stone or does not have the word Boulder in it
                if( player.CurrentTool is Pickaxe ) {

                    var x = ( int ) player.GetToolLocation( false ).X;
                    var y = ( int ) player.GetToolLocation( false ).Y;
                    StardewValley.SerializableDictionary<Vector2, StardewValley.Object> gameObjects = Game1.currentLocation.objects;

                    if( gameObjects.ContainsKey( new Vector2( x / Game1.tileSize, y / Game1.tileSize ) ) ) {

                        StardewValley.Object objectThatJustGotHit = gameObjects[ new Vector2( x, y ) ];

                        if( objectThatJustGotHit.name.Equals( "Stone" ) || objectThatJustGotHit.name.Contains( "Boulder" ) ) {
                            // Do nothing
                        } else {
                            restoreAmount = ( ( float ) ( 2 * toolPower ) - ( float ) player.FarmingLevel * 0.1f );
                        }

                    }

                }

                // Restore if axe is not used on tree
                if( player.CurrentTool is Axe ) {

                    var x = ( int ) player.GetToolLocation( false ).X;
                    var y = ( int ) player.GetToolLocation( false ).Y;
                    StardewValley.SerializableDictionary<Vector2, StardewValley.Object> gameObjects = Game1.currentLocation.objects;

                    if( gameObjects.ContainsKey( new Vector2( x / Game1.tileSize, y / Game1.tileSize ) ) ) {

                        StardewValley.Object objectThatJustGotHit = gameObjects[ new Vector2( x, y ) ];

                        if( objectThatJustGotHit.name.Equals( "Stone" ) || objectThatJustGotHit.name.Contains( "Boulder" ) ) {
                            // Do nothing
                        } else {
                            restoreAmount = ( ( float ) ( 2 * toolPower ) - ( float ) player.FarmingLevel * 0.1f );
                        }

                    }

                }

                Game1.player.stamina += restoreAmount;
                foundToolEvent = false;
            }



        }
    }
}