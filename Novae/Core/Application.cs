using Arch.Core;
using Novae.ECS;
using Novae.Graphics;
using Novae.Graphics.Commands;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Novae.Core
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ObjectData
    {
        [FieldOffset(0)]
        public Matrix4X4<float> Model;
        [FieldOffset(16)]
        public Matrix4X4<float> Projection;
        [FieldOffset(32)]
        public Matrix4X4<float> View;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ObjectMaterialData
    {
        [FieldOffset(0)]
        public ulong DiffuseTexture;
    }

    public class Application : IDisposable
    {
        #region Private Data
        private bool _disposed;
        private bool _initialized;
        private bool _isRunning;

        private float _fixedTickCounter;

        private Window _window;

        #endregion

        #region Public Data
        public static Application Instance { get; private set; }
        public static World GameWorld { get; private set; }

        public static IRenderer Renderer { get; internal set; }

        public uint Width
        {
            get
            {
                return _window.Width;
            }
            set
            {
                _window.SetSize(value, Height);
            }
        }

        public uint Height
        {
            get
            {
                return _window.Height;
            }
            set
            {
                _window.SetSize(Width, value);
            }
        }

        public string Title
        {
            get
            {
                return _window.Title;
            }
            set
            {
                _window.Title = value;
            }
        }
        #endregion

        #region TestData
        private ShaderProgram _program;
        private ShaderBuffer ObjectDataBuffer;
        private ShaderBuffer ObjectMaterialBuffer;
        private Graphics.Texture _texture;
        #endregion

        #region Application Events
        public void Initialize()
        {
            AssertNotDisposed();

            if (_initialized)
                return;

            _initialized = true;

            Window.WindowCreationInfo createInfo = new()
            {
                Width = 1280,
                Height = 720,
                Name = "X Novae Game",
                VSync = false,
                Fullscreen = false
            };

            _window = Window.Create(createInfo);

            Renderer = new Graphics.OGL.Renderer3D();
            Renderer.Init();

            Vertex[] vertices =
            {
                new Vertex {Position = new Vector3D<float>(0.5f, 0.5f, 0.0f), UV = new Vector2D<float>(1.0f, 1.0f) },
                new Vertex {Position = new Vector3D<float>(0.5f, -0.5f, 0.0f), UV = new Vector2D<float>(1.0f, 0.0f) },
                new Vertex {Position = new Vector3D<float>(-0.5f, -0.5f, 0.0f), UV = new Vector2D<float>(0.0f, 0.0f) },
                new Vertex {Position = new Vector3D<float>(-0.5f, 0.5f, 0.0f), UV = new Vector2D<float>(0.0f, 1.0f) }
            };

            uint[] indices =
            {
                0, 1, 3, 1, 2, 3
            };

            Mesh m = new()
            {
                Vertices = vertices,
                Indices = indices
            };

            _texture = new Graphics.Texture("data/test.png");
            _program = new ShaderProgram(new ShaderModule(Graphics.ShaderType.VertexShader, File.ReadAllText("data/shader_vs.glsl")),
                new ShaderModule(Graphics.ShaderType.FragmentShader, File.ReadAllText("data/shader_fs.glsl")));

            GameWorld = World.Create();
            GameWorld.Create(new TransformComponent(), new MeshRendererComponent(m), new MaterialComponent(_program, _texture));

            BufferLayout ObjectDataBufferLayout = new();
            ObjectDataBufferLayout.Push<Matrix4X4<float>>("Model");
            ObjectDataBufferLayout.Push<Matrix4X4<float>>("Projection");
            ObjectDataBufferLayout.Push<Matrix4X4<float>>("View");
            ObjectDataBuffer = new ShaderBuffer(ObjectDataBufferLayout);


            BufferLayout ObjectMaterialBufferLayout = new();
            ObjectMaterialBufferLayout.Push<ulong>("DiffuseTexture");
            ObjectMaterialBuffer = new ShaderBuffer(ObjectMaterialBufferLayout);

            /// TODO: This is where some kind of level loading and such needs to take place, or user initialize code
        }

        public void Tick()
        {
            AssertNotDisposed();
        }

        public void FixedTick()
        {
            AssertNotDisposed();
        }

        public void Render()
        {
            AssertNotDisposed();

            CommandQueue queue = new();
            queue.SetClearColor(Color.CornflowerBlue);
            queue.Clear(true, true, false);
            queue.BindSwapchain();

            // TODO: find out when to make textures resident
            queue.MakeTextureResident(_texture);

            // Meshes query
            var query = new QueryDescription().WithAll<TransformComponent, MeshRendererComponent, MaterialComponent>();
            GameWorld.Query(in query, (ref TransformComponent transform, ref MeshRendererComponent meshRenderer, ref MaterialComponent material) =>
            {
                queue.BindGraphicsPipeline(material.Program);

                ObjectData data = new()
                {
                    Model = transform.Matrix
                };
                queue.UploadShaderBuffer(ObjectDataBuffer, ShaderProgram.ObjectDataBufferBinding, data);

                ObjectMaterialData materialData = new()
                {
                    DiffuseTexture = material.Diffuse.BindlessHandle
                };
                queue.UploadShaderBuffer(ObjectMaterialBuffer, ShaderProgram.ObjectMaterialDataBufferBinding, materialData);

                queue.BindVertexArray(meshRenderer._vao);

                queue.DrawElements(GLEnum.Triangles, (uint)meshRenderer.Mesh.Indices.Length, GLEnum.UnsignedInt);

                queue.BindGraphicsPipeline(null);
            });

            // TODO: find out when to make textures non resident
            queue.MakeTextureNonResident(_texture);

            Renderer.Submit(queue);

            _window.SwapBuffers();
        }

        public void Shutdown()
        {
            AssertNotDisposed();

            Renderer.Shutdown();
        }
        #endregion

        #region Public API
        public void Run()
        {
            AssertNotDisposed();

            Initialize();

            _isRunning = true;
            while(_isRunning)
            {
                Window.PollEvents();

                Time.Calculate();

                if (_window.ShouldClose)
                    Quit();

                /// TODO: Test this with lag
                _fixedTickCounter += Time.DeltaTime;
                if(_fixedTickCounter >= Time.FixedDeltaTime)
                {
                    _fixedTickCounter -= Time.FixedDeltaTime;
                    if (_fixedTickCounter < 0.0f)
                        _fixedTickCounter = 0.0f;

                    FixedTick();
                }

                Tick();
                Render();
            }

            Shutdown();
        }

        public void Quit()
        {
            AssertNotDisposed();

            _isRunning = false;
        }
        #endregion

        #region IDisposable Implementation
        [DebuggerNonUserCode]
        private void AssertNotDisposed()
        {
            if (_disposed)
            {
                string name = GetType().Name;
                throw new ObjectDisposedException(
                    name, string.Format("The {0} object was used after being Disposed.", name));
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
