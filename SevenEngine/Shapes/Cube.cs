// Author(s):
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com
// Last Edited: 11-16-13

using OpenTK;
using Seven.Mathematics;

namespace SevenEngine.Physics.Primitives
{
  public class Cube
  {
    Vector3 _position;
    float _scale;
    Quaternion _orientation;

    public Vector3 Position { get { return _position; } set { _position = value; } }
    public float Scale { get { return _scale; } set { _scale = value; } }
    public Quaternion Orientation { get { return _orientation; } set { _orientation = value; } }

    public Cube(Vector3 position, float scale, Quaternion orientation)
    {
      _position = position;
      _scale = scale;
      _orientation = orientation;
    }
  }
}
