using Game.Objects.Types;
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
        protected SmallCannon[] _cannons;
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
        {/*
            _cannons = new SmallCannon[6];
            for (int i = 0; i < _cannons.Length; i++)
            {
                _cannons[i] = new SmallCannon(id + "_SmallCannon_" + i+1);
            }
            _coolerLeftModels = new StaticModel[3];
            _coolerRightModels = new StaticModel[3];
            _ringModels = new StaticModel[2];

            for (int i = 0; i < _coolerLeftModels.Length; i++)
            {
                _coolerLeftModels[i] = StaticModelManager.GetModel("Dreadnaught_cooler_model");
                _coolerRightModels[i] = StaticModelManager.GetModel("Dreadnaught_cooler_model");
                _coolerRightModels[i].Scale = new Vector<float>( - _coolerRightModels[i].Scale.X, _coolerRightModels[i].Scale.Y, _coolerRightModels[i].Scale.Z);
            }

            //TODO setup cannons and coolers relative positions
            _cannons[0].Position = new Vector<float>(0, 0, 0);
            _cannons[1].Position = new Vector<float>(0, 0, 0);
            _cannons[2].Position = new Vector<float>(0, 0, 0);
            _cannons[3].Position = new Vector<float>(0, 0, 0);
            _cannons[4].Position = new Vector<float>(0, 0, 0);
            _cannons[5].Position = new Vector<float>(0, 0, 0);

            _coolerLeftModels[0].Position = new Vector<float>(0, 0, 0);
            _coolerRightModels[0].Position = new Vector<float>(0, 0, 0);
            _coolerLeftModels[1].Position = new Vector<float>(0, 0, 0);
            _coolerRightModels[1].Position = new Vector<float>(0, 0, 0);
            _coolerLeftModels[2].Position = new Vector<float>(0, 0, 0);
            _coolerRightModels[2].Position = new Vector<float>(0, 0, 0);

            _ringModels[0] = StaticModelManager.GetModel("Dreadnaught_ringfront_model");
            _ringModels[1] = StaticModelManager.GetModel("Dreadnaught_ringback_model");
          * */
        }
    }
}
