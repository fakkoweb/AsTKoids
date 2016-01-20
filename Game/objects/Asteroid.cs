// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14

using AsTKoids.Objects.Types.Behaviours;
using OpenTK;
using SevenEngine;
using SevenEngine.Physics.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsTKoids.Objects
{
    public class Asteroid : Movable, Damageable
    {
        protected int _health;
        protected int _damage;
        protected bool _isDead;
        Sphere _box;

        public int Health { get { return _health; } set { _health = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public virtual bool IsDead { get { return _isDead; } set { _isDead = value; } }
        public Sphere BoundingBox { get { return _box; } set { _box = value; } }

        public override Vector3 Position { get { return _mainModel.Position; } set { _mainModel.Position = value; _box.Position = _mainModel.PositionRelative; } }
        public override Vector3 PositionRelative { get { return _mainModel.PositionRelative; } set { _mainModel.PositionRelative = value; _box.Position = _mainModel.PositionRelative; } }
        public override Quaternion Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                base.Orientation = value;
                _box.Orientation = _mainModel.OrientationRelative;
            }
        }
        public override Quaternion OrientationRelative
        {
            get
            {
                return base.OrientationRelative;
            }
            set
            {
                base.OrientationRelative = value;
                _box.Orientation = _mainModel.OrientationRelative;
            }
        }
        public override Vector3 Scale { get { return _mainModel.Scale; } set { _mainModel.Scale = value; _box.Radius= _box.Radius*_mainModel.Scale.X; } }


        public Asteroid(string id, int health)
            : base(id, "Asteroid_model", 1, 0, 0, 0)
        {
            _box = new Sphere(new Vector3(0, 0, 0), 1.45f, Geometric.Generate_Quaternion(0, 0, 0, 0));
            _health = health;
            _damage = 0;
            _isDead = false;
        }

        public void Move()
        {
            Position = new Vector3(Position.X + Velocity.X * Game.DeltaTime, Position.Y + Velocity.Y * Game.DeltaTime, Position.Z + Velocity.Z * Game.DeltaTime);
        }

        public int Hit(int damage)
        {
            _damage = _damage + damage;
            if (_damage >= _health)
            {
                _isDead = true;
                return _health;
            }
            else
            {
                return 0;
            }
        }

        public bool HasCollided(Vector3 point)
        {
            if ((_box.Position - point).LengthFast <= _box.Radius)
                return true;
            else
                return false;
        }

        public bool HasCollided(Sphere obj)
        {
            if ((_box.Position - obj.Position).LengthFast <= _box.Radius + obj.Radius)
                return true;
            else
                return false;
        }

        public bool HasCollided(RectangularPrism obj)
        {
            if (HasCollided(obj.Position))
            {
                return true;
            }
            else if
                (
                    HasCollided(Geometric.Quaternion_Rotate(Orientation, new Vector3(obj.Position.X + obj.Width / 2, 0, obj.Position.Z + obj.Width / 2)))
                    ||
                    HasCollided(Geometric.Quaternion_Rotate(Orientation, new Vector3(obj.Position.X - obj.Width / 2, 0, obj.Position.Z - obj.Width / 2)))
                    ||
                    HasCollided(Geometric.Quaternion_Rotate(Orientation, new Vector3(obj.Position.X + obj.Width / 2, 0, obj.Position.Z - obj.Width / 2)))
                    ||
                    HasCollided(Geometric.Quaternion_Rotate(Orientation, new Vector3(obj.Position.X - obj.Width / 2, 0, obj.Position.Z + obj.Width / 2)))
                )
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
