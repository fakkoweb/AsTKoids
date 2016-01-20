// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14


using AsTKoids.Objects.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsTKoids.Objects
{
    class Drone : Unit
    {
        public Drone(string id)
            : base(id, "Drone_model", 20, 600, 300, 15, 100)
        {

        }
    }
}
