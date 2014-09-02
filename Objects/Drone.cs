using Game.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects
{
    class Drone : Unit
    {
        public Drone(string id)
            : base(id, "Drone_model", 20, 600, 300, 15, 100)
        {

        }
    }
}
