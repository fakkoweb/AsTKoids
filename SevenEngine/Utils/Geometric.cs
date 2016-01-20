// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14

using OpenTK;
using Seven.Mathematics;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenEngine
{
    public static class Geometric
    {

        public static Matrix<float> Matrix_From_Quaternion(Quaternion q)
        {
            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;
            float m00 = (float)(sqx - sqy - sqz + sqw);
            float m11 = (float)(-sqx + sqy - sqz + sqw);
            float m22 = (float)(-sqx - sqy + sqz + sqw);

            double tmp1 = q.X * q.Y;
            double tmp2 = q.Z * q.W;
            float m10 = (float)(2.0 * (tmp1 + tmp2));
            float m01 = (float)(2.0 * (tmp1 - tmp2));

            tmp1 = q.X * q.Z;
            tmp2 = q.Y * q.W;
            float m20 = (float)(2.0 * (tmp1 - tmp2));
            float m02 = (float)(2.0 * (tmp1 + tmp2));

            tmp1 = q.Y * q.Z;
            tmp2 = q.X * q.W;
            float m21 = (float)(2.0 * (tmp1 + tmp2));
            float m12 = (float)(2.0 * (tmp1 - tmp2));

            float[,] matrix_components = new float[3, 3] { { m00, m01, m02 }, { m10, m11, m12 }, { m20, m21, m22 } };
            return new Matrix<float>(matrix_components);

        }
   
        public static Vector3 Quaternion_Rotate(Quaternion q, Vector3 v)
        {

            Matrix<float> m = Matrix_From_Quaternion(q);

            Vector3 result = new Vector3(v.X * m[0, 0] + v.Y * m[0, 1] + v.Z * m[0, 2], v.X * m[1, 0] + v.Y * m[1, 1] + v.Z * m[1, 2], v.X * m[2, 0] + v.Y * m[2, 1] + v.Z * m[2, 2]);

            return result;
        }

        public static Quaternion Generate_Quaternion(float angle, float x, float y, float z)
        {
            Quaternion result = new Quaternion( (float)Math.Sin(angle / 2) * x, (float)Math.Sin(angle / 2) * y, (float)Math.Sin(angle / 2) * z, (float)Math.Cos(angle / 2) );
            if (angle == 0 || (x == 0 && y == 0 && z == 0))
                return new Quaternion(0, 0, 0, 1);
            else
            {
                return result;
            }
            
        }

        public static Quaternion FreeLookAt(StaticModel actor, Vector3 targetAbsRef, Vector3 upRelRef)
        {

            //Vector v1 = new Vector3(0, 0, -1);
            //Vector moveV = _staticModel.Position - vector;
            //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

            /*Vector forward = lookAt.Normalized();
            Vector right = Vector::Cross(up.Normalized(), forward);
            Vector up = Vector::Cross(forward, right);*/
            Vector3 eye = actor.Position;
            Vector3 forward;
            Vector3 up;

            //Normalizing target.. local system is the same as world system
            forward = targetAbsRef;
            up = upRelRef;

            //Insert manual imprecision to avoid singularity
            if (Vector3.Dot(forward, up) == 1)
            {
                forward.X += 0.001f;
                forward.Y += 0.001f;
                forward.Z += 0.001f;
            }
            //forward = forward.Normalized();

            // HACK INVERSION
            //forward = -forward;

            //float angle = (float)Math.Acos( Vector3.Dot(current,targetAbsRef) );
            //Vector3 rotAxis = Vector3.CrossProduct(current, forward).Normalized();
            //Vector3 right = Vector3.CrossProduct(forward, up);

            Matrix4 lookAt_result = Matrix4.LookAt(forward.X, forward.Y, forward.Z, eye.X, eye.Y, eye.Z, up.X, up.Y, up.Z);
            Matrix3 targetRelOrientation_matrix = new Matrix3(lookAt_result);
            Quaternion targetRelOrientation_quaternion = Quaternion.FromMatrix(targetRelOrientation_matrix);

            /*
            Quaternion targetRelOrientation_quaternion = new Quaternion();
            targetRelOrientation_quaternion.W = (float)Math.Sqrt((double)(1.0f + right.X + up.Y + forward.Z)) * 0.5f;
            float w4_recip = 1.0f / (4.0f * targetRelOrientation_quaternion.W);
            targetRelOrientation_quaternion.X = (forward.Y - up.Z) * w4_recip;
            targetRelOrientation_quaternion.Y = (right.Z - forward.X) * w4_recip;
            targetRelOrientation_quaternion.Z = (up.X - right.Y) * w4_recip;
            */

            return targetRelOrientation_quaternion;

        }

        public static Quaternion FreeLookAt(Vector3 actorEye, Vector3 targetAbsRef, Vector3 upRelRef)
        {

            //Vector v1 = new Vector3(0, 0, -1);
            //Vector moveV = _staticModel.Position - vector;
            //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

            /*Vector forward = lookAt.Normalized();
            Vector right = Vector::Cross(up.Normalized(), forward);
            Vector up = Vector::Cross(forward, right);*/
            Vector3 eye = actorEye;
            Vector3 forward;
            Vector3 up;

            //Normalizing target.. local system is the same as world system
            forward = targetAbsRef;
            up = upRelRef;

            //Insert manual imprecision to avoid singularity
            if (Vector3.Dot(forward, up) == 1)
            {
                forward.X += 0.001f;
                forward.Y += 0.001f;
                forward.Z += 0.001f;
            }
            //forward = forward.Normalized();

            // HACK INVERSION
            //forward = -forward;

            //float angle = (float)Math.Acos( Vector3.Dot(current,targetAbsRef) );
            //Vector3 rotAxis = Vector3.CrossProduct(current, forward).Normalized();
            //Vector3 right = Vector3.CrossProduct(forward, up);

            Matrix4 lookAt_result = Matrix4.LookAt(forward.X, forward.Y, forward.Z, eye.X, eye.Y, eye.Z, up.X, up.Y, up.Z);
            Matrix3 targetRelOrientation_matrix = new Matrix3(lookAt_result);
            Quaternion targetRelOrientation_quaternion = Quaternion.FromMatrix(targetRelOrientation_matrix);

            /*
            Quaternion targetRelOrientation_quaternion = new Quaternion();
            targetRelOrientation_quaternion.W = (float)Math.Sqrt((double)(1.0f + right.X + up.Y + forward.Z)) * 0.5f;
            float w4_recip = 1.0f / (4.0f * targetRelOrientation_quaternion.W);
            targetRelOrientation_quaternion.X = (forward.Y - up.Z) * w4_recip;
            targetRelOrientation_quaternion.Y = (right.Z - forward.X) * w4_recip;
            targetRelOrientation_quaternion.Z = (up.X - right.Y) * w4_recip;
            */

            return targetRelOrientation_quaternion;

        }

        // WARNING EXPERIMENTAL FUNCTIONS: behaviour undefined!

        public static Quaternion OLDFreeLookAt(StaticModel actor, Vector3 targetAbsRef, Vector3 upRelRef)
        {

            //Vector v1 = new Vector3(0, 0, -1);
            //Vector moveV = _staticModel.Position - vector;
            //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

            /*Vector forward = lookAt.Normalized();
            Vector right = Vector::Cross(up.Normalized(), forward);
            Vector up = Vector::Cross(forward, right);*/
            Vector3 eye = actor.PositionRelative;
            Vector3 forward;
            Vector3 up;

            
            if (actor.IsChild)
            {
                //Normalizing target and transforming it to local system
                forward = Geometric.Quaternion_Rotate(actor.ParentModel.Orientation.Inverted(),targetAbsRef) - actor.ParentModel.Position;
                up = upRelRef;
            }
            else
            {
                //Normalizing target.. local system is the same as world system
                forward = targetAbsRef;
                up = upRelRef;
            }
            //Normalizing upVector (we are assuming it is expressed in local system)


            //Insert manual imprecision to avoid singularity
            if (Vector3.Dot(forward, up) == 1)
            {
                forward.X += 0.001f;
                forward.Y += 0.001f;
                forward.Z += 0.001f;
            }
            //forward = forward.Normalized();

            // HACK INVERSION
            //forward = -forward;

            //float angle = (float)Math.Acos( Vector3.Dot(current,targetAbsRef) );
            //Vector3 rotAxis = Vector3.CrossProduct(current, forward).Normalized();
            //Vector3 right = Vector3.CrossProduct(forward, up);

            Matrix4 lookAt_result = Matrix4.LookAt(forward.X, forward.Y, forward.Z, eye.X, eye.Y, eye.Z, up.X, up.Y, up.Z);
            Matrix3 targetRelOrientation_matrix = new Matrix3(lookAt_result);
            Quaternion targetRelOrientation_quaternion = Quaternion.FromMatrix(targetRelOrientation_matrix);

            /*
            Quaternion targetRelOrientation_quaternion = new Quaternion();
            targetRelOrientation_quaternion.W = (float)Math.Sqrt((double)(1.0f + right.X + up.Y + forward.Z)) * 0.5f;
            float w4_recip = 1.0f / (4.0f * targetRelOrientation_quaternion.W);
            targetRelOrientation_quaternion.X = (forward.Y - up.Z) * w4_recip;
            targetRelOrientation_quaternion.Y = (right.Z - forward.X) * w4_recip;
            targetRelOrientation_quaternion.Z = (up.X - right.Y) * w4_recip;
            */

            return targetRelOrientation_quaternion;

        }

        public static Quaternion FreeLookAtWithConstraint(StaticModel actor, Vector3 targetAbsRef, Vector3 upRelRef)
        {

            //Vector v1 = new Vector3(0, 0, -1);
            //Vector moveV = _staticModel.Position - vector;
            //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

            /*Vector forward = lookAt.Normalized();
            Vector right = Vector::Cross(up.Normalized(), forward);
            Vector up = Vector::Cross(forward, right);*/
            Vector3 eye = actor.PositionRelative;
            Vector3 forward;
            Vector3 up;

            if (actor.IsChild)
            {
                //Normalizing target and transforming it to local system
                forward = Geometric.Quaternion_Rotate(actor.ParentModel.Orientation.Inverted(), targetAbsRef) - actor.ParentModel.Position;
                up = Quaternion_Rotate(actor.OrientationRelative, upRelRef);
            }
            else
            {
                //Normalizing target.. local system is the same as world system
                forward = targetAbsRef;
                up = upRelRef;
            }
            //Normalizing upVector (we are assuming it is expressed in local system)


            //Insert manual imprecision to avoid singularity
            if (Vector3.Dot(forward, up) == 1)
            {
                forward.X += 0.001f;
                forward.Y += 0.001f;
                forward.Z += 0.001f;
            }
            //forward = forward.Normalized();

            // HACK INVERSION
            //forward = -forward;

            //float angle = (float)Math.Acos( Vector3.Dot(current,targetAbsRef) );
            //Vector3 rotAxis = Vector3.CrossProduct(current, forward).Normalized();
            //Vector3 right = Vector3.CrossProduct(forward, up);
            //Output.WriteLine(forward.X + "," + forward.Y + "," + forward.Z);
            Matrix4 lookAt_result;
            Matrix3 targetRelOrientation_matrix;
            Quaternion targetRelOrientation_quaternion;
            if (forward.X >= 0 && forward.Y >= 0)
            {
                lookAt_result = Matrix4.LookAt(forward.X, forward.Y, forward.Z, eye.X, eye.Y, eye.Z, up.X, up.Y, up.Z);
                targetRelOrientation_matrix = new Matrix3(lookAt_result);
                targetRelOrientation_quaternion = Quaternion.FromMatrix(targetRelOrientation_matrix);
            }
            else
            {
                targetRelOrientation_quaternion = actor.OrientationRelative;
            }
            /*
            Quaternion targetRelOrientation_quaternion = new Quaternion();
            targetRelOrientation_quaternion.W = (float)Math.Sqrt((double)(1.0f + right.X + up.Y + forward.Z)) * 0.5f;
            float w4_recip = 1.0f / (4.0f * targetRelOrientation_quaternion.W);
            targetRelOrientation_quaternion.X = (forward.Y - up.Z) * w4_recip;
            targetRelOrientation_quaternion.Y = (right.Z - forward.X) * w4_recip;
            targetRelOrientation_quaternion.Z = (up.X - right.Y) * w4_recip;
            */

            return targetRelOrientation_quaternion;

        }

        public static Quaternion ConstraintLookAt(StaticModel actor, Vector3 targetAbsRef, Vector3 upRelRef, Vector3 constraintAxisSelect)
        {

            Vector3 targetRelRef;

            //Transforming target into relative coordinates if child
            if (actor.IsChild)
                targetRelRef = Geometric.Quaternion_Rotate(actor.ParentModel.Orientation.Inverted(), targetAbsRef);
            else
                targetRelRef = new Vector3(targetAbsRef.X, targetAbsRef.Y, targetAbsRef.Z);

            
            // HACK INVERSION
            //targetRelRef = -targetRelRef;
            //
            

            //Make a non projected safe copy of relative target
            Vector3 targetRelRef_notConstraint = new Vector3(targetRelRef.X, targetRelRef.Y, targetRelRef.Z);

            /*
            //Project relative target to plane defined by constraint
            if (constraintAxisSelect.X != 0)
                targetRelRef.X = 0;
            else if (constraintAxisSelect.Y != 0)
                targetRelRef.Y = 0;
            else if (constraintAxisSelect.Z != 0)
                targetRelRef.Z = 0;
             */

            //Normalizing constrainted relative target
            Vector3 eye = actor.PositionRelative;
            Vector3 up;
            Vector3 forward;
            if (actor.IsChild)
            {
                //Normalizing target and transforming it to local system
                forward = targetRelRef - actor.ParentModel.Position;
                up = Quaternion_Rotate(actor.OrientationRelative, upRelRef);
            }
            else
            {
                //Normalizing target.. local system is the same as world system
                forward = targetRelRef;
                up = upRelRef;
            }

            //Normalizing upVector (we are assuming it is expressed in local system)


            //Insert manual imprecision tilt to avoid singularity  (if any!)
            float ciao = Vector3.Dot(targetRelRef_notConstraint.Normalized(), up);
            if (Vector3.Dot(targetRelRef_notConstraint.Normalized(), up) == 1)
            {
                forward.X = 0.001f;
                forward.Y = 0.001f;
                forward.Z = 0.001f;
            }

            //Project relative target to plane defined by constraint
            if (constraintAxisSelect.X != 0)
                forward.X = 0;
            else if (constraintAxisSelect.Y != 0)
                forward.Y = 0;
            else if (constraintAxisSelect.Z != 0)
                forward.Z = 0;

            //forward = forward.Normalized();

//            Vector3 v1 = new Vector3(forward.X,forward.Y,forward.Z)
//            float ciao = Vector3.Dot(forward, up);


            //float angle = (float)Math.Acos( Vector3.Dot(current,targetAbsRef) );
            //Vector3 rotAxis = Vector3.CrossProduct(current, forward).Normalized();
            //Vector3 right = Vector3.CrossProduct(forward, up);

            Matrix4 lookAt_result = Matrix4.LookAt(forward.X, forward.Y, forward.Z, eye.X, eye.Y, eye.Z, up.X, up.Y, up.Z);
            Matrix3 targetRelOrientation_matrix = new Matrix3(lookAt_result);
            Quaternion targetRelOrientation_quaternion = Quaternion.FromMatrix(targetRelOrientation_matrix);

            /*
            Quaternion targetRelOrientation_quaternion = new Quaternion();
            targetRelOrientation_quaternion.W = (float)Math.Sqrt((double)(1.0f + right.X + up.Y + forward.Z)) * 0.5f;
            float w4_recip = 1.0f / (4.0f * targetRelOrientation_quaternion.W);
            targetRelOrientation_quaternion.X = (forward.Y - up.Z) * w4_recip;
            targetRelOrientation_quaternion.Y = (right.Z - forward.X) * w4_recip;
            targetRelOrientation_quaternion.Z = (up.X - right.Y) * w4_recip;
            */

            return targetRelOrientation_quaternion;

        }
    }
}
