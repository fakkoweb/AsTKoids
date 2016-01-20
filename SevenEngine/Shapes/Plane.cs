using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seven.Mathematics;
using OpenTK;

namespace SevenEngine.Physics.Primitives
{
    public class Plane
    {
        public Vector3 Normal;
        public float Dist;

        public Plane(float nx, float ny, float nz, float d)
        {
            Normal = new Vector3(nx, ny, nz);
            Dist = d;
        }
        public Plane(float[] values)
            : this(values[0], values[1], values[2], values[3])
        {

        }

        public float A
        {
            get
            {
                return Normal.X;
            }
        }
        public float B
        {
            get
            {
                return Normal.Y;
            }
        }
        public float C
        {
            get
            {
                return Normal.Z;
            }
        }
        public float D
        {
            get
            {
                return Dist;
            }
        }

        public static Plane FromPointAndNormal(Vector3 p, Vector3 n)
        {
            return new Plane(n.X, n.Y, n.Z, Vector3.Dot(n, p));
        }

        public float Intersect(Vector3 p0, Vector3 p1)
        {
            Vector3 Direction = p1 - p0;
            float denominator = Vector3.Dot(Normal, Direction);
            if (denominator == 0)
            {
                throw new DivideByZeroException("Can not get the intersection of a plane with a line when they are parallel to each other.");
            }
            float t = (Dist - Vector3.Dot(Normal, p0)) / denominator;
            return t;
        }

        public Vector3 GetIntersection(Vector3 p0, Vector3 p1)
        {

            Vector3 Direction = p1 - p0;
            float denominator = Vector3.Dot(Normal, Direction);
            if (denominator == 0)
            {
                throw new DivideByZeroException("Can not get the intersection of a plane with a line when they are parallel to each other.");
            }
            float t = (Dist - Vector3.Dot(Normal, p0)) / denominator;
            return p0 + Direction * t;
        }
    }
}
