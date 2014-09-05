using System;

using Game.States;
using SevenEngine.Imaging;
using Seven.Mathematics;
using Seven.Structures;
using SevenEngine.StaticModels;

namespace Game.Units
{
  public class KillemRanged : Ranged
  {
    Unit _target;
    float _time = 0;
    float _delay = 0;
    int move;
    bool attack;

    public KillemRanged(string id, StaticModel staticModel) : base(id, staticModel) { _time = 0; if (AiBattle._map == 0) _delay = 4000;}

    public override void AI(float elapsedTime, Omnitree<Unit, double> octree)
    {
      if (_time <= _delay)
        _time += elapsedTime;
      if (IsDead == false)
      {
        // Targeting
        if (_target == null || _target.IsDead || move > 20)
        {
          attack = false;
          move = 0;
          float shortest = float.MaxValue;
          octree.Foreach
          (
            (Unit current) =>
            {
              if (current is ZackKamakazi && !current.IsDead)
              {
                if (_target == null || _target.IsDead || !(_target is KillemKamakazi))
                {
                  _target = current;
                  shortest = (current.Position - Position).LengthSquared();
                }
                else
                {
                  float length = (current.Position - Position).LengthSquared();
                  if (length < shortest)
                  {
                    _target = current;
                    shortest = length;
                  }
                }
              }
              else if ((current is ZackMelee || current is ZackRanged) && !current.IsDead)
              {
                if (_target == null || _target.IsDead)
                {
                  _target = current;
                  shortest = (current.Position - Position).Length();
                }
                else
                {
                  float length = (current.Position - Position).Length();
                  if (length < shortest)
                  {
                    _target = current;
                    shortest = length;
                  }
                }
              }
            }
          );
        }
        // Attacking
        else if (Logic.Abs((Position - _target.Position).LengthSquared()) < _attackRangedSquared)
        {
          Link<Vector3, Vector3, Color> link =
            new Link<Vector3, Vector3, Color>(
              new Vector3(Position.X, Position.Y, Position.Z),
              new Vector3(_target.Position.X, _target.Position.Y, _target.Position.Z),
              Color.Red);

          if (!attack && !AiBattle.lines.Contains(link, AiBattle.Compare))
            AiBattle.lines.Add(link);

          if (Attack(_target))
            _target = null;
          move = 0;
          attack = !attack;
        }
        // Moving
        else if (_time > _delay)
        {
          Position += (_target.Position - Position).Normalize() * MoveSpeed;
          move++;
          attack = false;
        }
        this.StaticModel.Orientation.W+=.1f;
      }
    }
  }
}