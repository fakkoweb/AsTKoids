using Game.Objects.Types;
using OpenTK;
using Seven.Mathematics;
using SevenEngine;
using SevenEngine.Physics.Primitives;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects
{
    class Dreadnaught : Unit
    {
        protected RectangularPrism _box;

        protected StaticModel _backModel;
        protected SmallCannon[] _frontCannons;
        protected SmallCannon[] _backCannons;
        protected StaticModel[] _coolerLeftModels;
        protected StaticModel[] _coolerRightModels;
        protected StaticModel[] _ringModels;
        float _ringAnimateAngle;
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
                _box.Width = _box.Width*_mainModel.Scale.X; 
                _box.Height = _box.Height*_mainModel.Scale.Z;
                foreach (StaticModel childModel in _mainModel.ChildrenModels)
                {
                    childModel.Scale = new Vector3(value.X, value.Y, value.Z);
                }
                foreach (StaticModel childModel in _backModel.ChildrenModels)
                {
                    childModel.Scale = new Vector3(value.X, value.Y, value.Z);
                }
                foreach (SmallCannon cannon in _frontCannons)
                {
                    cannon.Scale = cannon.Scale * 2;
                }
                /*foreach (SmallCannon cannon in _backCannons)
                {
                    cannon.Scale = cannon.Scale * 2;
                }*/
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
        public RectangularPrism BoundingBox { get { return _box; } set { _box = value; } }

        public Dreadnaught(string id)
            : base(id, "Dreadnaught_front_model", 0.5f, 10000, 4, 0.01f, 100000)
        {
            //Collider
            _box = new RectangularPrism(25, 94, Position);

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
            //_ringModels[1].Scale = new Vector3(1, -1, 1);
            _ringModels[1].setParent(_backModel);
            _ringAnimateAngle = 0;

            //Cannons
            _frontCannons = new SmallCannon[5];
            for (int i = 0; i < _frontCannons.Length; i++)
            {
                _frontCannons[i] = new SmallCannon(id + "_SmallCannon_" + (i+1) );
                _frontCannons[i].StaticModel.Id = id + "_SmallCannon_" + (i+1) ;
                _frontCannons[i].FireSpeed = 3;
            }

            float offset = 1f;
            //Upper Front cannons positions
            _frontCannons[0].StaticModel.PositionRelative = new Vector3(5f,2.467f+offset,40.251f);
            _frontCannons[0].StaticModel.setParent(_mainModel);
            _frontCannons[1].StaticModel.PositionRelative = new Vector3(-5f, 2.467f+offset, 40.251f);
            _frontCannons[1].StaticModel.setParent(_mainModel);
            _frontCannons[2].StaticModel.PositionRelative = new Vector3(6.793f, 3.102f+offset, 24.521f);
            _frontCannons[2].StaticModel.setParent(_mainModel);
            _frontCannons[3].StaticModel.PositionRelative = new Vector3(-6.793f, 3.102f+offset, 24.521f);
            _frontCannons[3].StaticModel.setParent(_mainModel);
            //Lower Front cannons positions
            _frontCannons[4].StaticModel.PositionRelative = new Vector3(0, -6.162f-offset, 24.521f);
            _frontCannons[4].StaticModel.OrientationRelative = Geometric.Generate_Quaternion(Constants.pi_float, 0, 0, 1);
            _frontCannons[4].StaticModel.setParent(_mainModel);
            /*
            _frontCannons[0].StaticModel.PositionRelative = new Vector3(5f, 2.467f, 40.251f);
            _frontCannons[0].StaticModel.setParent(_backModel);
            _frontCannons[0].StaticModel.PositionRelative = new Vector3(5f, 2.467f, 40.251f);
            _frontCannons[0].StaticModel.setParent(_backModel);
            
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
            foreach (SmallCannon cannon in _frontCannons)
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
            Quaternion new_orientation = Geometric.FreeLookAt(_mainModel,targetRef, new Vector3(0, 1, 0));
            OrientationRelative = Quaternion.Slerp(_mainModel.OrientationRelative, new_orientation, Game.DeltaTime * 0.001f * _maxRotSpeed);

            //Output.WriteLine(_frontCannons[0].StaticModel.PositionRelative + ", " + _frontCannons[1].StaticModel.PositionRelative);
        }

        public void AimAt(Vector3 targetRef)
        {
            foreach (SmallCannon cannon in _frontCannons)
            {
                cannon.LookAt(targetRef);
            }
        }

        
        public List<Bullet> Shoot(Vector3 targetRef)        //target is used to determine which cannon will fire!
        {
            List<Bullet> bulletsShot = new List<Bullet>();
            foreach (SmallCannon cannon in _frontCannons)
            {
                Bullet newBullet = cannon.Shoot();
                if (newBullet != null) bulletsShot.Add(newBullet);
            }
            return bulletsShot;
        }
        
    
    }
}
