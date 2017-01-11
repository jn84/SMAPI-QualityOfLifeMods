using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Monsters;
using StardewValley;
using System.Xml.Serialization;

namespace Demiacle_SVM {
    public class ModData {

        public Boolean hasMonstersBeenCreated = false;
        public Boolean hasMonstersBeenLoaded = false;

        public static readonly string SHOW_LUCK_ICON = "Show luck icon";
        public static readonly string SHOW_EXPERIENCE_BAR = "Show experience bar";
        public static readonly string EXPERIENCE_BAR_ALWAYS_VISIBLE = "Experience bar always invisible";
        public static readonly string SHOW_HEAR_FILLS = "Show heart fills";
        public static readonly string SHOW_NPCS_ON_MAP = "Show npcs on map";
        public static readonly string SHOW_EXTRA_ITEM_INFORMATION = "Show extra item information";
            
            
            
            
        
        // Dictionary of string => bool is used for easy creating and loading of checkboxes programatically
        public SerializableDictionary<string, bool> uiOptions = new SerializableDictionary<string, bool>(){
            { SHOW_LUCK_ICON, true },
            { SHOW_EXPERIENCE_BAR, true }, 
            { EXPERIENCE_BAR_ALWAYS_VISIBLE, false },
            { SHOW_HEAR_FILLS, true},
            { SHOW_NPCS_ON_MAP, true},
            { SHOW_EXTRA_ITEM_INFORMATION, true}
        };

    }
}
