using OpenTK;
using Seven.Mathematics;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects.Types.Properties
{
    public class Movable : Orientable
    {
        protected float _maxSpeed;
        protected Vector3 _velocity;
        protected float _maxAcceleration;

        public float MaxSpeed { get { return _maxSpeed; } }
        public float MaxAcceleration { get { return _maxAcceleration; } }
        public Vector3 Velocity { get { return _velocity; } set { _velocity = value; } }

        public Movable(string id, StaticModel staticModel, float maxRotSpeed, float viewDistance, float maxSpeed, float maxAcceleration)
            : base(id, staticModel, maxRotSpeed, viewDistance)
        {
            _maxSpeed = maxSpeed;
            _maxAcceleration = maxAcceleration;
            _velocity = new Vector3(0, 0, 0);
        }

        public Movable(string id, string staticModel, float maxRotSpeed, float viewDistance, float maxSpeed, float maxAcceleration)
            : base(id, staticModel, maxRotSpeed, viewDistance)
        {
            _maxSpeed = maxSpeed;
            _maxAcceleration = maxAcceleration;
            _velocity = new Vector3(0, 0, 0);
        }

        /* EXPERIMENTAL CODE - automatic moving for AI

        public void MoveTowards(Vector3 target)
        {
            //This algorithm makes this model move to the defined position. Position can be set again any time
            _mainModel.Position = getCinematics(_mainModel.Position, target, ref _velocity, _maxSpeed, _maxAcceleration);
        }

        private bool isEnoughBrakingSpace(Vector3 xt, Vector3 xf, Vector3 vt, float vmax, float amax)
        {
            return (xf-xt).Length > vmax * Game.DeltaTime + Math.Pow(vt.Length, 2) / (2f * amax);
        }

        private Vector3 getCinematics(Vector3 xt, Vector3 xf, ref Vector3 vt, float vmax, float amax)
        {

            if (xf != xt)
            {
                float delta = (xf - xt).Length;
                Vector3 vt_versor = (xf - xt).Normalized();
                float vt_magnitude = vt.Length;

                if (isEnoughBrakingSpace(xt, xf, vt, vmax, amax))
                {
                    Vector3 vt_next = vt_versor * (vt_magnitude + amax * Game.DeltaTime);
                    if (vt_magnitude < vmax && vt.Length < vmax)
                    {
                        vt = vt_next;
                    }
                    else
                    {
                        vt = vt_versor * vmax;
                    }
                }
                else
                {
                    Vector3 vt_next = vt_versor * (vt_magnitude - amax * Game.DeltaTime);
                    if (vt_magnitude > vmax && vt_next.Length > vmax)
                    {
                        vt = vt_next;
                    }
                    else if (delta > 0)
                    {
                        vt = vt_versor * delta;
                    }
                    else
                    {
                        vt = new Vector3(0, 0, 0);
                    }
                }

                xt = xt + vt * Game.DeltaTime;
            }

            return xt;

            /*
            if (xt != xf)
            {

                if (_movementPhase == 1)                //acceleration phase
                {
                    if (Vector3.Magnitude(vt) < vmax)
                    {
                        vt = vt + amax * Game.DeltaTime;        //accelerate
                        xt = xf - xt + vt * Game.DeltaTime;
                    }
                    else
                    {
                        vt = vmax;
                        xt = xf - xt + vt * Game.DeltaTime;     //start keep constant speed
                        _movementPhase = 2;
                    }
                }
                else if (_movementPhase == 2)           //maximum speed phase
                {
                    if ()
                    {
                        xt = xf - xt + vt * Game.DeltaTime;     //keep constant speed (until braking space will be enough)
                    }
                    else
                    {
                        vt = vt - amax * Game.DeltaTime;        //start to decelerate
                        xt = xf - xt + vt * Game.DeltaTime;
                        _movementPhase = 3;
                    }
                }
                else if (_movementPhase == 3)           //decelerating phase
                {
                    if (vt - amax * Game.DeltaTime > 0)
                    {
                        vt = vt - amax * Game.DeltaTime;        //keep decelerating
                        xt = xf - xt + vt * Game.DeltaTime;
                    }
                    else
                    {
                        if (xf - xt > 0)
                        {
                            vt = xf - xt;
                            xt = xf - xt + vt * Game.DeltaTime; //smooth deceleration before stopping
                        }
                        else
                        {
                            xt = xf;                            //stop
                            _movementPhase = 1;
                        }
                    }
                }

            }

            

        }
         */ 
    }
}
