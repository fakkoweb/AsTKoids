﻿using System;
using System.IO;
using SevenEngine;
using SevenEngine.Imaging;
using Seven.Structures;
using SevenEngine.Shaders;
using Game.States;

using OpenTK.Graphics.OpenGL;
using SevenEngine.Physics.Primitives;
using OpenTK;
using Seven.Mathematics;

namespace Game
{
  // This is an example of how to use my engine. 
  // Read this file and the "GameState.cs" file within the "State" folder.
  // Hope you enjoy using my engine :)
  public class Game : SevenEngineWindow
  {
    public Game(int width, int height) : base(width, height) { }

    public override void InitializeDisplay()
    {
      // SET INITIAL DISPLAY SETTINGS HERE.
      // Use the static class "GraphicsSettingsManager"
      // EXAMPLES:
        // GraphicsSettingsManager.SettingToChange = newValue;
      //GraphicsSettingsManager.Fullscreen = true;
      GraphicsSettingsManager.BackFaceCulling = true;
      GraphicsSettingsManager.DepthBuffer = true;
      GraphicsSettingsManager.VerticalSyncronization = true;
      GraphicsSettingsManager.ClearColor = Color.DEFAULT;
      GraphicsSettingsManager.Texture2D = true;
      GraphicsSettingsManager.Blend = true;
      GraphicsSettingsManager.SetAlphaBlending();
      GraphicsSettingsManager.Lighting = true;
    }

    public override void InitializeSounds()
    {
      // LOAD SOUNDS HERE.
      // Use the static class "SoundManager"

      // Just keep this function here. I haven't finished the SoundManager class yet...
      Output.WriteLine("No sound effects currently loaded.");
    }

    public override void InitializeTextures()
    {
      // LOAD TEXTURES HERE.
      // Use the static class "TextureManager"
      // Supported file types: bmp, jpeg, png, gif, ttf
      // EXAMPLES:
        // TextureManager.LoadTexture("nameOfTexture", "filePath");
      // NOTE: If you use my static "FilePath" class the directory should be cross platform

      // Textures for models
      TextureManager.LoadTexture("grass", FilePath.FromRelative(@"\..\..\Assets\Textures\grass.bmp"));
      TextureManager.LoadTexture("rock", FilePath.FromRelative(@"\..\..\Assets\Textures\rock3.bmp"));
      TextureManager.LoadTexture("rock2", FilePath.FromRelative(@"\..\..\Assets\Textures\rock4.bmp"));
      TextureManager.LoadTexture("RedRanger", FilePath.FromRelative(@"\..\..\Assets\Textures\RedRangerBody.bmp"));
      TextureManager.LoadTexture("Tux", FilePath.FromRelative(@"\..\..\Assets\Textures\tux.bmp"));
      TextureManager.LoadTexture("TuxRed", FilePath.FromRelative(@"\..\..\Assets\Textures\tuxRed.bmp"));
      TextureManager.LoadTexture("TuxGreen", FilePath.FromRelative(@"\..\..\Assets\Textures\tuxGreen.bmp"));
      TextureManager.LoadTexture("BlueRanger", FilePath.FromRelative(@"\..\..\Assets\Textures\BlueRangerBody.bmp"));
      TextureManager.LoadTexture("PinkRanger", FilePath.FromRelative(@"\..\..\Assets\Textures\PinkRangerBody.bmp"));
      TextureManager.LoadTexture("BlackRanger", FilePath.FromRelative(@"\..\..\Assets\Textures\BlackRangerBody.bmp"));
      TextureManager.LoadTexture("YellowRanger", FilePath.FromRelative(@"\..\..\Assets\Textures\YellowRangerBody.bmp"));
      TextureManager.LoadTexture("MushroomCloud", FilePath.FromRelative(@"\..\..\Assets\Textures\MushCloud.bmp"));

      TextureManager.LoadTexture("Dreadnaught_front_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Dreadnaught_front_texture.bmp"));
      TextureManager.LoadTexture("Cannon_small_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Cannon_small_texture.bmp"));
      TextureManager.LoadTexture("Cannon_small_base_texture", FilePath.FromRelative(@"\..\..\Assets\Textures\Cannon_small_base_texture.bmp"));

