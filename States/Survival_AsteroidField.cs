using System;
using System.Collections.Generic;

using Seven;
using SevenEngine;
using SevenEngine.Imaging;
using SevenEngine.StaticModels;
using Seven.Mathematics;
using SevenEngine.Physics.Primitives;

using AsTKoids.Objects;
using OpenTK;
using AsTKoids.Objects.Types.Behaviours;
using AsTKoids.Objects.Types;

namespace AsTKoids.States
{
    public class Survival_AsteroidField : InterfaceGameState
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

        Game _gameWindow;

        Camera _camera = new Camera();
        const float _maxCamHeight = 5000;
        const float _minCamHeight = 800;
        uint _cameraMode = 1;

        //SkyBox _skybox;                           //Not currently really useful (camera view is always towards terrain)

        Plane _cursorField;
        Static _playerCursor;
        Vector3 _savedPlayerCursorPosition;         //This vector is used as a copy of playerCursor when Dreadnaught Yaw is disabled in game
        float _cursorAnimationRotation;

        //Setup limits
        Vector3 _spaceLimit = new Vector3(8000, 2000, 6000);    //BOX: maximum absolute values for deleting physics objects out of a certain range
        const float _playerLimitRange = 1800;                   //SPHERE: maximum range around the origin reachable by dreadnaught
        // EXPERIMENTAL
        //float _playerLimitEccentricityX = 500;                //0 is a perfect circle, negative is reducing, positive is extending along X axis
        //float _playerLimitEccentricityZ = -500;               //0 is a perfect circle, negative is reducing, positive is extending along Z axis

        //One intance objects
        StaticModel[] _planet;
        Dreadnaught _dreadnaught;
        //List of objects for physics
        private List<Bullet> _bullets;
        private List<Asteroid> _asteroids;

        //Asteroids control
        ulong _total_spawned_asteroids;
        int _num_asteroids_on_screen;
        const int _max_asteroids_on_screen = 20;
        //Following values apply for biggest asteroids (the slowest)
        const float _min_spawning_delay = 10;
        Vector3[] _spawnpoints;
        float[] _spawnpoint_angles;
        float[] _spawnpoint_times;
        float _spawnRange;
        const float _minAsteroidSpeed = 0.1f;
        const float _maxAsteroidSpeed = 0.3f;

        //Game info
        ulong _score = 0;

        #endregion

        public Survival_AsteroidField(string id, Game gameWindow)
        {
            _id = id;
            _isReady = false;
            _gameWindow = gameWindow;
        }

        #region Loading

