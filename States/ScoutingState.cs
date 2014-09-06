using System;
using System.Collections.Generic;

using Seven;
using SevenEngine;
using SevenEngine.Imaging;
using SevenEngine.StaticModels;
using Seven.Mathematics;
using SevenEngine.Physics.Primitives;

using Game.Objects;
using OpenTK;
using Game.Objects.Types.Properties;
using Game.Objects.Types;

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
        Plane _cursorField;
        Vector3 _spaceLimit;        //maximum absolute values for deleting physics objects out of a certain range
        Vector3 _playerLimit;       //maximum absolute values for the player to go
        Static _playerCursor;

        Camera _camera;
        const float _maxCamHeight = 6000;
        const float _minCamHeight = 100;
        
        StaticModel _terrain;
        StaticModel _mountain;

        Dreadnaught _dreadnaught;

        //Current list of objects for physics
        private List<Bullet> _bullets;
        private List<Asteroid> _asteroids;

        ulong _total_spawned_asteroids = 0;
        int _num_asteroids_on_screen = 0;
        const int _max_asteroids_on_screen = 50;
        float _min_spawning_time = 15;
        Vector3[] _spawnpoints;
        float[] _spawnpoint_angles;
        float[] _spawnpoint_times;
        float _spawnRange;
        float _minAsteroidSpeed;
        float _maxAsteroidSpeed;

        SkyBox _skybox;

        #endregion

        public ScoutingState(string id, Game gameWindow)
        {
            _id = id;
            _isReady = false;
            _gameWindow = gameWindow;
        }

        #region Loading

        public void Load()
        {
            //Camera initial position
            _camera = new Camera();
            _camera.PositionSpeed = 15;
            _camera.Move(_camera.Up, _maxCamHeight);
            _camera.Move(_camera.Backward, 3400);
            //Camera initial orientation
            Quaternion lookAtCenter = Geometric.FreeLookAt(_camera.Position, new Vector3(0,0,0), new Vector3(0,1,0));
            _camera.Forward = Geometric.Quaternion_Rotate(lookAtCenter, _camera.Forward);
            _camera.Up = Geometric.Quaternion_Rotate(lookAtCenter, _camera.Up);

            _cursorField = new Plane(0, 1, 0, 0);
            _spaceLimit = new Vector3(8000, 2000, 6000);    //Y is important since some debris could escape that way
            _playerLimit = new Vector3(3800, 1000, 2100);   //Y is not important since player cannot move there

            _asteroids = new List<Asteroid>();
            _bullets = new List<Bullet>();


            //The following loaded values have been chosen to keep limits symmetric and have a reasonable dynamic camera view for the game
            _spawnpoints = new Vector3[5];
            _spawnpoint_angles = new float[5];
            _spawnpoint_times = new float[5] { 0, 0, 0, 0, 0 };    
            _spawnRange = Constants.pi_float / 6;
            _minAsteroidSpeed = 0.05f;
            _maxAsteroidSpeed = 0.2f;
            _spawnpoints[0] = new Vector3(4000, 0, 750);
            _spawnpoint_angles[0] = 3 * Constants.pi_float/2 - Constants.pi_float / 6;
            _spawnpoints[1] = new Vector3(3000, 0, 3000);
            _spawnpoint_angles[1] = 2 * Constants.pi_float - Constants.pi_float / 4 - Constants.pi_float/2;
            _spawnpoints[2] = new Vector3(0, 0, 3400);
            _spawnpoint_angles[2] = 3 * (Constants.pi_float / 2) - Constants.pi_float/2;
            _spawnpoints[3] = new Vector3(-3000, 0, 3000);
            _spawnpoint_angles[3] = Constants.pi_float + Constants.pi_float / 4 - Constants.pi_float/2;
            _spawnpoints[4] = new Vector3(-4000, 0, 750);
            _spawnpoint_angles[4] = Constants.pi_float + Constants.pi_float / 6 - Constants.pi_float/2;
            /*
            _spawnpoints[0] = new Vector3(4000, 0, 750);
            _spawnpoint_angles[0] = 2 * Constants.pi_float - Constants.pi_float / 6;
            _spawnpoints[1] = new Vector3(3000, 0, 3000);
            _spawnpoint_angles[1] = 2 * Constants.pi_float - Constants.pi_float / 4;
            _spawnpoints[2] = new Vector3(0, 0, 3000);
            _spawnpoint_angles[2] = 3 * (Constants.pi_float / 2);
            _spawnpoints[3] = new Vector3(-3000, 0, 3000);
            _spawnpoint_angles[3] = Constants.pi_float + Constants.pi_float / 4;
            _spawnpoints[4] = new Vector3(-4000, 0, 750);
            _spawnpoint_angles[4] = Constants.pi_float + Constants.pi_float / 6;
             * */

            //TODO: SET POSITIONS

            _playerCursor = new Static("cursor","Tux");
            _playerCursor.StaticModel.Scale=new Vector3(10, 10, 10);
            
            _skybox = new SkyBox();
            _skybox.Scale = new Vector3(10000, 10000, 10000);

            _skybox.Left = TextureManager.Get("SkyboxLeft");
            _skybox.Right = TextureManager.Get("SkyboxRight");
            _skybox.Front = TextureManager.Get("SkyboxFront");
            _skybox.Back = TextureManager.Get("SkyboxBack");
            _skybox.Top = TextureManager.Get("SkyboxTop");
            _skybox.Bottom = TextureManager.Get("SkyboxBottom");

            _terrain = StaticModelManager.GetModel("Planet_model");
            _terrain.Scale = new Vector3(500, 1, 500);
            _terrain.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _terrain.Position = new Vector3(0, -1000, 0);
            
            _mountain = StaticModelManager.GetModel("Mountain");
            _mountain.Scale = new Vector3(5000, 5000, 5000);
            _mountain.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _mountain.Position = new Vector3(4000, 0, 1000);

            //_mountain2 = StaticModelManager.GetModel("Mountain2");
            //_mountain2.Scale = new Vector3(3500, 3500, 3500);
            //_mountain2.Orientation = new Quaternion(0, 0, 0, 0);
            //_mountain2.Position = new Vector3(0, 0, 2500);
            
            _dreadnaught = new Dreadnaught("player");
            _dreadnaught.Position = new Vector3(0, 0, -1400);
            _dreadnaught.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _dreadnaught.Scale = new Vector3(10, 10, 10);


            Renderer.Font = TextManager.GetFont("Calibri");

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

            Renderer.DrawSkybox(_skybox);
            Renderer.DrawStaticModel(_terrain);
            //Renderer.DrawStaticModel(_mountain);
            // Renderer.DrawStaticModel(_mountain2);

            if (true)
            {
                Renderer.DrawStaticModel(_dreadnaught.StaticModel);
            }
            
            //Renderer.DrawStaticModel(_cannon.StaticModel);
            //Renderer.DrawStaticModel(_cannon2.StaticModel);
            Renderer.DrawStaticModel(_playerCursor.StaticModel);

            foreach (Asteroid asteroid in _asteroids)
            {
                if (!asteroid.IsDead)
                    Renderer.DrawStaticModel(asteroid.StaticModel);
            }

            foreach (Bullet bullet in _bullets)
            {
                if (!bullet.IsDead)
                    Renderer.DrawLine(bullet.Position, bullet.Position + bullet.Velocity*600, new Color(255, 0, 0, 1));
            }

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
            ScrollPlanet();
            
            CameraControls();

            DreadnaughtControls();



            //_dreadnaught.LookAt(_playerCursor.Position);
            _dreadnaught.AimAt(_playerCursor.Position);
            _dreadnaught.UpdateStandard();
            
            _skybox.Position = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);

            Physics();

            //Output.WriteLine(_playerCursor.Position + "x");
            /*
            if (InputManager.Keyboard.Gdown)
            {
                _cannon.Position = new Vector3(500, 500, 500);

                //_cannon.LookAt(new Vector3(50, 50, 50));
                //_cannon2.LookAt(new Vector3(0, 1000, 0));
            }
            //_cannon.LookAt(new Vector3(0, 1000, 0));  
            //angle = angle - Constants.pi_float/ 64;
            else
            {
                _cannon.Position = new Vector3(-350, 500, -350);
                //_cannon.LookAt(new Vector3(-35, 50, -35));
                //_cannon2.LookAt(new Vector3(0, 1000, 0));  
                }
                //_cannon.LookAt(new Vector3(-50, -50, -50));
                //angle = angle + Constants.pi_float / 64;

             */

            //_cannon.LookAt(_playerCursor.Position);

            //_cannon.StaticModel.OrientationRelative = Quaternion.Slerp(_cannon.StaticModel.OrientationRelative, Geometric.Generate_Quaternion(angle, 0, 1, 0), Game.DeltaTime * 0.001f);
            //_cannon.Orientation = Geometric.Generate_Quaternion(angle, 0, 1, 0);

            if (InputManager.Mouse.LeftClickdown)
            {
                List<Bullet> incoming = _dreadnaught.Shoot(_playerCursor.Position);
                if (incoming.Count > 0) foreach (Bullet bullet in incoming) _bullets.Add(bullet);
            }

            if (InputManager.Keyboard.Spacepressed)
                _paused = !_paused;


            SpawnAsteroids();

            //CollisionDetection();

            //Updating where cursor is!
            UpdateCursorPosition();

            return "Don't Change States";
        }

        private void CameraControls()
        {

            // ORIENTATION
            //Quaternion lookAtCenter = Geometric.FreeLookAt(_camera.Position, new Vector3(0,0,0), new Vector3(0, 1, 0));
            Quaternion lookAtPlayer = Geometric.FreeLookAt(_camera.Position, _dreadnaught.Position, new Vector3(0, 1, 0));
            //Quaternion cameraLook = Quaternion.Slerp(lookAtCenter, lookAtCursor, 0.001f);
            _camera.Forward = Geometric.Quaternion_Rotate(lookAtPlayer, new Vector3(0, 0, 1));
            _camera.Up = Geometric.Quaternion_Rotate(lookAtPlayer, new Vector3(0, 1, 0));

            // Z POSITION
            float percentage = -_camera.Forward.Y;
            float newY = ((_maxCamHeight - _minCamHeight) * percentage) + _minCamHeight;
            _camera.Position = new Vector3(_camera.Position.X, newY, _camera.Position.Z);
            //Output.WriteLine(_camera.Position + "x");
            /*
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
            */

        }

        private void DreadnaughtControls()
        {
            bool commandsEnabled = false;
            Vector3 command = new Vector3(0, 0, 0);
            Vector3 repelForce = new Vector3(0, 0, 0);


            if (!IsOutOfBounds(_dreadnaught.Position))   //if dreadnaught is in bounds, enable controls (player can accelerate spaceship)
            {
                commandsEnabled = true;
            }
            else
            {
                //If out of bounds, disable controls and apply a small "force" towards the origin, until spaceship changes direction
                if (Vector3.Dot(_dreadnaught.Velocity, _dreadnaught.Position) > 0)
                {
                    commandsEnabled = false;
                    repelForce = new Vector3(-_dreadnaught.Position.X, 0, -_dreadnaught.Position.Z);
                    repelForce.NormalizeFast();
                }//If out of bounds but spaceship has changed direction, enable commands (to allow player to get back)
                else
                {
                    commandsEnabled = true;
                }
            }


            // Dreadnaught position movement
            if (commandsEnabled)
            {
                if (InputManager.Keyboard.Sdown)
                    if (InputManager.Keyboard.ShiftLeftdown)
                        _camera.Move(_camera.Down, _camera.PositionSpeed * 100);
                    else
                        command.Z -= 1;

                if (InputManager.Keyboard.Wdown)
                    if (InputManager.Keyboard.ShiftLeftdown)
                        _camera.Move(_camera.Up, _camera.PositionSpeed * 100);
                    else
                        command.Z += 1;

                if (InputManager.Keyboard.Adown)
                    if (InputManager.Keyboard.ShiftLeftdown)
                        _camera.Move(_camera.Left, _camera.PositionSpeed * 100);
                    else
                        command.X += 1;

                if (InputManager.Keyboard.Ddown)
                    if (InputManager.Keyboard.ShiftLeftdown)
                        _camera.Move(_camera.Left, _camera.PositionSpeed * 100);
                    else
                        command.X -= 1;
            }

            command.NormalizeFast();
            command += repelForce;      //add repelforce (if any)
            Vector3 nextVelocity = _dreadnaught.Velocity + (command * _dreadnaught.MaxAcceleration * Game.DeltaTime);
            if (Math.Abs(nextVelocity.X) > _dreadnaught.MaxSpeed) nextVelocity.X = _dreadnaught.Velocity.X;
            if (Math.Abs(nextVelocity.Z) > _dreadnaught.MaxSpeed) nextVelocity.Z = _dreadnaught.Velocity.Z;
            
            //Movement apply
            _dreadnaught.Velocity = nextVelocity;
            _dreadnaught.Position = _dreadnaught.Position + _dreadnaught.Velocity;

 

            //_dreadnaught.Position = Vector3.Lerp(_dreadnaught.Position, destination, _dreadnaught.MoveSpeed * Game.DeltaTime);


        }

        #endregion

        #region State Functions

        //Generate a model space ray from mouse position on screen
        public Ray GetMouseVector()
        {
            // convert the viewport coords to openGL normalized device coords
            float xpos = 2 * ((float)InputManager.Mouse.X / _gameWindow.Width) - 1;
            float ypos = 2 * (1 - (float)InputManager.Mouse.Y / _gameWindow.Height) - 1;
            //Output.WriteLine(InputManager.Mouse.X + "x" + InputManager.Mouse.Y);
            //Output.WriteLine(xpos + "x" + ypos);
            // View ray
            Vector4 startRay = new Vector4(xpos, ypos, -1, 1);
            Vector4 endRay = new Vector4(xpos, ypos, 1, 1);

            // Reverse Project to World Space
            Matrix4 view_matrix = _camera.GetMatrix();
            Matrix4 projection_matrix = _camera.GetProjectionMatrix();
            Matrix4 trans = view_matrix * projection_matrix;
            trans.Invert();
            startRay = Vector4.Transform(startRay, trans);
            endRay = Vector4.Transform(endRay, trans);
            Vector3 sr = startRay.Xyz / startRay.W;
            Vector3 er = endRay.Xyz / endRay.W;

            return new Ray(new Vector3(sr.X, sr.Y, sr.Z), new Vector3(er.X, er.Y, er.Z));
        }

        private void UpdateCursorPosition()
        {
            Ray playerRay = GetMouseVector();
            try
            {
                _playerCursor.Position = _cursorField.GetIntersection(playerRay.Start, playerRay.End);
                if (float.IsNaN(_playerCursor.Position.X))
                    throw new Exception("NaN");
            }
            catch (DivideByZeroException)
            {
            }
        }

        public bool IsOutOfView(Vector3 obj)
        {
            Vector3 abs = new Vector3(Math.Abs(obj.X), Math.Abs(obj.Y), Math.Abs(obj.Z));
            if (abs.X > _spaceLimit.X || abs.Y > _spaceLimit.Y || abs.Z > _spaceLimit.Z)
                return true;
            else
                return false;
        }

        public bool IsOutOfBounds(Vector3 obj)
        {
            Vector3 abs = new Vector3(Math.Abs(obj.X), Math.Abs(obj.Y), Math.Abs(obj.Z));
            if (abs.X > _playerLimit.X || abs.Y > _playerLimit.Y || abs.Z > _playerLimit.Z)
                return true;
            else
                return false;
        }

        private void SpawnAsteroids()
        {
            Random random = new Random();
            Asteroid _spawning;
            for (int i = 0; i < 5; i++) _spawnpoint_times[i] -= Game.DeltaTime * 0.001f;

            if (_num_asteroids_on_screen < _max_asteroids_on_screen)
            {
                int _selectedSpawnPoint = random.Next(0, 5);
                if (_spawnpoint_times[_selectedSpawnPoint] <= 0)
                {
                    _spawning = new Asteroid("asteroid_" + _total_spawned_asteroids, 100);
                    _spawning.Scale = new Vector3(200, 200, 200);

                    _spawning.Position = _spawnpoints[_selectedSpawnPoint];
                    double angleRatio = random.NextDouble() - 0.5f;
                    float angle = _spawnpoint_angles[_selectedSpawnPoint] + _spawnRange * (float)angleRatio;
                    double speedRatio = random.NextDouble();
                    float speed = (_maxAsteroidSpeed - _minAsteroidSpeed) * (float)speedRatio + _minAsteroidSpeed;
                    _spawning.Velocity = new Vector3(speed * (float)Math.Sin(angle), 0, speed * (float)Math.Cos(angle));

                    _asteroids.Add(_spawning);
                    _num_asteroids_on_screen++;
                    _total_spawned_asteroids++;
                    //Output.WriteLine("spawned:" + _total_spawned_asteroids + "from: " + _selectedSpawnPoint);
                    _spawnpoint_times[_selectedSpawnPoint] = _min_spawning_time;
                }
            }

        }

        private void ScrollPlanet()
        {

        }

        private void Physics()
        {

            foreach (Bullet bullet in _bullets)
            {
                foreach (Asteroid asteroid in _asteroids)
                {
                    bullet.Move();

                    if (IsOutOfView(bullet.Position))
                        bullet.IsDead = true;
                    else if (!asteroid.IsDead && asteroid.HasCollided(bullet.Position))
                    {
                        asteroid.Hit(200);
                        bullet.IsDead = true;
                        _num_asteroids_on_screen--;
                        //_bullets.Remove(bullet);  //CAN'T DO IN CURRENTLY SCROLLING LIST!!
                    }
                }
            }

            foreach (Asteroid asteroid in _asteroids)
            {
                //Output.WriteLine( asteroid.Id);
                if (!asteroid.IsDead)       //_asteroids.Remove(asteroid);  //CAN'T DO IN CURRENTLY SCROLLING LIST!!
                {
                    asteroid.Move();

                    if (IsOutOfView(asteroid.Position))
                        asteroid.IsDead = true;
                    else
                    {
                        if (asteroid.HasCollided(_dreadnaught.BoundingBox))
                        {
                            _dreadnaught.IsDead = true;
                        }

                        foreach (Asteroid other_asteroid in _asteroids)
                        {
                            if (asteroid.HasCollided(other_asteroid.BoundingBox) && asteroid.Id != other_asteroid.Id)
                            {
                                //Exchange velocity vectors (bounce)
                                Vector3 temp = new Vector3(asteroid.Velocity.X, 0, asteroid.Velocity.Z);
                                asteroid.Velocity = new Vector3(other_asteroid.Velocity.X, 0, other_asteroid.Velocity.Z);
                                other_asteroid.Velocity = temp;
                            }
                        }
                    }
                }
            }

            //Flush lists for dead objects (after it reaches a custom limit so I don't need to flush it each cycle)
            if (_asteroids.Count > 100) _asteroids.RemoveAll(item => item.IsDead == true);
            if (_bullets.Count > 200) _bullets.RemoveAll(item => item.IsDead == true);
            //ATTENTION: OBJECTS IN LIST DO NOT CORRESPOND TO ALIVE OBJECTS!! IT ONLY CONTAINS REFERENCES TO DEAD AND ALIVE OBJECTS IN MEMORY!
            //Alternative to flushing lists can be iterating within the for BACKWARDS!!


        }

        #endregion
    }
}