// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// Last Edited: 08-09-14

using System;
using Seven.Mathematics;
using OpenTK;

namespace SevenEngine.Physics.Primitives
{
  public class RectangularPrism
  {
    private float _width, _height;
    private Vector3 _position;
    private Quaternion _orientation;

    public Vector3 Position { get { return _position; } set { _position = value; } }
    public Quaternion Orientation { get { return _orientation; } set { _orientation = value; } }
    public float Width { get { return _width; } set { _width = value; } }
    public float Height { get { return _height; } set { _height = value; } }

    public RectangularPrism(float width, float height, float x, float y, float z)
    {
      _width = width;
      _height = height;
      _position = new Vector3(x, y, z);
    }

    public RectangularPrism(float width, float height, Vector3 position)
    {
      _width = width;
      _height = height;
      _position = position;
    }
  }
}