        public void Load()
        {
            // LOAD THE INITIAL GAME STATE HERE

            _score = 0;

            //Camera initial position
            _camera = new Camera();
            _camera.PositionSpeed = 15;
            _camera.Move(_camera.Up, _maxCamHeight);
            _camera.Move(_camera.Backward, 2600);
            //Camera initial orientation
            Quaternion lookAtCenter = Geometric.FreeLookAt(_camera.Position, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            _camera.Forward = Geometric.Quaternion_Rotate(lookAtCenter, _camera.Forward);
            _camera.Up = Geometric.Quaternion_Rotate(lookAtCenter, _camera.Up);
            //Camera -> FIRST THING TO DO IS INITIZIALIZE RENDERER WITH A Camera
            Renderer.CurrentCamera = _camera;

            //Setup cursor plane
            _cursorField = new Plane(0, 1, 0, 0);

            //Initializing lists
            _asteroids = new List<Asteroid>();
            _num_asteroids_on_screen = 0;
            _total_spawned_asteroids = 0;
            _bullets = new List<Bullet>();

            //The following loaded values have been chosen to keep limits symmetric and have a reasonable dynamic camera view for the game
            _spawnpoints = new Vector3[5];
            _spawnpoint_angles = new float[5];
            _spawnpoint_times = new float[5] { 0, 0, 0, 0, 0 };
            _spawnRange = Constants.pi_float / 6;
            _spawnpoints[0] = new Vector3(4000, 0, 1250);
            _spawnpoint_angles[0] = 3 * Constants.pi_float / 2 - Constants.pi_float / 6;
            _spawnpoints[1] = new Vector3(3000, 0, 3500);
            _spawnpoint_angles[1] = 2 * Constants.pi_float - Constants.pi_float / 4 - Constants.pi_float / 2;
            _spawnpoints[2] = new Vector3(0, 0, 4500);
            _spawnpoint_angles[2] = 3 * (Constants.pi_float / 2) - Constants.pi_float / 2;
            _spawnpoints[3] = new Vector3(-3000, 0, 3500);
            _spawnpoint_angles[3] = Constants.pi_float + Constants.pi_float / 4 - Constants.pi_float / 2;
            _spawnpoints[4] = new Vector3(-4000, 0, 1250);
            _spawnpoint_angles[4] = Constants.pi_float + Constants.pi_float / 6 - Constants.pi_float / 2;

            //Skybox setup (REMOVED)
            /*
            _skybox = new SkyBox();
            _skybox.Scale = new Vector3(20000, 20000, 20000);
            _skybox.Left = TextureManager.Get("SkyboxLeft");
            _skybox.Right = TextureManager.Get("SkyboxRight");
            _skybox.Front = TextureManager.Get("SkyboxFront");
            _skybox.Back = TextureManager.Get("SkyboxBack");
            _skybox.Top = TextureManager.Get("SkyboxTop");
            _skybox.Bottom = TextureManager.Get("SkyboxBottom");
             */

            //Models setup
            _playerCursor = new Static("cursor", "Crosshair_model");
            _playerCursor.StaticModel.Scale = new Vector3(200, 200, 200);
            _cursorAnimationRotation = 0;

            _planet = new StaticModel[2];
            _planet[0] = StaticModelManager.GetModel("Planet_model");
            _planet[0].Scale = new Vector3(15000, 1, 15000);
            _planet[0].Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _planet[0].Position = new Vector3(0, -1000, 0);
            _planet[1] = StaticModelManager.GetModel("Planet_model");
            _planet[1].Scale = _planet[0].Scale;
            _planet[1].Orientation = _planet[0].Orientation;
            _planet[1].Position = new Vector3(0, -1000, _planet[0].Position.Z + _planet[0].Scale.Z);     //Because planet model is a 1x1 square

            _dreadnaught = new Dreadnaught("player");
            _dreadnaught.Position = new Vector3(0, 0, -1400);
            _dreadnaught.Orientation = Geometric.Generate_Quaternion(0, 0, 0, 0);
            _dreadnaught.Scale = new Vector3(10, 10, 10);

            //Setup font
            Renderer.Font = TextManager.GetFont("Calibri");

            // ONCE YOU ARE DONE LOADING, BE SURE TO SET YOUR READY 
            // PROPERTY TO TRUE SO MY ENGINE DOESN'T SCREAM AT YOU
            _isReady = true;

        }

        #endregion

        #region Rendering

        public void Render()
        {
            // RENDER GAME OBJECTS HERE

            //Camera -> IT IS POSSIBLE TO SWITCH CAMERA
            Renderer.CurrentCamera = _camera;

            //Draw Indication for X axis limit
            Renderer.DrawLine(new Vector3(_playerLimitRange, 0, -_spaceLimit.Z), new Vector3(_playerLimitRange, 0, _spaceLimit.Z), new Color(230, 230, 230, 1));
            Renderer.DrawLine(new Vector3(-_playerLimitRange, 0, -_spaceLimit.Z), new Vector3(-_playerLimitRange, 0, _spaceLimit.Z), new Color(230, 230, 230, 1));
            //Draw Indication for Z axis lower limit
            Renderer.DrawLine(new Vector3(_playerLimitRange, 0, -_playerLimitRange), new Vector3(-_playerLimitRange, 0, -_playerLimitRange), new Color(230, 230, 230, 1));

            //Static models
            //Renderer.DrawSkybox(_skybox);             //Removed because no longer used for this kind of game
            Renderer.DrawStaticModel(_planet[0]);
            Renderer.DrawStaticModel(_planet[1]);

            //Dynamic models
            if (!_dreadnaught.IsDead)
            {
                Renderer.DrawStaticModel(_dreadnaught.StaticModel);
            }
            foreach (Asteroid asteroid in _asteroids)
            {
                if (!asteroid.IsDead)
                    Renderer.DrawStaticModel(asteroid.StaticModel);
            }
            foreach (Bullet bullet in _bullets)
            {
                if (!bullet.IsDead)
                    Renderer.DrawLine(bullet.Position, bullet.Position + bullet.Velocity * 600, new Color(100, 255, 0, 1));
            }


            //Cursor
            Renderer.DrawStaticModel(_playerCursor.StaticModel);

            //Texts
            Renderer.RenderText("Score: " + _score, 0.03f, 0.95f, 50f, 0, Color.White);

            Renderer.RenderText("Move Dreadnaught: W, A, S, D", 0.03f, .05f, 30f, 0, Color.Yellow);
            Renderer.RenderText("LeftMouse Button: FIRE Laser Cannons", 0.3f, .05f, 30f, 0, Color.Yellow);
            Renderer.RenderText("RightMouse Button: Set YAW", 0.6f, .05f, 30f, 0, Color.Yellow);
            Renderer.RenderText("ESC: EXIT game", 0.87f, .05f, 30f, 0, Color.Yellow);

            if (_dreadnaught.IsDead)
            {
                Renderer.RenderText("GAME OVER", 0.4f, .5f, 80f, 0, Color.AntiqueWhite);
                Renderer.RenderText("Press SPACEBAR for new game!", 0.405f, .43f, 30f, 0, Color.AntiqueWhite);
            }


            //Drawing Lines for testing..
            //Vector3 targetRelative = Geometric.Quaternion_Rotate(_dreadnaught.Orientation.Inverted(), _playerCursor.Position - _dreadnaught.Position);
            //Renderer.DrawLine(_dreadnaught.Position, targetRelative, Color.Red);
            //Renderer.DrawLine(_playerCursor.Position, new Vector3(0,0,0), Color.White);
            //foreach(SmallCannon can in _dreadnaught.Cannons)
            //  Renderer.DrawLine(can.Position, _playerCursor.Position, new Color(255, 0, 0, 1));
        }

        #endregion

        #region Updating

        public string Update(float elapsedTime)
        {
            // UPDATE GAME STATE HERE

            //Scroll background
            ScrollPlanet();

            //Camera Update
            MoveCamera();
            //_skybox.Position = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z);

            if (!_dreadnaught.IsDead)
            {
                //Dreadnaught Movements and Behaviour Update
                MoveDreadnaught();
                _dreadnaught.UpdateStandard();              //updates animations and timings
                _dreadnaught.AimAt(_playerCursor.Position); //makes weapons to aim to player cursor

                //Spawn new bullets
                if (InputManager.Mouse.LeftClickdown)
                {
                    List<Bullet> incoming = _dreadnaught.Shoot(_playerCursor.Position);
                    if (incoming.Count > 0) foreach (Bullet bullet in incoming) _bullets.Add(bullet);
                }

                //Update where cursor is!
                UpdateCursorPosition();
            }
            else
            {
                if (InputManager.Keyboard.Spacepressed)   //Spacebar to start new game
                {
                    Load();
                }
            }

            //Animate cursor: rotate cursor on itself
            _cursorAnimationRotation += 0.005f;
            if (_cursorAnimationRotation > 2 * Constants.pi_float) _cursorAnimationRotation = 0;
            _playerCursor.OrientationRelative = Geometric.Generate_Quaternion(_cursorAnimationRotation, 0, 1, 0);

            //Apply Physics
            Physics();

            //Spawn new asteroids
            SpawnAsteroids();

            return "Don't Change States";
        }

        #endregion

        #region State Functions

        //COMMANDS
        private void MoveCamera()
        {

            // ORIENTATION
            //Quaternion lookAtCenter = Geometric.FreeLookAt(_camera.Position, new Vector3(0,0,0), new Vector3(0, 1, 0));
            Quaternion lookAtPlayer = Geometric.FreeLookAt(_camera.Position, _dreadnaught.Position, new Vector3(0, 1, 0));
            _camera.Forward = Geometric.Quaternion_Rotate(lookAtPlayer, new Vector3(0, 0, 1));
            _camera.Up = Geometric.Quaternion_Rotate(lookAtPlayer, new Vector3(0, 1, 0));

            // POSITION
            float percentage = -_camera.Forward.Y;
            float newY = ((_maxCamHeight - _minCamHeight) * percentage) + _minCamHeight;

            if (InputManager.Keyboard.Cpressed)      //Change camera mode to free mode!
            {
                _cameraMode += 1;
                if (_cameraMode == 3) _cameraMode = 1;
            }

            switch (_cameraMode)
            {
                default:
                    // ONLY Z POSITION
                    _camera.Position = new Vector3(_camera.Position.X, newY, _camera.Position.Z);

                    break;

                case 1:

                    // ONLY Z POSITION
                    _camera.Position = new Vector3(_camera.Position.X, newY, _camera.Position.Z);

                    break;

                case 2:

                    // Camera Free position movement
                    if (InputManager.Keyboard.Udown)
                        if (InputManager.Keyboard.ShiftLeftdown)
                            _camera.Move(_camera.Down, _camera.PositionSpeed * 100);
                        else
                            _camera.Move(_camera.Down, _camera.PositionSpeed);

                    if (InputManager.Keyboard.Odown)
                        if (InputManager.Keyboard.ShiftLeftdown)
                            _camera.Move(_camera.Up, _camera.PositionSpeed * 100);
                        else
                            _camera.Move(_camera.Up, _camera.PositionSpeed);

                    if (InputManager.Keyboard.Jdown)
                        if (InputManager.Keyboard.ShiftLeftdown)
                            _camera.Move(_camera.Left, _camera.PositionSpeed * 100);
                        else
                            _camera.Move(_camera.Left, _camera.PositionSpeed);

                    if (InputManager.Keyboard.Idown)
                        if (InputManager.Keyboard.ShiftLeftdown)
                            _camera.Move(_camera.Forward, _camera.PositionSpeed * 100);
                        else
                            _camera.Move(_camera.Forward, _camera.PositionSpeed);

                    if (InputManager.Keyboard.Kdown)
                        if (InputManager.Keyboard.ShiftLeftdown)
                            _camera.Move(_camera.Backward, _camera.PositionSpeed * 100);
                        else
                            _camera.Move(_camera.Backward, _camera.PositionSpeed);

                    if (InputManager.Keyboard.Ldown)
                        if (InputManager.Keyboard.ShiftLeftdown)
                            _camera.Move(_camera.Right, _camera.PositionSpeed * 100);
                        else
                            _camera.Move(_camera.Right, _camera.PositionSpeed);

                    // Camera look angle adjustment
                    /*
                    if (InputManager.Keyboard.Kdown)
                        _camera.RotateX(.01f);
                    if (InputManager.Keyboard.Idown)
                        _camera.RotateX(-.01f);
                    if (InputManager.Keyboard.Jdown)
                        _camera.RotateY(.01f);
                    if (InputManager.Keyboard.Ldown)
                        _camera.RotateY(-.01f);
                     * */

                    break;
            }

        }

        private void MoveDreadnaught()
        {

            // ORIENTATION
            if (InputManager.Mouse.RightClickdown)      //Follow cursor position when right click is down or set new orientation if just right click
            {
                _savedPlayerCursorPosition = new Vector3(_playerCursor.Position.X, _playerCursor.Position.Y, _playerCursor.Position.Z);
            }
            _dreadnaught.AlignTo(_savedPlayerCursorPosition);


            // POSITION LIMIT CHECK
            bool commandsEnabled = true;
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
                    repelForce = new Vector3(-_dreadnaught.Position.X, -_dreadnaught.Position.Y, -_dreadnaught.Position.Z);
                    repelForce.NormalizeFast();
                }//If out of bounds but spaceship has changed direction, enable commands (to allow player to get back)
                else
                {
                    commandsEnabled = true;
                }
            }

