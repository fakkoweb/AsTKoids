using Game.Objects.Types;
using Game.Objects.Types.Properties;
using OpenTK;
using Seven.Mathematics;
using SevenEngine;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects
{
    class Cannon : DynamicSubsystem, Weapon
    {


        protected Vector3 _bulletHole;
        protected Vector3 _bulletVector;

        public virtual Vector3 BulletHole
        {         
            get
            {// TODO
                Vector3 result;
                if (_mainModel.IsChild)
                {
                    result = _mainModel.Position + (Geometric.Quaternion_Rotate(_mainModel.Orientation, _bulletHole));
                    return result;
                }
                else
                    return _bulletHole;
            }
            set 
            {
                if (_mainModel.IsChild)
                {
                    _bulletHole = (Geometric.Quaternion_Rotate(_mainModel.Orientation, value)) - _mainModel.Position;
                }
                else
                    _bulletHole = value;
            }
        }
        public virtual Vector3 BulletVector
        {
            get
            {// TODO
                Vector3 result;
                if (_mainModel.IsChild)
                {
                    result = Geometric.Quaternion_Rotate(_mainModel.Orientation, _bulletVector);
                    return result;
                }
                else
                    return _bulletVector;
            }
            set
            {
                if (_mainModel.IsChild)
                {
                    _bulletHole = (Geometric.Quaternion_Rotate(_mainModel.Orientation, value)) - _mainModel.Position;
                }
                else
                    _bulletHole = value;
            }
        }

        public Cannon(string id, StaticModel staticModel, float maxRotSpeed, float viewDistance, int health)
            : base(id, staticModel, maxRotSpeed, viewDistance, health)
        {
            _bulletHole = new Vector3(0, 0, 0);
            _bulletVector = new Vector3(0, 0, 0);
        }

        public Cannon(string id, string staticModel, float maxRotSpeed, float viewDistance, int health)
            : base(id, staticModel, maxRotSpeed, viewDistance, health)
        {
            _bulletHole = new Vector3(0, 0, 0);
            _bulletVector = new Vector3(0, 0, 0);
        }

        public Cannon(string id, StaticModel staticModel, float maxRotSpeed, float viewDistance, int health, Vector3 bulletHole, Vector3 bulletVector)
            : base(id, staticModel, maxRotSpeed, viewDistance, health)
        {
            _bulletHole = bulletHole;
            _bulletVector = bulletVector;
        }

        public Cannon(string id, string staticModel, float maxRotSpeed, float viewDistance, int health, Vector3 bulletHole, Vector3 bulletVector)
            : base(id, staticModel, maxRotSpeed, viewDistance, health)
        {
            _bulletHole = bulletHole;
            _bulletVector = bulletVector;
        }

    }
}
