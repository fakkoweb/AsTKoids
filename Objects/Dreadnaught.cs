using Game.Objects.Types;
using OpenTK;
using Seven.Mathematics;
using Seven.Structures;
using SevenEngine;
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
        protected StaticModel _backModel;
        protected SmallCannon[] _frontCannons;
        protected SmallCannon[] _backCannons;
        protected StaticModel[] _coolerLeftModels;
        protected StaticModel[] _coolerRightModels;
        protected StaticModel[] _ringModels;
/*
        protected AvlTree<StaticModel> _cannonModels;
        protected AvlTree<StaticModel> _coolerModels;
        protected AvlTree<StaticModel> _ringModels;
*/
        public Dreadnaught(string id)
            : base(id, "Dreadnaught_front_model", 3, 10000, 5, 1, 100000)
        {
            _frontCannons = new SmallCannon[5];
            for (int i = 0; i < _frontCannons.Length; i++)
            {
                _frontCannons[i] = new SmallCannon(id + "_SmallCannon_" + (i+1) );
                _frontCannons[i].StaticModel.Id = id + "_SmallCannon_" + (i+1) ;
            }

            //Upper Front cannons
            _frontCannons[0].StaticModel.PositionRelative = new Vector3(5f,2.467f,40.251f);
            _frontCannons[0].StaticModel.setParent(_mainModel);
            _frontCannons[1].StaticModel.PositionRelative = new Vector3(-5f, 2.467f, 40.251f);
            _frontCannons[1].StaticModel.setParent(_mainModel);
            _frontCannons[2].StaticModel.PositionRelative = new Vector3(6.793f, 3.102f, 24.521f);
            _frontCannons[2].StaticModel.setParent(_mainModel);
            _frontCannons[3].StaticModel.PositionRelative = new Vector3(-6.793f, 3.102f, 24.521f);
            _frontCannons[3].StaticModel.setParent(_mainModel);
            //Lower Front cannons
            _frontCannons[4].StaticModel.PositionRelative = new Vector3(0, -6.162f, 24.521f);
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

        public void LookAt(Vector3 targetRef)
        {
            Quaternion new_orientation = Geometric.FreeLookAt(_mainModel,targetRef, new Vector3(0, 1, 0));
            _mainModel.OrientationRelative = Quaternion.Slerp(_mainModel.OrientationRelative, new_orientation, Game.DeltaTime * 0.001f);

            //Output.WriteLine(_frontCannons[0].StaticModel.PositionRelative + ", " + _frontCannons[1].StaticModel.PositionRelative);
        }

        public void AimAt(Vector3 targetRef)
        {
            foreach (SmallCannon cannon in _frontCannons)
            {
                cannon.LookAt(targetRef);
            }
        }
    
    }
}
