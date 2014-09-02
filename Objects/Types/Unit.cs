using Game.Objects.Types.Properties;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects.Types
{
    class Unit : Movable, Damageable
    {
        protected int _health;
        protected int _damage;
        protected bool _isDead;

        public int Health { get { return _health; } set { _health = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public virtual bool IsDead { get { return _isDead; } set { _isDead = value; } }
    
        public Unit(string id, StaticModel staticModel, float maxRotSpeed, float viewDistance, float maxSpeed, float maxAcceleration, int health)
            : base(id, staticModel, maxRotSpeed, viewDistance, maxSpeed, maxAcceleration)
        {
            _damage = 0;
            _health = health;
            _isDead = false;
        }

        public Unit(string id, string staticModel, float maxRotSpeed, float viewDistance, float maxSpeed, float maxAcceleration, int health)
            : base(id, staticModel, maxRotSpeed, viewDistance, maxSpeed, maxAcceleration)
        {
            _damage = 0;
            _health = health;
            _isDead = false;
        }
    
    
    }
}
