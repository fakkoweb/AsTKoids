﻿using Seven.Mathematics;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects.Types.Properties
{
    class Movable : Orientable
    {
        protected float _maxSpeed;
        protected Vector<float> _velocity;
        protected float _maxAcceleration;

        public float Speed { get { return Vector<float>.Magnitude(_velocity); } }
        public Vector<float> Velocity { get { return _velocity; } }

        public Movable(string id, StaticModel staticModel, float maxRotSpeed, float viewDistance, float maxSpeed, float maxAcceleration)
            : base(id, staticModel, maxRotSpeed, viewDistance)
        {
            _maxSpeed = maxSpeed;
            _maxAcceleration = maxAcceleration;
            _velocity = new Vector<float>(0, 0, 0);
        }

        public Movable(string id, string staticModel, float maxRotSpeed, float viewDistance, float maxSpeed, float maxAcceleration)
            : base(id, staticModel, maxRotSpeed, viewDistance)
        {
            _maxSpeed = maxSpeed;
            _maxAcceleration = maxAcceleration;
            _velocity = new Vector<float>(0, 0, 0);
        }

        public void MoveTowards(Vector<float> target)
        {
            //This algorithm makes this model move to the defined position. Position can be set again any time
            _mainModel.Position = getCinematics(_mainModel.Position, target, ref _velocity, _maxSpeed, _maxAcceleration);
        }

        private bool isEnoughBrakingSpace(Vector<float> xt, Vector<float> xf, Vector<float> vt, float vmax, float amax)
        {
            return Vector<float>.Magnitude(xf - xt) > vmax * Game.DeltaTime + Math.Pow(Vector<float>.Magnitude(vt), 2) / (2f * amax);
        }

        private Vector<float> getCinematics(Vector<float> xt, Vector<float> xf, ref Vector<float> vt, float vmax, float amax)
        {

            if (xf != xt)
            {
                Vector<float> vt_versor = (xf - xt).Normalize();
                float vt_magnitude = Vector<float>.Magnitude(vt);

                if (isEnoughBrakingSpace(xt, xf, vt, vmax, amax))
                {
                    Vector<float> vt_next = vt_versor * (vt_magnitude + amax * Game.DeltaTime);
                    if (vt_magnitude < vmax && Vector<float>.Magnitude(vt_next) < vmax)
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
                    Vector<float> vt_next = vt_versor * (vt_magnitude - amax * Game.DeltaTime);
                    if (vt_magnitude > vmax && Vector<float>.Magnitude(vt_next) > vmax)
                    {
                        vt = vt_next;
                    }
                    else if (Vector<float>.Magnitude(xf - xt) > 0)
                    {
                        vt = vt_versor * Vector<float>.Magnitude(xf - xt);
                    }
                    else
                    {
                        vt = new Vector<float>(0, 0, 0);
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
                    if (Vector<float>.Magnitude(vt) < vmax)
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

            */

        }
    }
}
