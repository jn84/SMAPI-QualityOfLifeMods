using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace Demiacle_SVM.UiMods {
    /// <summary>
    /// Displays mugshots of townsfolk on the map.
    /// </summary>
    class UiModLocationOfTownsfolk {

        List<NPC> townsfolk = new List<NPC>();

        private UiModAccurateHearts uiModAccurateHearts;

        public UiModLocationOfTownsfolk( UiModAccurateHearts uiModAccurateHearts ) {
            this.uiModAccurateHearts = uiModAccurateHearts;
            GraphicsEvents.OnPostRenderGuiEvent += onPostRenderEvent;
            MenuEvents.MenuChanged += onMenuChange;
        }

        internal void onPostRenderEvent( object sender, EventArgs e ) {

            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            GameMenu currentMenu = (GameMenu) Game1.activeClickableMenu;

            if(  currentMenu.currentTab != GameMenu.mapTab ) {
                return;
            }
            
            foreach( NPC npc in townsfolk ) {

                int key = npc.name.GetHashCode();

                // If doesn't contain a key from social page then dont display anything
                if( !( uiModAccurateHearts.savedData.ContainsKey( key ) ) ) {
                    continue;

                // If key exists and value is false dont display anything
                } else if ( uiModAccurateHearts.savedData[ key ] == false ) {
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

                ClickableTextureComponent npcMugShot = new ClickableTextureComponent( npc.name, new Rectangle( positionX + offsetX, positionY + offsetY, Game1.activeClickableMenu.width, Game1.tileSize ), ( string ) null, npc.name, npc.sprite.Texture, rect, ( float ) Game1.pixelZoom, false );
                npcMugShot.scale = 3f;
                npcMugShot.draw( Game1.spriteBatch );
                
                return;
            }
        }

        /// <summary>
        /// Resets and populates the list of townsfolk to display every time the game menu is called
        /// </summary>
        internal void onMenuChange( object sender, EventArgsClickableMenuChanged e ) {
            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }

            townsfolk.Clear();
            foreach( GameLocation location in Game1.locations ) {
                foreach( NPC npc in location.characters ) {
                    if( Game1.player.friendships.ContainsKey( npc.name ) ) {
                        townsfolk.Add( npc );
                    }
                }
            }
        }

    }
}
