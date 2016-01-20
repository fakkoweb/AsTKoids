// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14


using OpenTK;
using SevenEngine;
using SevenEngine.StaticModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsTKoids.Objects.Types.Behaviours
{
    public class Static
    {
        protected string _id;
        protected StaticModel _mainModel;

        public string Id { get { return _id; } set { _id = value; } }
        public StaticModel StaticModel { get { return _mainModel; } set { _mainModel = value; } }
        public virtual Vector3 Position { get { return _mainModel.Position; } set { _mainModel.Position = value; } }
        public virtual Quaternion Orientation { get { return _mainModel.Orientation; } set { _mainModel.Orientation = value; } }
        public virtual Vector3 PositionRelative { get { return _mainModel.PositionRelative; } set { _mainModel.PositionRelative = value; } }
        public virtual Quaternion OrientationRelative { get { return _mainModel.OrientationRelative; } set { _mainModel.OrientationRelative = value; } }
        public virtual Vector3 Scale { get { return _mainModel.Scale; } set { _mainModel.Scale = value; } }

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
