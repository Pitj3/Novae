using Novae.Graphics.Commands;
using Silk.NET.OpenGL;

namespace Novae.Graphics.OGL
{
    public class Renderer3D : IRenderer
    {
        #region Private Data
        private bool _disposed;
        #endregion

        public void Init()
        {
            Graphics.GL.Enable(GLEnum.Blend);
            Graphics.GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        }

        public void Shutdown()
        {

        }

        public void Submit(CommandQueue queue)
        {
            foreach (Command command in queue.Commands)
            {
                command.Execute();
            }

            queue.Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
