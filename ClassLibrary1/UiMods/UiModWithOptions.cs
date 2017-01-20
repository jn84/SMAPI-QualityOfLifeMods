using System;
using System.Collections.Generic;

namespace DemiacleSvm.UiMods {
    abstract class UiModWithOptions {
        public Dictionary<string,Action<bool>> options = new Dictionary<string,Action<bool>>();

        public void addOption( string label, Action<bool> action, bool defaultValue = true ) {
            options.Add( label, action );

            // Create option value if it doesn't exist
            if( !ModEntry.modData.uiOptions.ContainsKey( label ) ) {
                ModEntry.modData.uiOptions.Add( label, defaultValue );
            }

        }

    }
}
