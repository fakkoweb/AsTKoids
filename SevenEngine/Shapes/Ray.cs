// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seven.Mathematics;
using OpenTK;

namespace SevenEngine.Physics.Primitives
{
    public class Ray
    {
        public Vector3 Start;
        public Vector3 End;
        public Vector3 Direction
        {
            get
            {
                Vector3 n = End - Start;
                n.Normalize();
                return n;
            }
            set
            {
                End = Start + Direction;
            }
        }
        public float Length
        {
            get
            {
                return (Start - End).Length;
            }
            set
            {
                End = Start + Direction * value;
            }
        }

        public bool Intersect(Sphere s)
        {
            Plane p = Plane.FromPointAndNormal(s.Position, Direction);
            float f = p.Intersect(Start, End);
            if (f < 0) f = 0;
            else if (f > 1) f = 1;
            Vector3 nearest = Start + f * (End - Start);
            float dist = (s.Position - nearest).Length;
            return dist <= s.Radius;
        }

        public Ray(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }
    }
}
