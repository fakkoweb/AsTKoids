// Author(s):
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com
// Last Edited: 11-16-13

using OpenTK;
using Seven.Mathematics;

namespace SevenEngine.Physics.Primitives
{
  public class Sphere
  {
    Vector3 _position;
    float _radius;
    Quaternion _orientation;

    public Vector3 Position { get { return _position; } set { _position = value; } }
    public float Radius { get { return _radius; } set { _radius = value; } }
    public Quaternion Orientation { get { return _orientation; } set { _orientation = value; } }

    public Sphere(Vector3 position, float radius, Quaternion orientation)
    {
      _position = position;
      _radius = radius;
      _orientation = orientation;
    }

  }
}