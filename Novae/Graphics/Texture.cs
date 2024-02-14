using StbImageSharp;

namespace Novae.Graphics
{
    public class Texture
    {
        internal uint ID { get; set; }
        internal ulong BindlessHandle { get; set; }

        public uint Width { get; set; }
        public uint Height { get; set; }

        public Texture(string path)
        {
            ID = Graphics.GL.CreateTexture(Silk.NET.OpenGL.GLEnum.Texture2D);

            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);

            Width = (uint)result.Width;
            Height = (uint)result.Height;

            Graphics.GL.TextureStorage2D(ID, 1, Silk.NET.OpenGL.GLEnum.Rgba8, Width, Height);

            unsafe
            {
                fixed (byte* ptr = result.Data)
                {
                    Graphics.GL.TextureSubImage2D(ID, 0, 0, 0, Width, Height, Silk.NET.OpenGL.GLEnum.Rgba, Silk.NET.OpenGL.GLEnum.UnsignedByte, ptr);
                }
            }

            Graphics.GL.GenerateTextureMipmap(ID);

            Graphics.GL.TextureParameter(ID, Silk.NET.OpenGL.GLEnum.TextureWrapS, (int)Silk.NET.OpenGL.TextureWrapMode.Repeat);
            Graphics.GL.TextureParameter(ID, Silk.NET.OpenGL.GLEnum.TextureWrapT, (int)Silk.NET.OpenGL.TextureWrapMode.Repeat);
            Graphics.GL.TextureParameter(ID, Silk.NET.OpenGL.GLEnum.TextureMinFilter, (int)Silk.NET.OpenGL.TextureMinFilter.Nearest);
            Graphics.GL.TextureParameter(ID, Silk.NET.OpenGL.GLEnum.TextureMagFilter, (int)Silk.NET.OpenGL.TextureMagFilter.Nearest);

            Silk.NET.OpenGL.Extensions.ARB.ArbBindlessTexture bt = new(Graphics.GL.Context);
            BindlessHandle = bt.GetTextureHandle(ID);
        }
    }
}
