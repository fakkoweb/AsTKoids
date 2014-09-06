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

        protected float _fireTime;
        protected float _fireSpeed;

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
                    _bulletHole = (Geometric.Quaternion_Rotate(_mainModel.Orientation.Inverted(), value)) - _mainModel.Position;
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
                    //result = Geometric.Quaternion_Rotate(_mainModel.OrientationRelative, _mainModel.Position + (Geometric.Quaternion_Rotate(_mainModel.Orientation, _bulletVector)));
                    result = Geometric.Quaternion_Rotate(_mainModel.Orientation, _bulletVector);
                    //result = _bulletVector;
                    return result;
                }
                else
                    return _bulletVector;
            }
            set
            {
                if (_mainModel.IsChild)
                {
                    _bulletVector = (Geometric.Quaternion_Rotate(_mainModel.Orientation, value)) - _mainModel.Position;
                }
                else
                    _bulletVector = value;
            }
        }
        public float FireSpeed { get { return _fireSpeed; } set { if (value <= 0) _fireSpeed = 0.1f; else  _fireSpeed = value; } }

        public Cannon(string id, StaticModel staticModel, float maxRotSpeed, float viewDistance, int health)
            : base(id, staticModel, maxRotSpeed, viewDistance, health)
        {
            _bulletHole = new Vector3(0, 0, 0);
            _bulletVector = new Vector3(0, 0, 0);
            _fireTime = 0;
            _fireSpeed = 1;
        }

        public Cannon(string id, string staticModel, float maxRotSpeed, float viewDistance, int health)
            : base(id, staticModel, maxRotSpeed, viewDistance, health)
        {
            _bulletHole = new Vector3(0, 0, 0);
            _bulletVector = new Vector3(0, 0, 0);
            _fireTime = 0;
            _fireSpeed = 1;
        }

        public Cannon(string id, StaticModel staticModel, float maxRotSpeed, float viewDistance, int health, Vector3 bulletHole, Vector3 bulletVector)
            : base(id, staticModel, maxRotSpeed, viewDistance, health)
        {
            _bulletHole = bulletHole;
            _bulletVector = bulletVector;
            _fireTime = 0;
            _fireSpeed = 1;
        }

        public Cannon(string id, string staticModel, float maxRotSpeed, float viewDistance, int health, Vector3 bulletHole, Vector3 bulletVector)
            : base(id, staticModel, maxRotSpeed, viewDistance, health)
        {
            _bulletHole = bulletHole;
            _bulletVector = bulletVector;
            _fireTime = 0;
            _fireSpeed = 1;
        }


        public void UpdateStandard()
        {
            //Each cannon has a delay between shoots
            _fireTime -= Game.DeltaTime * 0.001f;
        }

        public Bullet Shoot()
        {
            if (_fireTime <= 0)
            {
                //Output.WriteLine("Shoot!");
                Bullet _spawning = new Bullet(BulletHole, BulletVector);
                _fireTime = 1/_fireSpeed;    //two seconds for each new spawn
                return _spawning;
            }
            else
            {
                return null;
            }
            

        }

    }
}