      // Textures for menus
      TextureManager.LoadTexture("Menu", FilePath.FromRelative(@"\..\..\Assets\Textures\Menu.bmp"));

      // Textures for skybox
      TextureManager.LoadTexture("SkyboxLeft", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\NightWalker\NightWalkerLeft.bmp"));
      TextureManager.LoadTexture("SkyboxRight", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\NightWalker\NightWalkerRight.bmp"));
      TextureManager.LoadTexture("SkyboxFront", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\NightWalker\NightWalkerFront.bmp"));
      TextureManager.LoadTexture("SkyboxBack", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\NightWalker\NightWalkerBack.bmp"));
      TextureManager.LoadTexture("SkyboxTop", FilePath.FromRelative(@"\..\..\Assets\Textures\SkyBoxes\NightWalker\NightWalkerTop.bmp"));
    }

    public override void InitializeFonts()
    {
      // LOAD Fonts HERE.
      // Use the static class "TextManager"
      // Supported file types: fnt
      // NOTE: the image files used by the fnt files must be supported by my image importer
      // EXAMPLES:
        // TextManager.LoadFontFile("nameOfFont", "filePathToFont", "filePathToFontTextures");
        // Renderer.Font = TextManager.Get("nameOfFont");
      // NOTE: If you use my static "FilePath" class the directory should be cross platform
      TextManager.LoadFontFile("Calibri", FilePath.FromRelative(@"\..\..\Assets\Texts\Calibri2.fnt"), FilePath.FromRelative(@"\..\..\Assets\Texts\"));
    }

    public override void InitializeModels()
    {
      // LOAD MODEL FILES HERE.
      // Use the static class "StaticModelManager"
      // Supported file types: obj
      // NOTE: I only support obj file with single objects at the moment, please export each object separately
      // NOTE: I currently do not support materials

      // I WILL BE CHANGING THESE FUNCTIONS AROUND SOON (ONCE I GET A FULLY FEATURED OBJ IMPORTER WORKING),
        // BUT HERE IS THER CURRENT TWO FUNCTIONS YOU SHOULD USE...
      // EXAMPLES:
        // StaticModelManager.LoadMesh("meshName", "filePath");
        // string[] textures; string[] meshes; string[] meshNamesRelativeToTheModel;
        // StaticModelManager.LoadModel("modelName", textures, meshes, meshNamesRelativeToTheModel);
      // NOTE: If you use my static "FilePath" class the directory should be cross platform

      // Loading the meshes
      // Meshes are parts of a static model that have the same texture. You cannot render static 
      //   meshes because they do not have transformations. Put them in a static model to render them.
      StaticModelManager.LoadMesh("terrain", FilePath.FromRelative(@"\..\..\Assets\Models\Terrain.obj"));
      StaticModelManager.LoadMesh("RedRanger", FilePath.FromRelative(@"\..\..\Assets\Models\RedRanger.obj"));
      StaticModelManager.LoadMesh("BlackRanger", FilePath.FromRelative(@"\..\..\Assets\Models\RedRanger.obj"));
      StaticModelManager.LoadMesh("BlueRanger", FilePath.FromRelative(@"\..\..\Assets\Models\RedRanger.obj"));
      StaticModelManager.LoadMesh("YellowRanger", FilePath.FromRelative(@"\..\..\Assets\Models\RedRanger.obj"));
      StaticModelManager.LoadMesh("PinkRanger", FilePath.FromRelative(@"\..\..\Assets\Models\RedRanger.obj"));
      StaticModelManager.LoadMesh("Tux", FilePath.FromRelative(@"\..\..\Assets\Models\tux.obj"));
      StaticModelManager.LoadMesh("TuxRed", FilePath.FromRelative(@"\..\..\Assets\Models\tux.obj"));
      StaticModelManager.LoadMesh("TuxGreen", FilePath.FromRelative(@"\..\..\Assets\Models\tux.obj"));
      StaticModelManager.LoadMesh("mountain", FilePath.FromRelative(@"\..\..\Assets\Models\mountain.obj"));
      StaticModelManager.LoadMesh("MushroomCloud", FilePath.FromRelative(@"\..\..\Assets\Models\MushCloud.obj"));

      StaticModelManager.LoadMesh("Dreadnaught_front_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Dreadnaught_front_mesh.obj"));
      StaticModelManager.LoadMesh("Cannon_small_base_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Cannon_small_base_mesh.obj"));
      StaticModelManager.LoadMesh("Cannon_small_mesh", FilePath.FromRelative(@"\..\..\Assets\Models\Cannon_small_mesh.obj"));

      // Forming the static models out of the meshes and textures
      // Static models represent a collection of static meshes that all have the same transformational values.
      StaticModelManager.LoadModel("MushroomCloud", new string[] { "MushroomCloud" }, new string[] { "MushroomCloud" }, new string[] { "MushroomCloud" });
      StaticModelManager.LoadModel("Terrain", new string[] { "Terrain" }, new string[] { "terrain" }, new string[] { "grass" });
      StaticModelManager.LoadModel("Mountain", new string[] { "mountain" }, new string[] { "mountain" }, new string[] { "rock" });
      StaticModelManager.LoadModel("Mountain2", new string[] { "mountain" }, new string[] { "mountain" }, new string[] { "rock2" });

      StaticModelManager.LoadModel("Tux", new string[] { "Body" }, new string[] { "Tux" }, new string[] { "Tux" });
      StaticModelManager.LoadModel("TuxRed", new string[] { "Body" }, new string[] { "TuxRed" }, new string[] { "TuxRed" });
      StaticModelManager.LoadModel("TuxGreen", new string[] { "Body" }, new string[] { "TuxGreen" }, new string[] { "TuxGreen" });

      StaticModelManager.LoadModel("RedRanger", new string[] { "Body" }, new string[] { "RedRanger" }, new string[] { "RedRanger" });
      StaticModelManager.LoadModel("BlueRanger", new string[] { "Body" }, new string[] { "BlueRanger" }, new string[] { "BlueRanger" });
      StaticModelManager.LoadModel("BlackRanger", new string[] { "Body" }, new string[] { "BlackRanger" }, new string[] { "BlackRanger" });
      StaticModelManager.LoadModel("PinkRanger", new string[] { "Body" }, new string[] { "PinkRanger" }, new string[] { "PinkRanger" });
      StaticModelManager.LoadModel("YellowRanger", new string[] { "Body" }, new string[] { "YellowRanger" }, new string[] { "YellowRanger" });

      StaticModelManager.LoadMesh("Dodecahedron", FilePath.FromRelative(@"\..\..\Assets\Models\Dodecahedron.obj"));
      StaticModelManager.LoadModel("Dodecahedron", new string[] { "Body" }, new string[] { "Dodecahedron" }, new string[] { "rock2" });

      StaticModelManager.LoadModel("Dreadnaught_front_model", new string[] { "Body" }, new string[] { "Dreadnaught_front_mesh" }, new string[] { "Dreadnaught_front_texture" });
      StaticModelManager.LoadModel("Cannon_small_model", new string[] { "Body" }, new string[] { "Cannon_small_mesh" }, new string[] { "Cannon_small_texture" });
      StaticModelManager.LoadModel("Cannon_small_base_model", new string[] { "Body" }, new string[] { "Cannon_small_base_mesh" }, new string[] { "Cannon_small_base_texture" });
    }

    public override void InitializeShaders()
    {
      // LOAD SHADER FILES HERE.
      // Use the static class "ShaderManager"
      // Supported file types: glsl
      // EXAMPLES:
        // ShaderManager.LoadVertexShader("vertexShaderName", FilePath.FromRelative((@"filePathToVertexShader"));
        // ShaderManager.LoadFragmentShader("fragmentShaderName", FilePath.FromRelative((@"filePathToFragmentShader"));
        // ShaderManager.LoadGeometryShader("geometryShaderName", FilePath.FromRelative((@"filePathToGeometryShader"));
        // ShaderManager.LoadExtendedGeometryShader("extendedGeometryShaderName", FilePath.FromRelative((@"filePathToExtendedGeometryShader"));
        // ShaderManager.MakeShaderProgram("shaderProgramName", "vertexShaderName", "fragmentShaderName", "geometryShaderName", "extendedGeometryShaderName");
        // // NOTE: PARAMETERS TO THE "MakeShaderProgram()" METHOD MAY BE "null" IF YOU AREN'T USING THOSE SHADERS
        // ShaderManager.SetActiveShader("shaderProgramName");
      // NOTE: If you use my static "FilePath" class the directory should be cross platform

      // These basic shaders do not include lighting effects.
      //ShaderManager.LoadVertexShader("VertexShaderBasic", FilePath.FromRelative(@"\..\..\Assets\Shaders\Vertex\VertexShaderBasic.glsl"));
      //ShaderManager.LoadFragmentShader("FragmentShaderBasic", FilePath.FromRelative(@"\..\..\Assets\Shaders\Fragment\FragmentShaderBasic.glsl"));
      //ShaderManager.MakeShaderProgram("ShaderProgramBasic", "VertexShaderBasic", "FragmentShaderBasic", null, null);

      //ShaderManager.LoadVertexShader("VertexShaderLight", FilePath.FromRelative(@"\..\..\Assets\Shaders\Vertex\VertexShaderLight2.glsl"));
      //ShaderManager.LoadFragmentShader("FragmentShaderLight", FilePath.FromRelative(@"\..\..\Assets\Shaders\Fragment\FragmentShaderLight2.glsl"));
      //ShaderManager.MakeShaderProgram("ShaderProgramLight", "VertexShaderLight", "FragmentShaderLight", null, null);

      //ShaderManager.SetActiveShader("ShaderProgramBasic");

      // This is really hack-y, I will be editing this soon.
      
      GL.ShadeModel(ShadingModel.Flat);
      GL.Material(MaterialFace.Front, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
      GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 800.0f);
      GL.Material(MaterialFace.Front, MaterialParameter.Ambient, new float[] { 0.4f, 0.4f, 0.4f, 1.0f });
      GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, new float[] { .8f, .8f, 0.8f, 1.0f });
      GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0.0f, 1000.0f, 0.0f, 1.0f });
      
      
      Renderer.DefaultShaderProgram = ShaderManager.LightShader;
    }

    public override void InitializeStates()
    {

      // LOAD THE GAME STATES HERE
      // Use the static class "StateManager"
      // EXAMPLES:
        // StateManager.AddState(new YourStateClass("nameOfState"));
        // StateManager.StateManager.TriggerStateLoad("nameOfState");
        // StateManager.ChangeState("nameOfState");

        /*
        StateManager.AddState(new GameState("gameState"));
        StateManager.TriggerStateLoad("gameState");
        StateManager.ChangeState("gameState");
        */

        
        StateManager.AddState(new ScoutingState("AiBattle",this));
        // The following line calls the "Load" function of your state.
        // The state must be loaded before you make it the current state.
        StateManager.TriggerStateLoad("AiBattle");
        StateManager.ChangeState("AiBattle");
        

        // Try this state for sprite usage example
        /*
        StateManager.AddState(new SpriteState("SpriteState"));
        StateManager.TriggerStateLoad("SpriteState");
        StateManager.ChangeState("SpriteState");
        */
        /*
        StateManager.AddState(new Scouting("SpriteState"));
        StateManager.TriggerStateLoad("SpriteState");
        StateManager.ChangeState("SpriteState");
        */
    }

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