using System;

using Seven;
using SevenEngine;
using Seven.Structures;
using SevenEngine.Imaging;
using SevenEngine.StaticModels;
using Seven.Mathematics;
using SevenEngine.Physics.Primitives;

using Game.Objects;
using OpenTK;
using Game.Objects.Types.Properties;

namespace Game.States
{
    public class ScoutingState : InterfaceGameState
    {
        public static bool _3d = false;
        public static int _map = 0;
        public static bool _paused = false;
        public static bool _showlines = false;

        private string _id;
        private bool _isReady;

        public string Id { get { return _id; } set { _id = value; } }
        public bool IsReady { get { return _isReady; } }

        #region State Fields

        //public static readonly float MeterLength = 10;
        //public static ListArray<Explosion> _explosions = new ListArray(10);

        Game _gameWindow;
        Plane _field;
        Static _playerCursor;

        Camera _camera;
        
        //StaticModel _terrain;
        StaticModel _mountain;
        float angle;

        Dreadnaught _dreadnaught;
        SmallCannon _cannon;
        SmallCannon _cannon2;

        SkyBox _skybox;

        #endregion

        public ScoutingState(string id, Game gameWindow)
        {
            _id = id;
            _isReady = false;
            _gameWindow = gameWindow;
        }

        #region State Utils

        public Ray GetMouseVector()
        {
            // generate an object space ray
            // convert the viewport coords to openGL normalized device coords
            float xpos = 2 * ((float)InputManager.Mouse.X / _gameWindow.Width) - 1;
            float ypos = 2 * (1 - (float)InputManager.Mouse.Y / _gameWindow.Height) - 1;
            //Output.WriteLine(InputManager.Mouse.X + "x" + InputManager.Mouse.Y);
            //Output.WriteLine(xpos + "x" + ypos);
            Vector4 startRay = new Vector4(xpos, ypos, -1, 1);
            Vector4 endRay = new Vector4(xpos, ypos, 1, 1);
            // Reverse Project

            Matrix4 view_matrix = _camera.GetMatrix();
            Matrix4 projection_matrix = _camera.GetProjectionMatrix();
            Matrix4 trans = view_matrix * projection_matrix;
            trans.Invert();
            startRay = Vector4.Transform(startRay, trans);
            endRay = Vector4.Transform(endRay, trans);
            Vector3 sr = startRay.Xyz / startRay.W;
            Vector3 er = endRay.Xyz / endRay.W;

            return new Ray(new Vector<float>(sr.X, sr.Y, sr.Z), new Vector<float>(er.X, er.Y, er.Z));
        }

        void UpdateCursorPosition()
        {
            Ray playerRay = GetMouseVector();
            try
            {
                _playerCursor.Position = _field.GetIntersection(playerRay.Start, playerRay.End);
                if (float.IsNaN(_playerCursor.Position.X))
                    throw new Exception("NaN");
            }
            catch (DivideByZeroException)
            {
            }
        }

        #endregion

        #region Loading

        public void Load()
        {
            _camera = new Camera();
            _camera.PositionSpeed = 5;
            _camera.Move(_camera.Up, 400);
            _camera.Move(_camera.Backward, 1500);
            _camera.Move(_camera.Backward, 300);

            _field = new Plane(0, 1, 0, 0);

            _playerCursor = new Static("cursor","Tux");
            _playerCursor.StaticModel.Scale=new Vector<float>(20, 20, 20);
            

            _skybox = new SkyBox();
            _skybox.Scale.X = 10000;
            _skybox.Scale.Y = 10000;
            _skybox.Scale.Z = 10000;
            _skybox.Left = TextureManager.Get("SkyboxLeft");
            _skybox.Right = TextureManager.Get("SkyboxRight");
            _skybox.Front = TextureManager.Get("SkyboxFront");
            _skybox.Back = TextureManager.Get("SkyboxBack");
            _skybox.Top = TextureManager.Get("SkyboxTop");
            /*
            _terrain = StaticModelManager.GetModel("Terrain");
            _terrain.Scale = new Vector<float>(500, 20, 500);
            _terrain.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _terrain.Position = new Vector<float>(0, 0, 0);
            */
            _mountain = StaticModelManager.GetModel("Mountain");
            _mountain.Scale = new Vector<float>(5000, 5000, 5000);
            _mountain.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _mountain.Position = new Vector<float>(4000, 0, 1000);

            //_mountain2 = StaticModelManager.GetModel("Mountain2");
            //_mountain2.Scale = new Vector<float>(3500, 3500, 3500);
            //_mountain2.Orientation = new Quaternion(0, 0, 0, 0);
            //_mountain2.Position = new Vector<float>(0, 0, 2500);
            
            _dreadnaught = new Dreadnaught("player");
            _dreadnaught.StaticModel.Position = new Vector<float>(0, 0, 0);
            _dreadnaught.StaticModel.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _dreadnaught.StaticModel.Scale = new Vector<float>(20, 20, 20);
            
            _cannon = new SmallCannon("cannon_base");
            _cannon.StaticModel.Position = new Vector<float>(10, 10, 10);
            _cannon.StaticModel.Orientation = Geometric.Generate_Quaternion(-Constants.pi_float/4, 0, 0, 1);
            _cannon.StaticModel.Scale = new Vector<float>(20, 20, 20);
            
            /*
            _cannon2 = new SmallCannon("cannon");
            _cannon2.StaticModel.Position = new Vector<float>(0, 0, 0);
            //_cannon2.StaticModel.Orientation = new Quaternion(0,0,0,1);
            //_cannon2.StaticModel.Orientation = new Quaternion((float)Math.Sin(-Constants.pi_double / 8), 0, 0, (float)Math.Cos(-Constants.pi_double / 8));
            _cannon2.StaticModel.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _cannon2.StaticModel.Scale = new Vector<float>(20, 20, 20);
            */
            //_cannon.StaticModel.addChildren(_cannon2.StaticModel);

            GenerateUnits();

            Renderer.Font = TextManager.GetFont("Calibri");

            // ONCE YOU ARE DONE LOADING, BE SURE TO SET YOUR READY 
            // PROPERTY TO TRUE SO MY ENGINE DOESN'T SCREAM AT YOU
            _isReady = true;

        }