            // USER COMMANDS CATCH
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

            // POSITION
            command.NormalizeFast();
            command += repelForce;      //apply repelforce (if any)
            Vector3 nextVelocity = _dreadnaught.Velocity + (command * _dreadnaught.MaxAcceleration * Game.DeltaTime);   //SPEED LAW
            if (Math.Abs(nextVelocity.X) > _dreadnaught.MaxSpeed) nextVelocity.X = _dreadnaught.Velocity.X;     //limit max speed on X
            if (Math.Abs(nextVelocity.Z) > _dreadnaught.MaxSpeed) nextVelocity.Z = _dreadnaught.Velocity.Z;     //limit max speed on Z 
            _dreadnaught.Velocity = nextVelocity;   //new speed vector apply
            _dreadnaught.Position = _dreadnaught.Position + _dreadnaught.Velocity;  //POSITION LAW

        }

        //UTILS
        public Ray GetMouseVector()
        {
            // convert the viewport coords to openGL normalized device coords
            float xpos = 2 * ((float)InputManager.Mouse.X / _gameWindow.ScreenWidth) - 1;
            float ypos = 2 * (1 - (float)InputManager.Mouse.Y / _gameWindow.ScreenHeight) - 1;
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
            Vector3 normalized = new Vector3(obj.X, obj.Y, obj.Z);
            normalized.NormalizeFast();
            float norm = obj.LengthFast;
            //Vector3 abs = new Vector3(Math.Abs(obj.X), Math.Abs(obj.Y), Math.Abs(obj.Z));
            if (norm > _playerLimitRange)//- Math.Abs(normalized.Z*500))//+ normalized.X * _playerLimitEccentricityX + normalized.Z * _playerLimitEccentricityZ)
                return true;
            else
                return false;
        }

        //OBJECTS AND PHYSICS
        private void ScrollPlanet()
        {
            _planet[0].Position = new Vector3(_planet[0].Position.X, _planet[0].Position.Y, _planet[0].Position.Z - 1);
            _planet[1].Position = new Vector3(_planet[0].Position.X, _planet[0].Position.Y, _planet[1].Position.Z - 1);
            foreach (StaticModel p in _planet)
            {
                if (p.Position.Z == -p.Scale.Z) p.Position = new Vector3(p.Position.X, p.Position.Y, p.Position.Z + 2 * p.Scale.Z);
            }
        }

        private void SpawnAsteroids()
        {
            Random random = new Random();
            Asteroid spawning;
            float speedMultiplier;

            for (int i = 0; i < _spawnpoint_times.Length; i++) _spawnpoint_times[i] -= Game.DeltaTime * 0.001f;

            if (_num_asteroids_on_screen < _max_asteroids_on_screen)
            {
                int _selectedSpawnPoint = random.Next(0, _spawnpoint_times.Length);
                if (_spawnpoint_times[_selectedSpawnPoint] <= 0)
                {
                    int caseSwitch = random.Next(1, 4);
                    switch (caseSwitch)
                    {
                        //Three different versions of asteroids (different speeds, size and health)
                        case 1:
                            spawning = new Asteroid("asteroid_" + _total_spawned_asteroids, 200);
                            spawning.Scale = new Vector3(80, 80, 80);
                            speedMultiplier = 3;
                            break;
                        case 2:
                            spawning = new Asteroid("asteroid_" + _total_spawned_asteroids, 400);
                            spawning.Scale = new Vector3(140, 140, 140);
                            speedMultiplier = 2;
                            break;
                        case 3:
                            spawning = new Asteroid("asteroid_" + _total_spawned_asteroids, 800);
                            spawning.Scale = new Vector3(200, 200, 200);
                            speedMultiplier = 1;
                            break;
                        default:
                            spawning = new Asteroid("asteroid_" + _total_spawned_asteroids, 200);
                            spawning.Scale = new Vector3(50, 50, 50);
                            speedMultiplier = 3;
                            break;
                    }

                    spawning.Position = _spawnpoints[_selectedSpawnPoint];
                    double angleRatio = random.NextDouble() - 0.5f;
                    float angle = _spawnpoint_angles[_selectedSpawnPoint] + _spawnRange * (float)angleRatio;
                    double speedRatio = random.NextDouble();
                    float speed = ((_maxAsteroidSpeed - _minAsteroidSpeed) * (float)speedRatio + _minAsteroidSpeed) * speedMultiplier;
                    spawning.Velocity = new Vector3(speed * (float)Math.Sin(angle), 0, speed * (float)Math.Cos(angle));

                    _asteroids.Add(spawning);
                    _num_asteroids_on_screen++;
                    _total_spawned_asteroids++;
                    //Output.WriteLine("spawned:" + _total_spawned_asteroids + "from: " + _selectedSpawnPoint);
                    _spawnpoint_times[_selectedSpawnPoint] = _min_spawning_delay * (1 / speedMultiplier);  //if asteroid is small, spawn happens more often!
                }
            }

        }

        private void Physics()
        {

            foreach (Bullet bullet in _bullets)
            {
                if (!bullet.IsDead)
                {
                    foreach (Asteroid asteroid in _asteroids)
                    {
                        bullet.Move();

                        if (IsOutOfView(bullet.Position))
                            bullet.IsDead = true;
                        else if (!asteroid.IsDead && asteroid.HasCollided(bullet.Position))
                        {
                            _score += (ulong)asteroid.Hit(bullet.Health);
                            bullet.IsDead = true;
                            if (asteroid.IsDead) _num_asteroids_on_screen--;
                            //_bullets.Remove(bullet);  //CAN'T DO IN CURRENTLY SCROLLING LIST!!
                        }
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
                    {
                        asteroid.IsDead = true;
                        _num_asteroids_on_screen--;
                    }
                    else
                    {

                        if (asteroid.HasCollided(_dreadnaught.BoundingBox))
                        {
                            _dreadnaught.IsDead = true;
                        }


                        foreach (Asteroid other_asteroid in _asteroids)
                        {
                            if (!other_asteroid.IsDead && asteroid.HasCollided(other_asteroid.BoundingBox) && asteroid.Id != other_asteroid.Id)
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

            /*
            //Flush lists for dead objects (after it reaches a custom limit so I don't need to flush it each cycle)
            if (_asteroids.Count > 100) _asteroids.RemoveAll(item => item.IsDead == true);
            if (_bullets.Count > 50) _bullets.RemoveAll(item => item.IsDead == true);
            //ATTENTION: OBJECTS IN LIST DO NOT CORRESPOND TO ALIVE OBJECTS!! IT ONLY CONTAINS REFERENCES TO DEAD AND ALIVE OBJECTS IN MEMORY!
            //Alternative to flushing lists can be iterating within the for BACKWARDS!!
            */

        }


        #endregion
    }
}