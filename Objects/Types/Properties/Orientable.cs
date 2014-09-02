using Seven.Mathematics;
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

        public virtual void LookAt(Vector<float> targetRef, Vector<float> upRef)
        {
            if (!_mainModel.IsChild)
            {
                //Vector v1 = new Vector<float>(0, 0, -1);
                //Vector moveV = _staticModel.Position - vector;
                //Vector v2 = moveV.RotateBy(_staticModel.Orientation.W, 0, 1, 0);

                /*Vector forward = lookAt.Normalized();
                Vector right = Vector::Cross(up.Normalized(), forward);
                Vector up = Vector::Cross(forward, right);*/

                Vector<float> forward = targetRef.Normalize();
                Vector<float> up = upRef.Normalize();
                Vector<float> right = Vector<float>.CrossProduct(forward, up);

                Quaternion<float> targetOrientation = Quaternion<float>.FactoryIdentity;
                targetOrientation.W = (float)Math.Sqrt((double)(1.0f + right.X + up.Y + forward.Z)) * 0.5f;
                float w4_recip = 1.0f / (4.0f * targetOrientation.W);
                targetOrientation.X = (forward.Y - up.Z) * w4_recip;
                targetOrientation.Y = (right.Z - forward.X) * w4_recip;
                targetOrientation.Z = (up.X - right.Y) * w4_recip;

                _mainModel.Orientation.Slerp(targetOrientation, Game.DeltaTime * _maxRotSpeed);
            }
            else
            {


            }
        }

    }
}
