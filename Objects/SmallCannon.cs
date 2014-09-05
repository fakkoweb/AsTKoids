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
        public override Vector3 BulletHole
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
        public override Vector3 BulletVector
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

        public SmallCannon(string id)
            : base(id, "Cannon_small_model", 20, 8000, 1000, new Vector3(0,0,0), new Vector3(0,0,0))
        {

            //TODO set bullet vectors properly
        }

        public void LookAt(Vector3 targetRef)
        {
          
            //Cannon must stay parallel to plane level
            Vector3 see_vector = targetRef - _mainModel.Position;
            _mainModel.Orientation = Geometric.FreeLookAt(_mainModel, targetRef, new Vector3(0, 1, 0));


            // OLD CODE!!
            /*
            //Transforming relative forward of cannon to world
            Vector3 forward = Geometric.Quaternion_Rotate(_mainModel.Orientation.Inverted(), new Vector3(0, 0, 1));
            //Vector3 forward = new Vector3(0, 0, 1);
            //Projecting world target to XZ plane and normalizing
            Vector3 target_project_base = new Vector3(targetRef.X, 0, targetRef.Z);
            target_project_base.Normalize();
            //Calculating angle between them (around y axis)
            float angle = (float)(Math.Atan2(target_project_base.Z, -target_project_base.X) - Math.Atan2(forward.Z, forward.X));//(float)Math.Acos( forward.DotProduct(target_project_base) );
            //Generating the transformation to bring forward to target (around y axis)
            Quaternion new_base_orientation;
            new_base_orientation = Geometric.Generate_Quaternion(angle, 0, 1, 0);
            //Applying transformation
            //_mainModel.OrientationRelative = Quaternion.Slerp(_mainModel.OrientationRelative, _mainModel.OrientationRelative * new_base_orientation, Game.DeltaTime * 0.01f);
            //Output.WriteLine("angolo: " + angle + " x: " + target_project_base.X + " y: " + target_project_base.Y + " z: " + target_project_base.Z);
            //Output.WriteLine("angolo:  + angle2 +  x: " + target_project_cannon.X + " y: " + target_project_cannon.Y + " z: " + target_project_cannon.Z);
            */

            /*
            //Transforming relative forward of cannon to world
            Vector3 forward2 = Geometric.Quaternion_Rotate(_cannonTubes.Orientation.Inverted(), new Vector3(0, 0, 1));
            //Vector3 forward = new Vector3(0, 0, 1);
            //Projecting world target to XZ plane and normalizing
            Vector3 target_project_cannon = new Vector3(0, targetRef.Y+0.01f, targetRef.Z);
            target_project_cannon.Normalize();
            //Calculating angle between them (around y axis)
            float angle2 = (float)(Math.Atan2(target_project_cannon.Z, -target_project_cannon.Y) - Math.Atan2(forward2.Z, forward2.Y));//(float)Math.Acos( forward.DotProduct(target_project_base) );
            //Generating the transformation to bring forward to target (around y axis)
            Quaternion new_cannon_orientation;
            new_cannon_orientation = Geometric.Generate_Quaternion(angle2, 1, 0, 0);
            //Applying transformation
            _mainModel.OrientationRelative = Quaternion.Slerp(_mainModel.OrientationRelative, _mainModel.OrientationRelative * new_cannon_orientation, Game.DeltaTime * 0.01f);
            Output.WriteLine("angolo2: " + angle2 + " x: " + target_project_cannon.X + " y: " + target_project_cannon.Y + " z: " + target_project_cannon.Z);
            */
            
 
            /*
            //Cannon tubes must orient toward target keeping is up-side to its y axis and rotating around its x axis
            new_base_orientation = Geometric.FreeLookAt(_mainModel, targetRef, new Vector3(0, 1, 0));
            _mainModel.OrientationRelative = Quaternion.Slerp(_mainModel.OrientationRelative, new_base_orientation, Game.DeltaTime * 0.001f);
            new_cannon_orientation = Geometric.FreeLookAt(_cannonTubes, targetRef, new Vector3(0, 1, 0));
            _cannonTubes.OrientationRelative = Quaternion.Slerp(_cannonTubes.OrientationRelative, new_cannon_orientation, Game.DeltaTime * 0.001f);
            */
            //Check if target is reachable within an angle constraint
            /*
            float angle = (float)(2 * Math.Acos(new_cannon_orientation.W));
            Output.WriteLine("Angle: " + angle);
            if (angle > 0 && angle < Constants.pi_float / 2)
            {

                _cannonTubes.OrientationRelative = Quaternion.Slerp(_cannonTubes.OrientationRelative, new_cannon_orientation, Game.DeltaTime * 0.001f);
                Output.WriteLine("New orientation loaded");
            }
            else
            {
                Output.WriteLine("NO orientation");
            }
             * */

            //Cannon base must orient toward target keeping is up-side to its y axis and rotating around its y axis
            
            
            
            
            
            //The whole system will then point towards the target.

            //TODO: look at for base and cannon

            //TODO: update bullethole and bullet vector
        }

    }
}
