﻿// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14

using AsTKoids.Objects.Types.Behaviours;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsTKoids.Objects.Types
{
    public class StaticSubsystem : Static, Damageable
    {
        protected int _health;
        protected int _damage;
        protected bool _isDead;

        public int Health { get { return _health; } set { _health = value; } }
        public int Damage { get { return _damage; } set { _damage = value; } }
        public virtual bool IsDead { get { return _isDead; } set { _isDead = value; } }
        
        public StaticSubsystem(string id, StaticModel staticModel, int health)
            : base(id, staticModel)
        {
            _damage = 0;
            _health = health;
            _isDead = false;
        }

        public StaticSubsystem(string id, string staticModel, int health)
            : base(id, staticModel)
        {
            _damage = 0;
            _health = health;
            _isDead = false;
        }

    }
}
