using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace Demiacle.ImprovedQualityOfLife {
    public class ScytheDamageMod {

        public ScytheDamageMod() {
            PlayerEvents.InventoryChanged += onInvChange;
        }

        /// <summary>
        /// Makes all weapons only deal 1 dmg... mwahahahahaha
        /// Also replaces the Scythe with a SuperScythe.
        /// </summary>
        internal void onInvChange( object sender, EventArgsInventoryChanged e ) {
            foreach( StardewModdingAPI.Inheritance.ItemStackChange item in e.Removed ) {
                ModEntry.Log( "onInvChange " + item.Item.Name );
            }

            // makes all weapons weak af!
            for( int i = 0; i < e.Inventory.Count; i++ ) {
                if( e.Inventory[i] is MeleeWeapon && e.Inventory[i].Name != "Scythe" ) {
                    ModEntry.Log( e.Inventory[ i ].Name );
                    ( ( MeleeWeapon ) e.Inventory[ i ] ).minDamage = 1;
                    ( ( MeleeWeapon ) e.Inventory[ i ] ).maxDamage = 1;
                } else if( e.Inventory[ i ] is MeleeWeapon && e.Inventory[ i ].Name == "Scythe" ) {
                    e.Inventory[ i ] = null;
                    e.Inventory[ i ] = (Tool)(new Scythe());
                }
            }
        }

        public class Scythe : MeleeWeapon{
            public Scythe() : base( 47 ) {
                maxDamage = 200;
                minDamage = 100;
                //name = "SuperScythe"; this fails because crucial game code has checks against the name Scythe
                description = "Not your ordinary everyday scythe.";
                //salePrice(0);
                //( this as MeleeWeapon).price = 0;
                
            }

            public override int salePrice() {
                return 0;
            }

            public override void leftClick( Farmer who ) {
                base.leftClick( who );
                if( !who.UsingTool ) {
                    //who.stamina -= 10;
                }
            }
        }
    }
}