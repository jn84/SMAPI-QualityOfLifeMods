using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Monsters;
using StardewValley;
using System.Xml.Serialization;

namespace DemiacleSvm {
    public class ModData {


        // Dictionary of int => bool is used because OptionsCheckbox uses int as a stored value.
        // The int used here is actually a hash value of the npc name string
        public SerializableDictionary<int, bool> locationOfTownsfolkOptions = new SerializableDictionary<int, bool>();

        // Dictionary of string => bool is used for easy creating and loading of checkboxes programatically
        // Look at UiModWithOptions for details
        public SerializableDictionary<string, bool> uiOptions = new SerializableDictionary<string, bool>(){
            //{ SHOW_LUCK_ICON, true },
            //{ SHOW_EXPERIENCE_BAR, true }, 
            //{ EXPERIENCE_BAR_ALWAYS_VISIBLE, false },
            //{ SHOW_EXP_GAIN, true },
            //{ SHOW_LEVEL_UP_ANIMATION, true },
            //{ SHOW_HEART_FILLS, true},
            //{ SHOW_NPCS_ON_MAP, true},
            //{ SHOW_EXTRA_ITEM_INFORMATION, true}
        };

    }
}
