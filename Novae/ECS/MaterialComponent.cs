using Novae.Graphics;

namespace Novae.ECS
{
    public class MaterialComponent
    {
        public ShaderProgram Program { get; private set; }
        public Texture Diffuse { get; set; }
        public MaterialComponent(ShaderProgram program, Texture diffuse)
        {
            Program = program;
            Diffuse = diffuse;
        }

    }
}
