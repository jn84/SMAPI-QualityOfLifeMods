using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demiacle_SVM.OutdoorMonsters.AI {
    public class MovementType {

        public virtual void calculateNextMovement( OutDoorMonster outDoorMonster  ) {
        }

        public virtual void updateOnEveryTick( OutDoorMonster outDoorMonster ) {
        }

    }
}
