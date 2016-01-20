// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com -- for Seven library and importer
// Last Edited: 08-09-14

using System;
using System.IO;
using SevenEngine;
using SevenEngine.Imaging;
using SevenEngine.Shaders;

using OpenTK.Graphics.OpenGL;
using SevenEngine.Physics.Primitives;
using OpenTK;
using AsTKoids.Objects.Types.Behaviours;
using SevenEngine.StaticModels;
using AsTKoids.Objects;
using AsTKoids.States;
using System.Collections.Generic;
using Seven.Mathematics;
 

namespace AsTKoids
{
  public class Game : SevenEngineWindow
  {
    public Game(int width, int height) : base(width, height) { }

    #region Initialization

    public override void InitializeDisplay()
    {
      // SET INITIAL DISPLAY SETTINGS
      GraphicsSettingsManager.BackFaceCulling = true;
      GraphicsSettingsManager.DepthBuffer = true;
      GraphicsSettingsManager.VerticalSyncronization = false;
      GraphicsSettingsManager.ClearColor = Color.Black;
      GraphicsSettingsManager.Texture2D = true;
      GraphicsSettingsManager.Blend = true;
      GraphicsSettingsManager.SetAlphaBlending();
      GraphicsSettingsManager.Lighting = true;
    }

    public override void InitializeSounds()
    {
      // LOAD SOUNDS
      Output.WriteLine("No sound effects currently loaded.");
    }

    public override void InitializeTextures()
    {

      // Textures for models
      TextureManager.LoadTexture("Planet_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Planet_texture.bmp"));
      TextureManager.LoadTexture("Dreadnaught_front_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Dreadnaught_front_texture.bmp"));
      TextureManager.LoadTexture("Dreadnaught_back_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Dreadnaught_back_texture.bmp"));
      TextureManager.LoadTexture("Dreadnaught_ring_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Dreadnaught_ring_texture.bmp"));
      TextureManager.LoadTexture("Cannon_small_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Cannon_small_darker_texture.bmp"));
      TextureManager.LoadTexture("Asteroid_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Asteroid_texture.bmp"));
      TextureManager.LoadTexture("Red_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Red_texture.bmp"));

      // Textures for skybox (removed because useless in this game - ALERT BottomTexture is not working, check SkyBox.cs in Engine)
      //TextureManager.LoadTexture("SkyboxLeft", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\"));
      //TextureManager.LoadTexture("SkyboxRight", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\"));
      //TextureManager.LoadTexture("SkyboxFront", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\"));
      //TextureManager.LoadTexture("SkyboxBack", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\"));
      //TextureManager.LoadTexture("SkyboxTop", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\"));
      //TextureManager.LoadTexture("SkyboxBottom", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\"));
    }

    public override void InitializeFonts()
    {
      // LOAD Fonts HERE
      TextManager.LoadFontFile("Calibri", FilePath.FromRelative(@"\..\..\Assets\Texts\Ubuntu.fnt"), FilePath.FromRelative(@"\..\..\Assets\Texts\"));
    }

    public override void InitializeModels()
    {
      // LOAD MODELS

      // MESHES
      // Meshes are parts of a static model that have the same texture. do not have transformations.
      // They must be in a StaticModel to be rendered.
      StaticModelManager.LoadMesh("Planet_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Planet_mesh.obj"));
      StaticModelManager.LoadMesh("Dreadnaught_front_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Dreadnaught_front_mesh.obj"));
      StaticModelManager.LoadMesh("Dreadnaught_back_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Dreadnaught_back_mesh.obj"));
      StaticModelManager.LoadMesh("Dreadnaught_ring_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Dreadnaught_ring_mesh.obj"));
      StaticModelManager.LoadMesh("Cannon_small_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Cannon_small_mesh.obj"));
      StaticModelManager.LoadMesh("Asteroid_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Asteroid_mesh.obj"));
      StaticModelManager.LoadMesh("Crosshair_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Crosshair_mesh.obj"));

      // MODELS
      // Forming the static models out of the meshes and textures
      // Static models represent a collection of static meshes that all have the same transformational values.
      StaticModelManager.LoadModel("Planet_model", new string[] { "Planet" }, new string[] { "Planet_mesh" }, new string[] { "Planet_texture" });
      StaticModelManager.LoadModel("Dreadnaught_front_model", new string[] { "Dreadnaught" }, new string[] { "Dreadnaught_front_mesh" }, new string[] { "Dreadnaught_front_texture" });
      StaticModelManager.LoadModel("Dreadnaught_back_model", new string[] { "Dreadnaught" }, new string[] { "Dreadnaught_back_mesh" }, new string[] { "Dreadnaught_back_texture" });
      StaticModelManager.LoadModel("Dreadnaught_ring_model", new string[] { "Dreadnaught" }, new string[] { "Dreadnaught_ring_mesh" }, new string[] { "Dreadnaught_ring_texture" });
      StaticModelManager.LoadModel("Cannon_small_model", new string[] { "Cannon" }, new string[] { "Cannon_small_mesh" }, new string[] { "Cannon_small_texture" });
      StaticModelManager.LoadModel("Asteroid_model", new string[] { "Asteroid" }, new string[] { "Asteroid_mesh" }, new string[] { "Asteroid_texture" });
      StaticModelManager.LoadModel("Crosshair_model", new string[] { "Crosshair" }, new string[] { "Crosshair_mesh" }, new string[] { "Red_texture" });
    }

    public override void InitializeShading()
    {
      //LOAD LIGHTING
      GL.ShadeModel(ShadingModel.Flat);
      GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0.0f, 1000.0f, 0.0f, 1.0f });

      Renderer.DefaultShaderProgram = ShaderManager.DefaultShader;
    }

    public override void InitializeStates()
    {
        // LOAD THE GAME STATES HERE
        // Use the static class "StateManager"
        // EXAMPLES:
        // StateManager.AddState(new YourStateClass("nameOfState"));
        // StateManager.StateManager.TriggerStateLoad("nameOfState");
        // StateManager.ChangeState("nameOfState");

        //StateManager.AddState(new GameState("gameState"));
        StateManager.AddState(new Survival_AsteroidField("Survival_AsteroidField",this));
        // The following line calls the "Load" function of your state.
        // The state must be loaded before you make it the current state.
        StateManager.TriggerStateLoad("Survival_AsteroidField");
        StateManager.ChangeState("Survival_AsteroidField");

    }

    #endregion

    public override void Update(double elapsedTime)
    {
        // DO NOT UPDATE LOW LEVEL GAME LOGIC HERE!!!
        // Only change states as need be with the static "StateManager" class.
        // EXAMPLES:
        // string stateStatus = StateManager.Update((float)elapsedTime);
        // if (stateStatus == "menuState")
        //  StateManager.ChangeState("menuState");

        // NOTE: DO NOT alter this function unless you fully understand it
        string stateStatus = StateManager.Update((float)elapsedTime);
    }

  }
}