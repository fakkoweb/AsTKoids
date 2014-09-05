﻿//using System;

using Seven;

using SevenEngine;
using Seven.Structures;
using SevenEngine.Imaging;
using SevenEngine.StaticModels;
using Seven.Mathematics;

namespace Game.States
{
  public class GameState : InterfaceGameState
  {
    private string _id;
    private bool _isReady;

    public string Id { get { return _id; } set { _id = value; } }
    public bool IsReady { get { return _isReady; } }

    #region State Fields

    public static Comparison Compare(double left, double right)
    {
      int comparison = left.CompareTo(right);
      if (comparison > 0)
        return Comparison.Greater;
      else if (comparison < 0)
        return Comparison.Less;
      else
        return Comparison.Equal;
    }

    public Omnitree<StaticModel, double> _octree = 
      new Omnitree_Array<StaticModel, double>(
        new double[] { -100000, -100000, -100000 },
        new double[] { 100000, 100000, 100000 },
        (StaticModel i, out double[] location) => { location = new double[] { i.Position.X, i.Position.Y, i.Position.Z }; },
        Compare,
        (double left, double right) => { return (left + right) * .5d; });
        

    Camera _camera;
    StaticModel _terrain;
    StaticModel _mountain;
    StaticModel _mountain2;
    StaticModel[] _rangers;
    StaticModel[] _tuxes;
    StaticModel _mushroomCloud;
    float _time;
    bool _bool;
    SkyBox _skybox;

    StaticModel _dodeca;

    #endregion

    public GameState(string id)
    {
      _id = id;
      _isReady = false;
    }

    #region Loading

    public void Load()
    {
      _camera = new Camera();
      _camera.PositionSpeed = 5;
      _camera.Move(_camera.Up, 400);
      _camera.Move(_camera.Backward, 1500);
      _camera.Move(_camera.Backward, 300);

      _skybox = new SkyBox();
      _skybox.Scale.X = 10000;
      _skybox.Scale.Y = 10000;
      _skybox.Scale.Z = 10000;
      _skybox.Left = TextureManager.Get("SkyboxLeft");
      _skybox.Right = TextureManager.Get("SkyboxRight");
      _skybox.Front = TextureManager.Get("SkyboxFront");
      _skybox.Back = TextureManager.Get("SkyboxBack");
      _skybox.Top = TextureManager.Get("SkyboxTop");

      _terrain = StaticModelManager.GetModel("Terrain");
      _terrain.Scale = new Vector3(500, 20, 500);
      _terrain.Orientation = new Quaternion<float>(0, 0, 0, 0);
      _terrain.Position = new Vector3(0, 0, 0);

      _mushroomCloud = StaticModelManager.GetModel("MushroomCloud");
      _mushroomCloud.Scale = new Vector3(500, 20, 500);
      _mushroomCloud.Orientation = new Quaternion<float>(0, 0, 0, 0);
      _mushroomCloud.Position.X = 0;
      _mushroomCloud.Position.Y = _terrain.Position.Y + 30;
      _mushroomCloud.Position.Z = 0;
      _time = 0;
      _bool = false;

      _mountain = StaticModelManager.GetModel("Mountain");
      _mountain.Scale = new Vector3(5000, 5000, 5000);
      _mountain.Orientation = new Quaternion<float>(0, 0, 0, 0);
      _mountain.Position = new Vector3(4000, 0, 1000);

      _mountain2 = StaticModelManager.GetModel("Mountain2");
      _mountain2.Scale = new Vector3(3500, 3500, 3500);
      _mountain2.Orientation = new Quaternion<float>(0, 0, 0, 0);
      _mountain2.Position = new Vector3(0, 0, 2500);

      string[] colors = new string[] { "YellowRanger", "RedRanger", "BlueRanger", "BlackRanger", "PinkRanger" };

      System.Random random = new System.Random();
      _rangers = new StaticModel[80];
      for (int i = 0; i < _rangers.Length; i++)
      {
        _rangers[i] = StaticModelManager.GetModel(colors[random.Next(0, 5)]);
        _rangers[i].Position.X = -100;
        _rangers[i].Position.Y = _terrain.Position.Y + 10;
        _rangers[i].Position.Z = -i * 50;
        _rangers[i].Scale = new Vector3(5, 5, 5);
        _rangers[i].Orientation = new Quaternion<float>(0, 1, 0, 0);
        _rangers[i].Orientation.W = i * 2;
        _rangers[i].Id = "Ranger" + i;
        _octree.Add(_rangers[i]);
      }

      _tuxes = new StaticModel[80];
      for (int i = 0; i < _tuxes.Length; i++)
      {
        _tuxes[i] = StaticModelManager.GetModel("Tux");
        _tuxes[i].Position.X = 100;
        _tuxes[i].Position.Y = _terrain.Position.Y + 10;
        _tuxes[i].Position.Z = i * 50;
        _tuxes[i].Scale = new Vector3(25, 25, 25);
        _tuxes[i].Orientation = new Quaternion<float>(0, 1, 0, 0);
        _tuxes[i].Orientation.W = i * 2;
        _tuxes[i].Id = "Tux" + i;
        _octree.Add(_tuxes[i]);
      }

      //for (int i = 0; i < _rangers.Length; i += 2)
      //{
      //  _rangers[i].Meshes.Remove<string>("Body", (StaticMesh mesh, string key) => { return mesh.Id.CompareTo(key); });
      //  //_octree.Remove("Ranger" + i);
      //  _tuxes[i].Meshes.Remove<string>("Body", (StaticMesh mesh, string key) => { return mesh.Id.CompareTo(key); });
      //}

      Renderer.Font = TextManager.GetFont("Calibri");

      _dodeca = StaticModelManager.GetModel("Dodecahedron");

      // ONCE YOU ARE DONE LOADING, BE SURE TO SET YOUR READY 
      // PROPERTY TO TRUE SO MY ENGINE DOESN'T SCREAM AT YOU
      _isReady = true;
    }

