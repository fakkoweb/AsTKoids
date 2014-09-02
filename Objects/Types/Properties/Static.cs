using OpenTK;
using Seven.Mathematics;
using SevenEngine;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects.Types.Properties
{
    class Static
    {
        protected string _id;
        protected StaticModel _mainModel;

        public string Id { get { return _id; } set { _id = value; } }
        public StaticModel StaticModel { get { return _mainModel; } set { _mainModel = value; } }
        public Vector<float> Position { get { return _mainModel.Position; } set { _mainModel.Position = value; } }
        public Quaternion Orientation { get { return _mainModel.Orientation; } set { _mainModel.Orientation = value; } }

        public Static(string id, StaticModel staticModel)
        {
          _id = id;
          _mainModel = staticModel;
        }

        public Static(string id, string staticModel)
        {
            _id = id;
            _mainModel = StaticModelManager.GetModel(staticModel);
        }

        public static int CompareTo(Static left, Static right) { return left.Id.CompareTo(right.Id); }
        public static int CompareTo(Static left, string right) { return left.Id.CompareTo(right); }

    }
}
