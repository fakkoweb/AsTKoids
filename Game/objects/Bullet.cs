// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14

using AsTKoids.Objects.Types.Behaviours;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTKoids.Objects
{
    public class Bullet : Damageable
    {
        protected int _health;
        protected int _damage;
        protected bool _isDead;

        public int Health { get { return _health; } set { _health = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public virtual bool IsDead { get { return _isDead; } set { _isDead = value; } }

        protected Vector3 _bulletPosition;
        protected Vector3 _bulletVector;

        public Vector3 Position { get { return _bulletPosition; } set { _bulletPosition = value; } }
        public Vector3 Velocity { get { return _bulletVector; } set { _bulletVector = value; } }

        public Bullet(Vector3 pos, Vector3 vec)
        {
            _bulletPosition = pos;
            _bulletVector = vec;
            _isDead = false;
        }

        public void Move()
        {
            Position = new Vector3(Position.X + Velocity.X, Position.Y + Velocity.Y, Position.Z + Velocity.Z);
        }

    }
}
