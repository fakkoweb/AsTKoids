using OpenTK;
using Seven.Mathematics;
using SevenEngine;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Game.Objects.Types.Properties
{
    class Orientable : Static
    {
        protected float _maxRotSpeed;
        protected float _viewDistance;

        public float MoveSpeed { get { return _maxRotSpeed; } set { _maxRotSpeed = value; } }
        public float ViewDistance { get { return _viewDistance; } set { _viewDistance = value; } }

        public Orientable(string id, StaticModel staticModel, float maxRotSpeed, float viewDistance) : base(id, staticModel)
        {
            _maxRotSpeed = maxRotSpeed;
            _viewDistance = viewDistance;
        }

        public Orientable(string id, string staticModel, float maxRotSpeed, float viewDistance) : base(id, staticModel)
        {
            _maxRotSpeed = maxRotSpeed;
            _viewDistance = viewDistance;
        }

        public virtual void FreeLookAt(Vector<float> targetAbsRef, Vector<float> upRelRef)
        {

            //Vector v1 = new Vector<float>(0, 0, -1);
            //Vector moveV = _staticModel.Position - vector;
            //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

            /*Vector forward = lookAt.Normalized();
            Vector right = Vector::Cross(up.Normalized(), forward);
            Vector up = Vector::Cross(forward, right);*/
            Vector<float> forward ;
            if (_mainModel.IsChild)
            {
                //Normalizing target and transforming it to local system
                forward = Geometric.Quaternion_Rotate(_mainModel.ParentModel.Orientation.Inverted(), targetAbsRef.Normalize()) - _mainModel.ParentModel.Position;
            }
            else
            {
                //Normalizing target.. local system is the same as world system
                forward = targetAbsRef.Normalize();
            }
            //Normalizing upVector (we are assuming it is expressed in local system)
            Vector<float> eye = _mainModel.PositionRelative.Normalize();
            Vector<float> up = upRelRef.Normalize();

            //Insert manual imprecision to avoid singularity
            if( Vector<float>.DotProduct(forward,up) == 1 )
            {
                forward.X += 0.001f;
                forward.Y += 0.001f;
                forward.Z += 0.001f;
            }

            //float angle = (float)Math.Acos( Vector<float>.DotProduct(current,targetAbsRef) );
            //Vector<float> rotAxis = Vector<float>.CrossProduct(current, forward).Normalize();
            //Vector<float> right = Vector<float>.CrossProduct(forward, up);

            Matrix4 lookAt_result = Matrix4.LookAt( eye.X, eye.Y, eye.Z, forward.X, forward.Y, forward.Z, up.X, up.Y, up.Z );
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

            _mainModel.OrientationRelative = Quaternion.Slerp(_mainModel.OrientationRelative, targetRelOrientation_quaternion, Game.DeltaTime * 0.001f);

        }

        public virtual void ConstraintLookAt(Vector<float> targetAbsRef, Vector<float> upRelRef, Vector<float> constraintAxisSelect)
        {

            Vector<float> targetRelRef;

            if (_mainModel.IsChild)
                targetRelRef = Geometric.Quaternion_Rotate(_mainModel.ParentModel.Orientation.Inverted(), targetAbsRef);
            else
                targetRelRef = targetAbsRef;

            if (constraintAxisSelect.X != 0)
                targetRelRef.X = 0;
            else if (constraintAxisSelect.Y != 0)
                targetRelRef.Y = 0;
            else if (constraintAxisSelect.Z != 0)
                targetRelRef.Z = 0;

            //Vector v1 = new Vector<float>(0, 0, -1);
            //Vector moveV = _staticModel.Position - vector;
            //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

            /*Vector forward = lookAt.Normalized();
            Vector right = Vector::Cross(up.Normalized(), forward);
            Vector up = Vector::Cross(forward, right);*/
            Vector<float> forward;
            if (_mainModel.IsChild)
            {
                //Normalizing target and transforming it to local system
                forward = targetRelRef.Normalize() - _mainModel.ParentModel.Position;
            }
            else
            {
                //Normalizing target.. local system is the same as world system
                forward = targetRelRef.Normalize();
            }
            //Normalizing upVector (we are assuming it is expressed in local system)
            Vector<float> eye = _mainModel.PositionRelative.Normalize();
            Vector<float> up = upRelRef.Normalize();

            //Insert manual imprecision to avoid singularity
            if (Vector<float>.DotProduct(forward, up) == 1)
            {
                forward.X += 0.001f;
                forward.Y += 0.001f;
                forward.Z += 0.001f;
            }

            //float angle = (float)Math.Acos( Vector<float>.DotProduct(current,targetAbsRef) );
            //Vector<float> rotAxis = Vector<float>.CrossProduct(current, forward).Normalize();
            //Vector<float> right = Vector<float>.CrossProduct(forward, up);

            Matrix4 lookAt_result = Matrix4.LookAt(eye.X, eye.Y, eye.Z, forward.X, forward.Y, forward.Z, up.X, up.Y, up.Z);
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

            _mainModel.OrientationRelative = Quaternion.Slerp(_mainModel.OrientationRelative, targetRelOrientation_quaternion, Game.DeltaTime * 0.001f);

        }
    }
}
