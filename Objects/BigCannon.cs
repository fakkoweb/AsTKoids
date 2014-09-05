using OpenTK;
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
            : base(id, "Cannon_big_model", 20, 8000, 1000, new Vector3(0, 0, 0), new Vector3(0, 0, 0))
        {
            // TODO: set bullet vectors properly
            
        }

        public override void FreeLookAt(Vector3 targetRef, Vector3 upRef)
        {
 	         base.FreeLookAt(targetRef, upRef);
        }
    }
}
