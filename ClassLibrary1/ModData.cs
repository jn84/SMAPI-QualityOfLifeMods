using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Monsters;

namespace Demiacle_SVM {
    public class ModData {
        public Boolean hasMonstersBeenCreated = false;

        //public Boolean usePersistantMonstersMod = true;
        //public Boolean useScytheDamageMod = true;

        List<Monster> persistantMonsters = new List<Monster>();

        public ModData() {
        }
    }
}
