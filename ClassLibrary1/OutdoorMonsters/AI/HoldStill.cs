using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Demiacle_SVM.OutdoorMonsters.AI {

    public class HoldStill : MovementType {
        public override void calculateNextMovement( OutDoorMonster outDoorMonster ) {
            outDoorMonster.Halt();
        }
    }
}
