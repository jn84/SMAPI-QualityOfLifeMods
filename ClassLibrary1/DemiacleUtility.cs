using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Demiacle_SVM {
    class DemiacleUtility {

        /// <summary>
        /// Creates a dialogue that only shows up if a menu is not open. And retries every second to see if menu has closed
        /// This is needed to stop a bug of disapearing items on forced exits of menu
        /// </summary>
        public static void createSafeDelayedDialogue( string dialogue, int timer ) {
            Task.Factory.StartNew( () => {
                System.Threading.Thread.Sleep( timer );
                while( true ) {
                    System.Threading.Thread.Sleep( 1000 );
                    if( !( Game1.activeClickableMenu is StardewValley.Menus.GameMenu ) ) {
                        Game1.setDialogue( dialogue, true );
                        return;
                    }
                }
            } );
        }

        /// <summary>
        /// Uses reflection to copy fields from one object to the other. This allows greater extendibility
        /// </summary>
        /// <param name="objectToCopyTo"></param>
        /// <param name="objectToCopyFrom"></param>
        public static void copyFields( object objectToCopyTo, object objectToCopyFrom ) {
            Type typeToUse = objectToCopyFrom.GetType();

            FieldInfo[] fields = typeToUse.GetFields( BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public );

            foreach( FieldInfo field in fields ) {
                var fieldToCopy = field.GetValue( objectToCopyFrom );
                field.SetValue( objectToCopyTo, fieldToCopy );
            }
        }

    }
}
