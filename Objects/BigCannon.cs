using Seven.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects
{
    class BigCannon : Cannon
    {
        public BigCannon(string id)
            : base(id, "Cannon_big_model", 20, 8000, 1000, new Vector<float>(0, 0, 0), new Vector<float>(0, 0, 0))
        {
            // TODO: set bullet vectors properly
            
        }

        public override void LookAt(Seven.Mathematics.Vector<float> targetRef, Seven.Mathematics.Vector<float> upRef)
        {
 	         base.LookAt(targetRef, upRef);
        }
    }
}
