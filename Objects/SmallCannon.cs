using Seven.Mathematics;
using SevenEngine;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Game.Objects
{
    class SmallCannon : Cannon
    {
        StaticModel _cannonTubes;

        //Must transform bullet origin with _cannonTubes transform also!
        public override Vector<float> BulletHole
        {
            get
            {// TODO
                Vector<float> result;
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
        public override Vector<float> BulletVector
        {
            get
            {// TODO
                Vector<float> result;
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

        public SmallCannon(string id)
            : base(id, "Cannon_small_base_model", 20, 8000, 1000, new Vector<float>(0,0,0), new Vector<float>(0,0,0))
        {
            _cannonTubes = StaticModelManager.GetModel("Cannon_small_model");
            _cannonTubes.Position = new Vector<float>(0, 0, 0);
            //_cannon.StaticModel.Orientation = new Quaternion(0, 1, 0, 0);
            _cannonTubes.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _cannonTubes.Scale = new Vector<float>(20, 20, 20);
            _cannonTubes.setParent(_mainModel);
            //TODO set bullet vectors properly
        }

        public void LookAt(Vector<float> targetRef)
        {


 	         ConstraintLookAt(targetRef, new Vector<float>(0,1,0), new Vector<float>(0,1,0));
            
            //TODO: look at for base and cannon

            //TODO: update bullethole and bullet vector
        }
    }
}
