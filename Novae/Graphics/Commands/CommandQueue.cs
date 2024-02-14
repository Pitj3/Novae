using Silk.NET.OpenGL;
using System.Drawing;

namespace Novae.Graphics.Commands
{
    public class CommandQueue : IDisposable
    {
        private bool _disposed;

        public List<Command> Commands { get; internal set; } = new();

        public bool HasCommands => Commands.Count > 0;

        public void SetClearColor(Color color)
        {
            Commands.Add(new OGL.ClearColorCommand(color));
        }

        public void Clear(bool color = false, bool depth = false, bool stencil = false)
        {
            Commands.Add(new OGL.ClearCommand(color, depth, stencil));
        }

        public void BindGraphicsPipeline(ShaderProgram? pipeline)
        {
            Commands.Add(new OGL.BindGraphicsPipeline(pipeline));
        }

        public unsafe void UploadShaderBuffer<T>(ShaderBuffer buffer, uint binding, T data) where T : struct
        {
            Commands.Add(new OGL.BindBufferBaseCommand(BufferTargetARB.ShaderStorageBuffer, binding, buffer));
            Commands.Add(new OGL.BufferSubDataCommand<T>(GLEnum.ShaderStorageBuffer, 0, buffer.Layout.Size, data));
        }

        public void BindVertexArray(VertexArray array)
        {
            Commands.Add(new OGL.BindVertexArrayCommand(array));
        }

        public void BindFrameBuffer(FrameBuffer buffer, FramebufferTarget target = FramebufferTarget.Framebuffer)
        {
            OGL.BindFramebufferCommand fbCommand = new(buffer)
            {
                Target = target
            };

            Commands.Add(fbCommand);
        }

        public void BindSwapchain()
        {
            Commands.Add(new OGL.BindSwapChainCommand());
        }

        public void DrawElements(GLEnum mode, uint count, GLEnum type)
        {
            Commands.Add(new OGL.DrawElementsCommand(mode, count, type));
        }

        public void MakeTextureResident(Texture texture)
        {
            Commands.Add(new OGL.MakeTextureHandleResident(texture.BindlessHandle));
        }

        public void MakeTextureNonResident(Texture texture)
        {
            Commands.Add(new OGL.MakeTextureHandleNonResident(texture.BindlessHandle));
        }

        public void Dispose()
        {
            if(_disposed) return;
            _disposed = true;

            Commands.Clear();

            GC.SuppressFinalize(this);
        }
    }
}
