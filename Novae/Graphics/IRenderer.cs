using Novae.Graphics.Commands;

namespace Novae.Graphics
{
    public interface IRenderer : IDisposable
    {
        internal void Init();
        internal void Shutdown();

        public void Submit(CommandQueue queue);
    }
}
