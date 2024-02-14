using Silk.NET.OpenGL;

namespace Novae.Graphics
{
    public class FrameBuffer
    {
        internal uint ID { get; private set; }

        internal uint RBOID { get; private set; }

        public FrameBuffer(uint width, uint height)
        {
            ID = Graphics.GL.GenFramebuffer();
            Graphics.GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

            uint texColorBuffer = Graphics.GL.GenTexture();
            Graphics.GL.BindTexture(GLEnum.Texture2D, texColorBuffer);
            unsafe
            {
                Graphics.GL.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, width, height,
                    0, GLEnum.Rgb, GLEnum.UnsignedByte, null);
            }
            Graphics.GL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            Graphics.GL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            Graphics.GL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0,
                GLEnum.Texture2D, texColorBuffer, 0);

            RBOID = Graphics.GL.GenRenderbuffer();
            Graphics.GL.BindRenderbuffer(GLEnum.Renderbuffer, RBOID);
            Graphics.GL.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, width, height);
            Graphics.GL.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment,
                GLEnum.Renderbuffer, RBOID);

            if (Graphics.GL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
            {
                // error in framebuffer creation
                throw new Exception("Error in creation of framebuffer");
            }

            Graphics.GL.BindFramebuffer(GLEnum.Framebuffer, 0);
        }

        public void Bind()
        {
            Graphics.GL.BindFramebuffer(GLEnum.Framebuffer, ID);
        }

        public static void Unbind()
        {
            Graphics.GL.BindFramebuffer(GLEnum.Framebuffer, 0);
        }
    }
}