        public static Comparison Compare(Link<Vector<float>, Vector<float>, Color> left, Link<Vector<float>, Vector<float>, Color> right)
        {
            // this is a terrible hack... dont do this
            if (left.One == right.One && left.Two == right.Two)
                return Comparison.Equal;
            else
                return Comparison.Greater;
        }

        private void GenerateUnits()
        {

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



            Renderer.DrawSkybox(_skybox);
            //Renderer.DrawStaticModel(_terrain);
            Renderer.DrawStaticModel(_mountain);
            // Renderer.DrawStaticModel(_mountain2);

            //Renderer.DrawStaticModel(_dreadnaught.StaticModel);
            Renderer.DrawStaticModel(_cannon.StaticModel);
            //Renderer.DrawStaticModel(_cannon2.StaticModel);
            Renderer.DrawStaticModel(_playerCursor.StaticModel);

            // EXAMPLE:
            // Renderer.RenderText("whatToWrite", x, y, size, rotation, color);
            Renderer.RenderText("Welcome To", 0f, 1f, 50f, 0, Color.Black);
            Renderer.RenderText("SevenEngine!", .15f, .95f, 50f, 0, Color.Teal);

            Renderer.RenderText("Battle Controls: Space, R, T, G, Y", .55f, .95f, 30f, 0, Color.Black);

            Renderer.RenderText("Map: " + _map, .85f, .85f, 30f, 0, Color.Black);
            if (_3d)
                Renderer.RenderText("Space: Yes", .85f, .9f, 30f, 0, Color.Black);
            else
                Renderer.RenderText("Space: No", .85f, .9f, 30f, 0, Color.Black);

            Renderer.RenderText("Unit Controls: z, x, c, v, b, n", .6f, .07f, 30f, 0, Color.Black);


            Renderer.RenderText("Close: ESC", 0f, .2f, 30f, 0f, Color.White);
            Renderer.RenderText("Fullscreen: F1", 0f, .15f, 30f, 0, Color.SteelBlue);
            Renderer.RenderText("Camera Movement: w, a, s, d", 0f, .1f, 30f, 0, Color.Tomato);
            Renderer.RenderText("Camera Angle: j, k, l, i", 0f, .05f, 30f, 0, Color.Yellow);
        }

        #endregion

        #region Updating

        public string Update(float elapsedTime)
        {
            CameraControls();
            _skybox.Position.X = _camera.Position.X;
            _skybox.Position.Y = _camera.Position.Y;
            _skybox.Position.Z = _camera.Position.Z;


            /*
            if (InputManager.Keyboard.Gdown)
            {
                _cannon.Position = new Vector<float>(500, 500, 500);

                //_cannon.LookAt(new Vector<float>(50, 50, 50));
                //_cannon2.LookAt(new Vector<float>(0, 1000, 0));
            }
            //_cannon.LookAt(new Vector<float>(0, 1000, 0));  
            //angle = angle - Constants.pi_float/ 64;
            else
            {
                _cannon.Position = new Vector<float>(-350, 500, -350);
                //_cannon.LookAt(new Vector<float>(-35, 50, -35));
                //_cannon2.LookAt(new Vector<float>(0, 1000, 0));  
                }
                //_cannon.LookAt(new Vector<float>(-50, -50, -50));
                //angle = angle + Constants.pi_float / 64;

             */

            _cannon.LookAt(_playerCursor.Position);
            _dreadnaught.LookAt(_cannon.Position);
            //_cannon.StaticModel.OrientationRelative = Quaternion.Slerp(_cannon.StaticModel.OrientationRelative, Geometric.Generate_Quaternion(angle, 0, 1, 0), Game.DeltaTime * 0.001f);
            //_cannon.Orientation = Geometric.Generate_Quaternion(angle, 0, 1, 0);

            
            if (InputManager.Keyboard.Rpressed)
                _showlines = !_showlines;

            if (InputManager.Keyboard.Ypressed)
                _3d = !_3d;

            if (InputManager.Keyboard.Tpressed)
                GenerateUnits();

            if (InputManager.Keyboard.Spacepressed)
                _paused = !_paused;



            //Updating where cursor is!
            UpdateCursorPosition();

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