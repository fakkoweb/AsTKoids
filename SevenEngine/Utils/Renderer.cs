// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com -- for Seven library and importer
// Last Edited: 08-09-14

using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SevenEngine;
using Seven.Structures;
using SevenEngine.Imaging;
using SevenEngine.Texts;
using SevenEngine.StaticModels;
using Seven.Mathematics;
using SevenEngine.Shaders;

namespace SevenEngine
{
  /// <summary>This class handles all rendering. Each call to a function is one shape/mesh to be designed on screen.</summary>
  public static class Renderer
  {
    #region Shading

    private static ShaderProgram _defaultShaderProgram;

    /// <summary>The default shader that will be used unless the item has a shader override.</summary>
    public static ShaderProgram DefaultShaderProgram { get { return _defaultShaderProgram; } set { _defaultShaderProgram = value; } }

    #endregion

    #region Transformations
    
    private static Matrix4 _projectionMatrix;

    public static Matrix4 MatrixProjection { get { return _projectionMatrix; } set { _projectionMatrix = value; } }

    private static Camera _currentCamera;

    // I will change this class in general is a short term fix. will probably use the renderer to store the transformations.
    private static int _screenHeight = 600;
    private static int _screenWidth = 800;

    public static Camera CurrentCamera { get { return _currentCamera; } set { _currentCamera = value; } }

    internal static int ScreenWidth
    {
      get { return _screenWidth; }
      set
      {
        _screenWidth = value;
      }
    }

    internal static int ScreenHeight
    {
      get { return _screenHeight; }
      set
      {
        _screenHeight = value;
      }
    }

    public static void SetViewport(int x, int y, int width, int height)
    {
      GL.Viewport(x, y, width, height);
    }

    public static void SetOrthographicMatrix()
    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      float halfWidth = _screenWidth / 2;
      float halfHeight = _screenHeight / 2;
      GL.Ortho(-halfWidth, halfWidth, -halfHeight, halfHeight, -1000, 1000);
    }

