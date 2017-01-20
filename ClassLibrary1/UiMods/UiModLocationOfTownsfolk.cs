using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DemiacleSvm.UiMods {

    /// <summary>
    /// Displays mugshots of townsfolk on the map.
    /// </summary>
    class UiModLocationOfTownsfolk : UiModWithOptions {

        private SocialPage socialPage;

        private List<ClickableTextureComponent> friendNames;
        private List<NPC> townsfolk = new List<NPC>();
        private List<OptionsCheckbox> checkboxes = new List<OptionsCheckbox>();

        public const string SHOW_NPCS_ON_MAP = "Show npcs on map";

        private int socialPanelWidth = 190;
        private int socialPanelOffsetX = 160;


        public UiModLocationOfTownsfolk() {
            addOption( SHOW_NPCS_ON_MAP, toggleShowNPCLocationOnMap );
        }

        internal void drawNPCLocationsOnMap( object sender, EventArgs e ) {
            
            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            var currentMenu = (GameMenu) Game1.activeClickableMenu;

            if(  currentMenu.currentTab != GameMenu.mapTab ) {
                return;
            }
            
            foreach( NPC npc in townsfolk ) {

                int key = npc.name.GetHashCode();

                // If key for some reason doesn't exist
                if( !( ModEntry.modData.locationOfTownsfolkOptions.ContainsKey( key ) ) ) {
                    continue;
                }

                // If key exists and value is false dont display anything
                if ( ModEntry.modData.locationOfTownsfolkOptions[ key ] == false ) {
                    continue;
                }

                Rectangle rect = npc.getMugShotSourceRect();
                rect.Height = rect.Height - 8;

                int offsetX = 0;
                int offsetY = 0;
                
                // Set the correct position for character head mugshots
                switch( npc.currentLocation.name ) {

                    case "Town":
                        offsetX = 680;
                        offsetY = 360;
                        break;

                    case "HaleyHouse":
                        offsetX = 652;
                        offsetY = 408;
                        break;

                    case "SamHouse":
                        offsetX = 612;
                        offsetY = 396;
                        break;

                    case "Blacksmith":
                        offsetX = 852;
                        offsetY = 388;
                        break;

                    case "ManorHouse":
                        offsetX = 768;
                        offsetY = 388;
                        break;

                    case "SeedShop":
                        offsetX = 696;
                        offsetY = 296;
                        break;

                    case "Saloon":
                        offsetX = 716;
                        offsetY = 352;
                        break;

                    case "Trailer":
                        offsetX = 780;
                        offsetY = 360;
                        break;

                    case "Hospital":
                        offsetX = 680;
                        offsetY = 304;
                        break;
                                        
                    case "Beach":
                        offsetX = 790;
                        offsetY = 550;
                        break;

                    case "ElliottHouse":
                        offsetX = 260;
                        offsetY = 572;
                        break;

                    case "ScienceHouse":
                        offsetX = 732;
                        offsetY = 148;
                        break;

                    case "Mountain":
                        offsetX = 762;
                        offsetY = 154;
                        break;

                    case "SebastianRoom":
                        break;

                    case "Tent":
                        offsetX = 784;
                        offsetY = 128;
                        break;

                    case "Forest":
                        offsetX = 80;
                        offsetY = 272;
                        break;

                    case "WizardHouseBasement":
                    case "WizardHouse":
                        offsetX = 196;
                        offsetY = 352;
                        break;

                    case "AnimalShop":
                        offsetX = 420;
                        offsetY = 392;
                        break;

                    case "LeahHouse":
                        offsetX = 452;
                        offsetY = 436;
                        break;

                    case "BusStop":
                        offsetX = 516;
                        offsetY = 224;
                        break;

                    case "Mine":
                        offsetX = 880;
                        offsetY = 100;
                        break;

                    case "Sewer":
                        offsetX = 380;
                        offsetY = 596;
                        break;
                    
                    case "Club":
                    case "Desert":
                        offsetX = 60;
                        offsetY = 92;
                        break;
                    
                    case "ArchaeologyHouse":
                        offsetX = 892;
                        offsetY = 416;
                        break;                    

                    case "Woods":
                        offsetX = 100;
                        offsetY = 272;
                        break;

                    case "Railroad":
                        offsetX = 644;
                        offsetY = 64;
                        break;
                    
                    case "FishShop":
                        offsetX = 844;
                        offsetY = 608;
                        break;

                    case "BathHouse_Entry":
                    case "BathHouse_MensLocker":
                    case "BathHouse_WomensLocker":
                    case "BathHouse_Pool":
                        offsetX = 576;
                        offsetY = 60;
                        break;

                    case "CommunityCenter":
                        offsetX = 692;
                        offsetY = 204;
                        break;

                    case "JojaMart":
                        offsetX = 872;
                        offsetY = 280;
                        break;
                    
                    case "Backwoods":
                        offsetX = 460;
                        offsetY = 156;
                        break;

                    case "BugLand":
                    case "SandyHouse":
                    case "Greenhouse":
                    case "SkullCave":
                    case "JoshHouse":
                    case "HarveyRoom":
                    case "Tunnel":
                    case "Cellar":
                    case "WitchSwamp":
                    case "WitchHut":
                    case "WitchWarpCave":
                    case "Summit":
                    case "AdentureGuild":
                    default:
                        ModEntry.Log( $"The location {npc.currentLocation.name} is not set for the uiMod location of townsfolk... please add and use reference from MapPages.cs" );
                        break;

                }

                int positionX = Game1.activeClickableMenu.xPositionOnScreen - 180;
                int positionY = Game1.activeClickableMenu.yPositionOnScreen - 40;

                var npcMugShot = new ClickableTextureComponent( npc.name, new Rectangle( positionX + offsetX, positionY + offsetY, Game1.activeClickableMenu.width, Game1.tileSize ), ( string ) null, npc.name, npc.sprite.Texture, rect, ( float ) Game1.pixelZoom, false );
                npcMugShot.scale = 3f;
                npcMugShot.draw( Game1.spriteBatch );
            }
        }

        public void drawSocialPageOptions( object sender, EventArgs e ) {

            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            var currentMenu = ( GameMenu ) Game1.activeClickableMenu;

            if( currentMenu.currentTab != GameMenu.socialTab ) {
                return;
            }

            // Draw Panel
            Game1.drawDialogueBox( Game1.activeClickableMenu.xPositionOnScreen - socialPanelOffsetX, Game1.activeClickableMenu.yPositionOnScreen, socialPanelWidth, Game1.activeClickableMenu.height, false, true );

            // Draw Content
            var slotPosition = ( int ) typeof( SocialPage ).GetField( "slotPosition", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( socialPage );
            int offsetY = 0;
            for( int i = slotPosition; i < slotPosition + 5; i++ ) {

                // Safety check... this exists inside the client so lets reproduce here just in case
                if( i > friendNames.Count ) {
                    return;
                }

                // Set Checkbox position - TODO this should be removed from the drawing method if possible
                checkboxes[ i ].bounds.X = Game1.activeClickableMenu.xPositionOnScreen - 60;
                checkboxes[ i ].bounds.Y = Game1.activeClickableMenu.yPositionOnScreen + 130 + offsetY;

                // Draw Checkbox
                checkboxes[ i ].draw( Game1.spriteBatch, 0, 0 );
                offsetY += 112;

                // Set color for magnifying glass
                Color magnifyingGlassColor = Color.Gray;
                if( checkboxes[ i ].isChecked ) {
                    magnifyingGlassColor = Color.White;
                }

                // Draw Magnifying glasses
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( checkboxes[ i ].bounds.X - 50, checkboxes[ i ].bounds.Y ), new Rectangle( 80, 0, 16, 16 ), magnifyingGlassColor, 0f, Vector2.Zero, 3, SpriteEffects.None, 1f );

                // Draw line below boxes omitting the last box... Hacky but W/E
                if( offsetY != 560 ) {
                    Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( checkboxes[ i ].bounds.X - 50, checkboxes[ i ].bounds.Y + 72, socialPanelWidth / 2 - 6, 4 ), Color.SaddleBrown );
                    Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( checkboxes[ i ].bounds.X - 50, checkboxes[ i ].bounds.Y + 76, socialPanelWidth / 2 - 6, 4 ), Color.BurlyWood );
                }

                // ReDraw the mouse
                Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );

                // Keep just in case
                // Draw Large Heart
                // Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( checkboxes[ i ].bounds.X + heartOffsetX, checkboxes[ i ].bounds.Y ), new Rectangle( 218, 428, 7, 6 ), Color.White, 0f, Vector2.Zero, 8, SpriteEffects.None, 0.88f );
            }
        }

        /// <summary>
        /// Resets and populates the list of townsfolk and checkboxes to display every time the game menu is called
        /// </summary>
        internal void onMenuChange( object sender, EventArgsClickableMenuChanged e ) {

            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            // Get pages from GameMenu            
            var pages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );

            // Save variables needed for mod 
            for( int k = 0; k < pages.Count; k++ ) {
                if( pages[ k ] is SocialPage ) {
                    socialPage = ( SocialPage ) pages[ k ];
                    friendNames = ( List<ClickableTextureComponent> ) typeof( SocialPage ).GetField( "friendNames", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( pages[ k ] );
                }
            }

            townsfolk.Clear();
            foreach( GameLocation location in Game1.locations ) {
                foreach( NPC npc in location.characters ) {
                    if( Game1.player.friendships.ContainsKey( npc.name ) ) {
                        townsfolk.Add( npc );
                    }
                }
            }

            checkboxes.Clear();

            // Create checkboxes in the same order as friends are
            foreach( ClickableTextureComponent friend in friendNames ) {

                int optionIndex = friend.name.GetHashCode();

                var checkbox = new OptionsCheckbox( "", optionIndex );
                checkboxes.Add( checkbox );

                // Disable checkbox if player has not talked to npc yet
                if( !( Game1.player.friendships.ContainsKey( friend.name ) ) ) {
                    checkbox.greyedOut = true;
                    checkbox.isChecked = false;
                }

                // Ensure an entry exists
                if( ModEntry.modData.locationOfTownsfolkOptions.ContainsKey( optionIndex ) == false ) {
                    ModEntry.modData.locationOfTownsfolkOptions.Add( optionIndex, false );
                }

                checkbox.isChecked = ModEntry.modData.locationOfTownsfolkOptions[ optionIndex ];
            }
        }

        public void toggleShowNPCLocationOnMap( bool setting ) {

            GraphicsEvents.OnPostRenderGuiEvent -= drawNPCLocationsOnMap;
            GraphicsEvents.OnPostRenderGuiEvent -= drawSocialPageOptions;
            ControlEvents.MouseChanged -= handleClick;
            MenuEvents.MenuChanged -= onMenuChange;

            if( setting ) {
                GraphicsEvents.OnPostRenderGuiEvent += drawNPCLocationsOnMap;
                GraphicsEvents.OnPostRenderGuiEvent += drawSocialPageOptions;
                ControlEvents.MouseChanged += handleClick;
                MenuEvents.MenuChanged += onMenuChange;
            }

        }

        private void handleClick( object sender, EventArgsMouseStateChanged e ) {
            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            if( e.NewState.LeftButton == ButtonState.Pressed ) {
                var slotPosition = ( int ) typeof( SocialPage ).GetField( "slotPosition", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( socialPage );

                for( int i = slotPosition; i < slotPosition + 5; i++ ) {
                    if( checkboxes[ i ].bounds.Contains( Game1.getMouseX(), Game1.getMouseY() ) && checkboxes[ i ].greyedOut == false ) {
                        checkboxes[ i ].isChecked = !checkboxes[ i ].isChecked;
                        ModEntry.modData.locationOfTownsfolkOptions[ checkboxes[ i ].whichOption ] = checkboxes[ i ].isChecked;
                    }
                }

            }
        }

    }
}
