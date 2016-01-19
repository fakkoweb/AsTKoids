// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14

using AsTKoids.Objects.Types;
using OpenTK;
using SevenEngine;
using SevenEngine.Imaging;
using SevenEngine.Physics.Primitives;
using SevenEngine.StaticModels;
using Seven.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsTKoids.Objects
{
    class Dreadnaught : Unit
    {
        //Sphere does not need to be transformed and seems to behave better than RectangularPrism, change if you prefer more precise Collision Detection
        //Warning: remember to save also a copy of Prism dimensions and set them in the SET for Scale
        protected Sphere _box;
        protected float _boxRadius;

        protected StaticModel _backModel;
        protected SmallCannon[] _cannons;
        protected SmallCannon[] _backCannons;
        protected StaticModel[] _coolerLeftModels;
        protected StaticModel[] _coolerRightModels;
        protected StaticModel[] _ringModels;
        float _ringAnimateAngle = 0;
        Vector3 _last_target_position = new Vector3(0, 0, 0);
        Quaternion _last_target_orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
        float _stopRotationFactor = 1;
/*
        protected AvlTree<StaticModel> _cannonModels;
        protected AvlTree<StaticModel> _coolerModels;
        protected AvlTree<StaticModel> _ringModels;
*/
        public override Vector3 Position { get { return _mainModel.Position; } set { _mainModel.Position = value; _box.Position = _mainModel.PositionRelative; } }
        public override Vector3 PositionRelative { get { return _mainModel.PositionRelative; } set { _mainModel.PositionRelative = value; _box.Position = _mainModel.PositionRelative; } }
        public override Vector3 Scale
        { 
            get 
            { 
                return _mainModel.Scale;
            }
            set
            { 
                _mainModel.Scale = value;
                // Change collider scale accordingly
                //_box.Width = _box.Width*_mainModel.Scale.X; 
                //_box.Height = _box.Height*_mainModel.Scale.Z;
                _box.Radius = _boxRadius * _mainModel.Scale.Z;
                foreach (StaticModel childModel in _mainModel.ChildrenModels)
                {
                    childModel.Scale = new Vector3(value.X, value.Y, value.Z);
                }
                foreach (StaticModel childModel in _backModel.ChildrenModels)
                {
                    childModel.Scale = new Vector3(value.X, value.Y, value.Z);
                }
                foreach (SmallCannon cannon in _cannons)
                {
                    cannon.Scale = cannon.Scale * 2;
                }

            } 
        }
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
        public Sphere BoundingBox { get { return _box; } set { _box = value; } }
        public SmallCannon[] Cannons { get { return _cannons; } }

        public Dreadnaught(string id)
            : base(id, "Dreadnaught_front_model", 0.87f, 10000, 4, 0.01f, 100000)
        {
            //Collider
            _boxRadius = 24.5f;
            //_box = new RectangularPrism(25, 94, Position);
            _box = new Sphere(Position, _boxRadius, new Quaternion(0, 0, 0, 1));

            //Back side
            _backModel = StaticModelManager.GetModel("Dreadnaught_back_model");
            _backModel.setParent(_mainModel);

            //Rings
            _ringModels = new StaticModel[2];
            _ringModels[0] = StaticModelManager.GetModel("Dreadnaught_ring_model");
            _ringModels[0].Position = new Vector3(0, -0.60810f, 0);     //this tilt is a fix
            _ringModels[0].setParent(_mainModel);
            _ringModels[1] = StaticModelManager.GetModel("Dreadnaught_ring_model");
            _ringModels[1].Position = new Vector3(0, -0.60810f, -3.0701f);     //this tilt is a fix
            _ringModels[1].setParent(_backModel);

            //Cannons
            _cannons = new SmallCannon[7];
            for (int i = 0; i < _cannons.Length; i++)
            {
                _cannons[i] = new SmallCannon(id + "_SmallCannon_" + (i+1) );
                _cannons[i].StaticModel.Id = id + "_SmallCannon_" + (i+1) ;
                _cannons[i].FireSpeed = 3;
            }

            float offset = 0.5f;
            //Upper Front cannons positions
            _cannons[0].StaticModel.PositionRelative = new Vector3(5f,2.467f+offset,40.251f);
            _cannons[0].StaticModel.setParent(_mainModel);
            _cannons[1].StaticModel.PositionRelative = new Vector3(-5f, 2.467f+offset, 40.251f);
            _cannons[1].StaticModel.setParent(_mainModel);
            _cannons[2].StaticModel.PositionRelative = new Vector3(6.793f, 3.102f+offset, 24.521f);
            _cannons[2].StaticModel.setParent(_mainModel);
            _cannons[3].StaticModel.PositionRelative = new Vector3(-6.793f, 3.102f+offset, 24.521f);
            _cannons[3].StaticModel.setParent(_mainModel);
            //Lower Front cannons positions
            _cannons[4].StaticModel.PositionRelative = new Vector3(0, -6.162f-offset, 24.521f);
            _cannons[4].StaticModel.OrientationRelative = Geometric.Generate_Quaternion(Constants.pi_float, 0, 0, 1);
            _cannons[4].StaticModel.setParent(_mainModel);
            //Lower Back cannons positions
            _cannons[5].StaticModel.PositionRelative = new Vector3(5.93f, -7.01f-offset, -22f);
            _cannons[5].StaticModel.OrientationRelative = Geometric.Generate_Quaternion(Constants.pi_float, 0, 0, 1);
            _cannons[5].StaticModel.setParent(_backModel);
            _cannons[6].StaticModel.PositionRelative = new Vector3(-5.93f, -7.01f-offset, -22f);
            _cannons[6].StaticModel.OrientationRelative = Geometric.Generate_Quaternion(Constants.pi_float, 0, 0, 1);
            _cannons[6].StaticModel.setParent(_backModel);
            
            /*
            _coolerLeftModels = new StaticModel[3];
            _coolerRightModels = new StaticModel[3];
            _ringModels = new StaticModel[2];

            for (int i = 0; i < _coolerLeftModels.Length; i++)
            {
                _coolerLeftModels[i] = StaticModelManager.GetModel("Dreadnaught_cooler_model");
                _coolerRightModels[i] = StaticModelManager.GetModel("Dreadnaught_cooler_model");
                _coolerRightModels[i].Scale = new Vector3( - _coolerRightModels[i].Scale.X, _coolerRightModels[i].Scale.Y, _coolerRightModels[i].Scale.Z);
            }

            
            //TODO setup cannons and coolers relative positions
            _frontCannons[0].Position = new Vector3(0, 0, 0);
            _frontCannons[1].Position = new Vector3(0, 0, 0);
            _frontCannons[2].Position = new Vector3(0, 0, 0);
            _frontCannons[3].Position = new Vector3(0, 0, 0);
            _frontCannons[4].Position = new Vector3(0, 0, 0);
            _frontCannons[5].Position = new Vector3(0, 0, 0);

            _coolerLeftModels[0].Position = new Vector3(0, 0, 0);
            _coolerRightModels[0].Position = new Vector3(0, 0, 0);
            _coolerLeftModels[1].Position = new Vector3(0, 0, 0);
            _coolerRightModels[1].Position = new Vector3(0, 0, 0);
            _coolerLeftModels[2].Position = new Vector3(0, 0, 0);
            _coolerRightModels[2].Position = new Vector3(0, 0, 0);

            _ringModels[0] = StaticModelManager.GetModel("Dreadnaught_ringfront_model");
            _ringModels[1] = StaticModelManager.GetModel("Dreadnaught_ringback_model");
            */
        }

        public void UpdateStandard()
        {
            foreach (SmallCannon cannon in _cannons)
            {
                cannon.UpdateStandard();
            }

            _ringAnimateAngle += 0.0005f;
            if (_ringAnimateAngle > 2 * Constants.pi_float) _ringAnimateAngle = 0;
            _ringModels[0].OrientationRelative = Geometric.Generate_Quaternion(_ringAnimateAngle, 0, 0, 1);
            _ringModels[1].OrientationRelative = Geometric.Generate_Quaternion(_ringAnimateAngle, 0, 0, -1);
        }

        public void LookAt(Vector3 targetRef)
        {
            Quaternion new_target_orientation = Geometric.FreeLookAt(_mainModel,targetRef, new Vector3(0, 1, 0));
            Orientation = Quaternion.Slerp(_mainModel.Orientation, new_target_orientation, Game.DeltaTime * 0.001f * _maxRotSpeed);
            _last_target_orientation = new_target_orientation;
            //Output.WriteLine(_frontCannons[0].StaticModel.PositionRelative + ", " + _frontCannons[1].StaticModel.PositionRelative);
        }

        public void AlignTo(Vector3 targetRef)
        {
            if (targetRef != _last_target_position)
            {
                _last_target_position = targetRef;
                _last_target_orientation = Geometric.FreeLookAt(_mainModel, _last_target_position, new Vector3(0, 1, 0));
            }
            Orientation = Quaternion.Slerp(_mainModel.Orientation, _last_target_orientation, Game.DeltaTime * 0.001f * _maxRotSpeed);
        }

        public void AimAt(Vector3 targetRef)
        {
            foreach (SmallCannon cannon in _cannons)
            {
                cannon.LookAt(targetRef);
            }
        }
        
        public List<Bullet> Shoot(Vector3 targetRef)        //target is used to determine which cannon will fire!
        {
            List<Bullet> bulletsShot = new List<Bullet>();
            List<SmallCannon> cannonsToShoot = new List<SmallCannon>();

            //Transforming target coordinates in dreadnaught system
            Vector3 targetRelative = Geometric.Quaternion_Rotate(Orientation.Inverted(), targetRef - Position);
            //Defining fire zones (relative to the ship's system)
            if (targetRelative.Z > -80 * Scale.Z)    //this value is chosen knowing that 95 is approx. ship lenght for scale = 1 (measured in blender)
            {
                cannonsToShoot.Add(_cannons[0]);
                cannonsToShoot.Add(_cannons[1]);
                cannonsToShoot.Add(_cannons[2]);
                cannonsToShoot.Add(_cannons[3]);
                cannonsToShoot.Add(_cannons[4]);
            }
            if (targetRelative.Z < 80 * Scale.Z)
            {
                cannonsToShoot.Add(_cannons[5]);
                cannonsToShoot.Add(_cannons[6]);
            }
            foreach (SmallCannon cannon in cannonsToShoot)
            {
                Bullet newBullet = cannon.Shoot();
                if (newBullet != null) bulletsShot.Add(newBullet);
            }
            return bulletsShot;
        }
        
    
    }
}