    public static Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(
        (float)_currentCamera.FieldOfView,
        (float)_screenWidth / (float)_screenHeight,
        (float)_currentCamera.NearClipPlane,
        (float)_currentCamera.FarClipPlane);
    }

    public static void SetProjectionMatrix()
    {
      // This creates a projection matrix that transforms objects due to depth (applies depth perception)
      GL.MatrixMode(MatrixMode.Projection);
      //GL.LoadIdentity(); // but i must use LoadMatrix() just after it
      Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(
        (float)_currentCamera.FieldOfView, 
        (float)_screenWidth / (float)_screenHeight, 
        (float)_currentCamera.NearClipPlane, 
        (float)_currentCamera.FarClipPlane);
      _projectionMatrix = perspective;
      GL.LoadMatrix(ref perspective);
    }

    #endregion

    #region Drawing Tools

    public static void DrawLine(Vector3 from, Vector3 to, Color color)
    {
      SetProjectionMatrix();
      GL.MatrixMode(MatrixMode.Modelview);
      Matrix4 camera = _currentCamera.GetMatrix();
      GL.LoadMatrix(ref camera);

      GL.UseProgram(ShaderManager.ColorShader.GpuHandle);
      int uniformLocation = GL.GetUniformLocation(ShaderManager.ColorShader.GpuHandle, "color");
      GL.Uniform4(uniformLocation, new Color4(color.R / 255f, color.G / 255f, color.B / 255f, 1));//color.A / 255f));

      //GL.Begin(PrimitiveType.Lines);
      GL.Begin(BeginMode.Lines);
      GL.Vertex3(from.X, from.Y, from.Z);
      GL.Vertex3(to.X, to.Y, to.Z);
      GL.Color3(color.R / 255f, color.G / 255f, color.B / 255f);
      GL.End();
    }

    #endregion

    #region Text

    private static Font _font;

    /// <summary>The current font that will be used when rendering text.</summary>
    public static Font Font { get { return _font; } set { _font = value; } }

    public static void RenderText(string message, float x, float y, float scale, float rotation, Color color)
    {
      // Apply the 2D orthographic matrix transformation
      SetOrthographicMatrix();

      rotation = rotation * 180f / Constants.pi_float;

      // Set the text shader program and pass in the color as a parameter
      GL.UseProgram(ShaderManager.TextShader.GpuHandle);
      int uniformLocation = GL.GetUniformLocation(ShaderManager.TextShader.GpuHandle, "color");
      GL.Uniform4(uniformLocation, new Color4(color.R / 255f, color.G/ 255f, color.B / 255f, color.A / 255f));

      // Apply the model view matrix transformations
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
      GL.Translate((x * _screenWidth - _screenWidth / 2f), (y * _screenHeight - _screenHeight / 2f), 0f);
      if (rotation != 0)
        GL.Rotate(rotation, 0, 0, 1);

      // Set up the verteces (hardware instanced for all character sprites)
      GL.BindBuffer(BufferTarget.ArrayBuffer, CharacterSprite.GpuVertexBufferHandle);
      GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero);
      GL.EnableClientState(ArrayCap.VertexArray);

      // this standardizes the size (not depedent on sprite sheet size)
      float sizeRatio = scale / (float)_font.LineHeight;

      for (int i = 0; i < message.Length; i++)
      {
        CharacterSprite sprite = _font.Get(message[i]);

        // Calculate the per-character transformational values
        float ySize = sizeRatio * (float)sprite.OriginalHeight;
        float yOffset = sizeRatio * (float)sprite.YOffset;
        float xSize = sizeRatio * (float)sprite.OriginalWidth;
        float xRatio = xSize / (float)(sprite.OriginalWidth);
        float xOffset = xRatio * (float)sprite.XOffset;
        float xAdvance = xRatio * (float)sprite.XAdvance;
        // Kearning (extra adjustments between specific charasters)
        if (i + 1 < message.Length)
          xAdvance += xRatio * (float)sprite.CheckKearning(message[i + 1]);

        // Apply the character offsets and scaling
        GL.Translate(xOffset, -yOffset, 0f);
        GL.Scale(xSize, ySize, 0f);

        // Bind the texture and set up the texture coordinates
        GL.BindTexture(TextureTarget.Texture2D, sprite.Texture.GpuHandle);
        GL.BindBuffer(BufferTarget.ArrayBuffer, sprite.GPUTextureCoordinateBufferHandle);
        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero);
        GL.EnableClientState(ArrayCap.TextureCoordArray);

        // Perform the render
        //GL.DrawArrays(PrimitiveType.Triangles, 0, sprite.VertexCount);
        GL.DrawArrays(BeginMode.Triangles, 0, sprite.VertexCount);

        // Remove the per character transforms and advance to the next charachers position
        GL.Scale(1 / xSize, 1 / ySize, 0f);
        GL.Translate(-xOffset + xAdvance, yOffset, 0f);
      }
    }

    #endregion

    #region Skybox

    public static void DrawSkybox(SkyBox skybox)
    {
      // Apply the 3D projection matrix transformation
      SetProjectionMatrix();

      //if (skybox.ShaderOverride != null)
      //  GL.UseProgram(_defaultShaderProgram.GpuHandle);
      //else
      //  GL.UseProgram(ShaderManager.DefaultShader.GpuHandle);
      GL.UseProgram(_defaultShaderProgram.GpuHandle);

      // Apply the model view matrix transformations
      GL.MatrixMode(MatrixMode.Modelview);
      // Apply the camera transformation
      Matrix4 cameraTransform = _currentCamera.GetMatrix();
      GL.LoadMatrix(ref cameraTransform);
      // Apply the world transformation
      GL.Translate(skybox.Position.X, skybox.Position.Y, skybox.Position.Z);
      GL.Scale(skybox.Scale.X, skybox.Scale.Y, skybox.Scale.Z);
      
      // Set up verteces
      GL.BindBuffer(BufferTarget.ArrayBuffer, skybox.GpuVertexBufferHandle);
      GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero);
      GL.EnableClientState(ArrayCap.VertexArray);

      // Select texture and set up texture coordinates
      GL.BindBuffer(BufferTarget.ArrayBuffer, skybox.GPUTextureCoordinateBufferHandle);
      GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero);
      GL.EnableClientState(ArrayCap.TextureCoordArray);

      // Render left side of skybox
      GL.BindTexture(TextureTarget.Texture2D, skybox.Left.GpuHandle);
      //GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
      GL.DrawArrays(BeginMode.Triangles, 0, 6);

      // Render front side of skybox
      GL.BindTexture(TextureTarget.Texture2D, skybox.Front.GpuHandle);
      //GL.DrawArrays(PrimitiveType.Triangles, 6, 6);
      GL.DrawArrays(BeginMode.Triangles, 6, 6);

      // Render right side of skybox
      GL.BindTexture(TextureTarget.Texture2D, skybox.Right.GpuHandle);
      //GL.DrawArrays(PrimitiveType.Triangles, 12, 6);
      GL.DrawArrays(BeginMode.Triangles, 12, 6);

      // Render back side of skybox
      GL.BindTexture(TextureTarget.Texture2D, skybox.Back.GpuHandle);
      //GL.DrawArrays(PrimitiveType.Triangles, 18, 6);
      GL.DrawArrays(BeginMode.Triangles, 18, 6);

      // Render top side of skybox
      GL.BindTexture(TextureTarget.Texture2D, skybox.Top.GpuHandle);
      //GL.DrawArrays(PrimitiveType.Triangles, 24, 6);
      GL.DrawArrays(BeginMode.Triangles, 24, 6);

      // Render bottom side of skybox
      //GL.BindTexture(TextureTarget.Texture2D, skybox.Bottom.GpuHandle);
      //GL.DrawArrays(PrimitiveType.Triangles, 30, 6);
      //GL.DrawArrays(BeginMode.Triangles, 30, 6);
    }

    #endregion

    #region StaticModel

    /// <summary>Renders a single static model using "GL.DrawArrays()". Renders also all its children submodels if any.</summary>
    /// <param name="staticModel">The mesh to be rendered.</param>
    public static void DrawStaticModel(StaticModel staticModel)
    {
        // Apply the 3D projection matrix transformations
        SetProjectionMatrix();

        // Use
        GL.UseProgram(DefaultShaderProgram.GpuHandle);

        // Apply the model view matrix transformations
        GL.MatrixMode(MatrixMode.Modelview);
        // Apply the camera transformation
        Matrix4 cameraTransform = _currentCamera.GetMatrix();
        GL.LoadMatrix(ref cameraTransform);

        // Apply the world transformation
        GL.Translate(staticModel.Position.X, staticModel.Position.Y, staticModel.Position.Z);
        Matrix4 glFriendly = Matrix4.CreateFromQuaternion( staticModel.Orientation );
        GL.MultMatrix(ref glFriendly);
        //GL.Rotate(staticModel.Orientation.W * 180f / 3.14f, staticModel.Orientation.X, staticModel.Orientation.Y, staticModel.Orientation.Z);
        GL.Scale(staticModel.Scale.X, staticModel.Scale.Y, staticModel.Scale.Z);

        // Call the drawing functions for each mesh within the model
        staticModel.Meshes.Foreach((Foreach<StaticMesh>)DrawStaticModelPart);

        // Call the drawing function recursively for each model parented to this model
        if (staticModel.HasChildren)
        {
            foreach(StaticModel childModel in staticModel.ChildrenModels)
            {
                DrawStaticModel(childModel);
            }
        }
        
    }

    private static void DrawStaticModelPart(StaticMesh staticMesh)
    {
      // Make sure something will render
      if (staticMesh.VertexBufferHandle == 0 ||
        (staticMesh.ColorBufferHandle == 0 && staticMesh.TextureCoordinateBufferHandle == 0))
        return;

      // Push current Array Buffer state so we can restore it later
      GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

      if (GL.IsEnabled(EnableCap.Lighting))
      {
        // Normal Array Buffer
        if (staticMesh.NormalBufferHandle != 0)
        {
          // Set up normals
          GL.BindBuffer(BufferTarget.ArrayBuffer, staticMesh.NormalBufferHandle);
          GL.NormalPointer(NormalPointerType.Float, 0, IntPtr.Zero);
          GL.EnableClientState(ArrayCap.NormalArray);
        }
      }
      else
      {
        // Color Array Buffer
        if (staticMesh.ColorBufferHandle != 0)
        {
          // Set up colors
          GL.BindBuffer(BufferTarget.ArrayBuffer, staticMesh.ColorBufferHandle);
          GL.ColorPointer(3, ColorPointerType.Float, 0, IntPtr.Zero);
          GL.EnableClientState(ArrayCap.ColorArray);
        }
      }

      // Texture Array Buffer
      if (GL.IsEnabled(EnableCap.Texture2D) && staticMesh.TextureCoordinateBufferHandle != 0)
      {
        // Select the texture and set up texture coordinates
        GL.BindTexture(TextureTarget.Texture2D, staticMesh.Texture.GpuHandle);
        GL.BindBuffer(BufferTarget.ArrayBuffer, staticMesh.TextureCoordinateBufferHandle);
        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, IntPtr.Zero);
        GL.EnableClientState(ArrayCap.TextureCoordArray);
      }
      else
        // Nothing will render if this branching is reached.
        return;

      // Set up verteces
      GL.BindBuffer(BufferTarget.ArrayBuffer, staticMesh.VertexBufferHandle);
      GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero);
      GL.EnableClientState(ArrayCap.VertexArray);

      if (staticMesh.ElementBufferHandle != 0)
      {
        // Set up indeces
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, staticMesh.ElementBufferHandle);
        GL.IndexPointer(IndexPointerType.Int, 0, IntPtr.Zero);
        GL.EnableClientState(ArrayCap.IndexArray);
        
        // Ready to render using an index buffer
        int elements = 0;
        //GL.DrawElements(PrimitiveType.Triangles, staticMesh.VertexCount, DrawElementsType.UnsignedInt, ref elements);
        GL.DrawElements(BeginMode.Triangles, staticMesh.VertexCount, DrawElementsType.UnsignedInt, ref elements);
      }
      else
        // Ready to render
        //GL.DrawArrays(PrimitiveType.Triangles, 0, staticMesh.VertexCount);
        GL.DrawArrays(BeginMode.Triangles, 0, staticMesh.VertexCount);


      GL.PopClientAttrib();
    }

    #endregion

  }
}