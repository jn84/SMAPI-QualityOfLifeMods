namespace DemiacleSvm {
    public class ModData {

        // Dictionary of int => bool is used because OptionsCheckbox uses int as a stored value.
        // The int used here is actually a hash value of the npc name string
        public SerializableDictionary<int, bool> locationOfTownsfolkOptions = new SerializableDictionary<int, bool>();

        // Dictionary of string => bool is used for easy creating and loading of checkboxes programatically
        // Look at UiModWithOptions for details
        public SerializableDictionary<string, bool> uiOptions = new SerializableDictionary<string, bool>(){ }; 

    }
}