    #endregion

    #region Rendering

    public void Render()
    {
      // RENDER YOUR GAME HERE
      // Use the static class "Renderer"
      // EXAMPLES:
        // Renderer.CurrentCamera = cameraYouWantToUse;
      Renderer.CurrentCamera = _camera;

      //Renderer.SetProjectionMatrix();

      _octree.Foreach(
        (StaticModel model) => { 
          Renderer.DrawStaticModel(model); },
        new double[] { -100000, -100000, -100000, },
        new double [] { 100000, 100000, 100000 });

      Renderer.DrawSkybox(_skybox);
      Renderer.DrawStaticModel(_terrain);
      Renderer.DrawStaticModel(_mountain);
      Renderer.DrawStaticModel(_mountain2);

      //if (_mushroomCloud.Scale.X > 0)
      if (_mushroomCloud.Scale.X > 0 && _bool)
        Renderer.DrawStaticModel(_mushroomCloud);

      // EXAMPLE:
        // Renderer.RenderText("whatToWrite", x, y, size, rotation, color);
      Renderer.RenderText("Welcome To", 0f, 1f, 50f, 0, Color.Black);
      Renderer.RenderText("SevenEngine!", .15f, .95f, 50f, 0, Color.Teal);

      Renderer.RenderText("Close: ESC", 0f, .2f, 30f, 0f, Color.White);
      Renderer.RenderText("Fullscreen: F1", 0f, .15f, 30f, 0, Color.SteelBlue);
      Renderer.RenderText("Camera Movement: w, a, s, d", 0f, .1f, 30f, 0, Color.Tomato);
      Renderer.RenderText("Camera Angle: j, k, l, i", 0f, .05f, 30f, 0, Color.Yellow);

      Renderer.DrawStaticModel(_dodeca);
    }

    #endregion

    #region Updating

    public string Update(float elapsedTime)
    {
      _time += elapsedTime / 4f;

      if (_time > 200)
      {
        _time = 0;
        _bool = !_bool;
      }

      CameraControls();
      foreach (StaticModel model in _rangers)
        model.Orientation.W += 3;
      foreach (StaticModel model in _tuxes)
        model.Orientation.W += 3;

      _skybox.Position.X = _camera.Position.X;
      _skybox.Position.Y = _camera.Position.Y;
      _skybox.Position.Z = _camera.Position.Z;

      _mushroomCloud.Scale.X = _time;
      _mushroomCloud.Scale.Y = _time;
      _mushroomCloud.Scale.Z = _time;

      //_mushroomCloud.Scale.X = Trigonometry.Sin(_time / 300f) * 200f;
      //_mushroomCloud.Scale.Y = Trigonometry.Sin(_time / 300f) * 200f;
      //_mushroomCloud.Scale.Z = Trigonometry.Sin(_time / 300f) * 200f;

      _dodeca.Orientation.Z += 1;
      _dodeca.Orientation.W += 1;

      return "Don't Change States";
    }

    private void CameraControls()
    {
      // Camera position movement
      if (InputManager.Keyboard.Qdown)
        if (InputManager.Keyboard.ShiftLeftdown)
          _camera.Move(_camera.Down, _camera.PositionSpeed * 100);
        else 
          _camera.Move(_camera.Down, _camera.PositionSpeed);
      if (InputManager.Keyboard.Edown)
        if (InputManager.Keyboard.ShiftLeftdown) 
          _camera.Move(_camera.Up, _camera.PositionSpeed * 100);
        else
          _camera.Move(_camera.Up, _camera.PositionSpeed);
      if (InputManager.Keyboard.Adown)
        if (InputManager.Keyboard.ShiftLeftdown)
          _camera.Move(_camera.Left, _camera.PositionSpeed * 100);
        else
          _camera.Move(_camera.Left, _camera.PositionSpeed);
      if (InputManager.Keyboard.Wdown)
        if (InputManager.Keyboard.ShiftLeftdown)
          _camera.Move(_camera.Forward, _camera.PositionSpeed * 100);
        else
          _camera.Move(_camera.Forward, _camera.PositionSpeed);
      if (InputManager.Keyboard.Sdown)
        if (InputManager.Keyboard.ShiftLeftdown)
          _camera.Move(_camera.Backward, _camera.PositionSpeed * 100);
        else
          _camera.Move(_camera.Backward, _camera.PositionSpeed);
      if (InputManager.Keyboard.Ddown)
        if (InputManager.Keyboard.ShiftLeftdown)
          _camera.Move(_camera.Right, _camera.PositionSpeed * 100);
        else
          _camera.Move(_camera.Right, _camera.PositionSpeed);

      // Camera look angle adjustment
      if (InputManager.Keyboard.Kdown)
        _camera.RotateX(.01f);
      if (InputManager.Keyboard.Idown)
        _camera.RotateX(-.01f);
      if (InputManager.Keyboard.Jdown)
        _camera.RotateY(.01f);
      if (InputManager.Keyboard.Ldown)
        _camera.RotateY(-.01f);
    }

    #endregion
  }
}