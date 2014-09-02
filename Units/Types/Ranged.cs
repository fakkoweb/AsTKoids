﻿using System;
using SevenEngine;
using SevenEngine.Imaging;
using Game.States;
using Seven.Structures;
using SevenEngine.StaticModels;
using Seven.Mathematics;

namespace Game.Units
{
  public abstract class Ranged : Unit
  {
    private const int _healthMin = 50;
    private const int _healthMax = 100;
    private const int _damageMin = 3;
    private const int _damageMax = 7;
    private const int _viewDistanceMin = 1;
    private const int _viewDistanceMax = 10000;
    private const int _attackRangeMin = 400;
    private const int _attackRangeMax = 550;
    private const int _moveSpeedMin = 30;
    private const int _moveSpeedMax = 40;

    public Ranged(string id, StaticModel staticModel) : base(id, staticModel)
    {
      Random random = new Random();
      _health = random.Next(_healthMin, _healthMax);
      _damage = random.Next(_damageMin, _damageMax);
      _viewDistance = random.Next(_viewDistanceMin, _viewDistanceMax);
      _attackRange = random.Next(_attackRangeMin, _attackRangeMax);
      _moveSpeed = random.Next(_moveSpeedMin, _moveSpeedMax) / 20f;
      _attackRangedSquared = _attackRange * _attackRange;
    }

    protected bool Attack(Unit defending)
    {
      defending.Health -= _damage;
      if (defending.Health <= 0)
      {
        defending.IsDead = true;
        return true;
      }
      return false;
    }
  }
}
