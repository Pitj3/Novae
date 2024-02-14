using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace Novae.Graphics
{
    internal sealed class Graphics
    {
        private static Glfw? _glfw;
        internal static Glfw Glfw
        {
            get 
            { 
                _glfw ??= Glfw.GetApi();

                if (!GlfwInitialized)
                    _glfw.Init();

                return _glfw;
            }
        }

        internal static bool GlfwInitialized
        {
            get; set;
        }

        private static GL? _gl;
        internal static GL GL
        {
            get { return _gl ??= GL.GetApi(Glfw.GetProcAddress); }
        }
    }
}
