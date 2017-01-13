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

        public Boolean hasMonstersBeenCreated = false;
        public Boolean hasMonstersBeenLoaded = false;

        public const string SHOW_LUCK_ICON = "Show luck icon";
        public const string ALLOW_EXPERIENCE_BAR_TO_FADE_OUT = "Allow experience bar to fade out";
        public const string SHOW_EXPERIENCE_BAR = "Show experience bar";
        public const string SHOW_EXP_GAIN = "Show experience gain";
        public const string SHOW_LEVEL_UP_ANIMATION = "Show level up animation";
        public const string SHOW_HEART_FILLS = "Show heart fills";
        public const string SHOW_NPCS_ON_MAP = "Show npcs on map";
        public const string SHOW_EXTRA_ITEM_INFORMATION = "Show extra item information";

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
