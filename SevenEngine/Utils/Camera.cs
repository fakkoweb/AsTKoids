
using System;
using OpenTK;
using OpenTK.Input;
using Seven.Mathematics;

namespace SevenEngine
{
  /// <summary>Represents a camera to assist a game by generating a view matrix transformation.</summary>
  public class Camera
  {
    private static Vector3 yAxis = new Vector3(0, 1, 0);

    private float _fieldOfView;

    public float _nearClipPlane = 1f;
    public float _farClipPlane = 100000f;

    private float _positionSpeed;
    private float _lookSpeed;

    private Vector3 _position;
    private Vector3 _forward;
    private Vector3 _up;

    public float NearClipPlane { get { return _nearClipPlane; } set { _nearClipPlane = value; } }
    public float FarClipPlane { get { return _farClipPlane; } set { _farClipPlane = value; } }

    /// <summary>The field of view applied to the projection matrix during rendering transformations.</summary>
    public float FieldOfView { get { return _fieldOfView; } set { _fieldOfView = value; } }

    /// <summary>The speed at which the camera's position moves (camera movement sensitivity).</summary>
    public float PositionSpeed { get { return _positionSpeed; } set { _positionSpeed = value; } }
    public float LookSpeed { get { return _lookSpeed; } set { _lookSpeed = value; } }

    public Vector3 Position { get { return _position; } set { _position = value; } }
    public Vector3 Forward { get { return _forward; } set { _forward = value; } }
    public Vector3 Up { get { return _up; } set { _up = value; } }

    public Vector3 Backward { get { return -_forward; } }
    public Vector3 Right { get { return Vector3.Cross(_forward,_up).Normalized(); } }
    public Vector3 Left { get { return Vector3.Cross(_up,_forward).Normalized(); } }
    public Vector3 Down { get { return -_up; } }

    public Camera()
    {
      _position = new Vector3(0, 0, 0);
      _forward = new Vector3(0, 0, 1);
      _up = new Vector3(0, 1, 0);

      _fieldOfView = .5f;
    }

    public Camera(Vector3 pos, Vector3 forward, Vector3 up, float fieldOfView)
    {
      _position = pos;
      _forward = forward.Normalized();
      _up = up.Normalized();
      _fieldOfView = fieldOfView;
    }

    public void Move(Vector3 direction, float ammount)
    {
      _position = _position + (direction * ammount);
    }

    public void RotateY(float angle)
    {
      Vector3 Haxis = Vector3.Cross(yAxis,_forward.Normalized());
      _forward = Geometric.Quaternion_Rotate(Geometric.Generate_Quaternion(angle, 0, 1, 0),_forward).Normalized();
      _up = Vector3.Cross(_forward,Haxis.Normalized());
    }

    public void RotateX(float angle)
    {
      Vector3 Haxis = Vector3.Cross(yAxis, _forward.Normalized());
      _forward = Geometric.Quaternion_Rotate(Geometric.Generate_Quaternion(angle, Haxis.X, Haxis.Y, Haxis.Z), _forward).Normalized();
      _up = Vector3.Cross(_forward, Haxis.Normalized());
    }

    public Matrix4 GetMatrix()
    {
      Matrix4 camera = Matrix4.LookAt(
        _position.X, _position.Y, _position.Z,
        _position.X + _forward.X, _position.Y + _forward.Y, _position.Z + _forward.Z,
        _up.X, _up.Y, _up.Z);
      return camera;
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(
        (float)_fieldOfView,
        (float)Renderer.ScreenWidth / (float)Renderer.ScreenHeight,
        (float)_nearClipPlane,
        (float)_farClipPlane);
    }

  }
}