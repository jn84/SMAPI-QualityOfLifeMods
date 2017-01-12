using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemiacleSvm.UiMods {
    abstract class UiModWithOptions {
        public Dictionary<string,Action<bool>> options = new Dictionary<string,Action<bool>>();
    }
}
