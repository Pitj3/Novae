using Novae.Graphics;
using Silk.NET.Maths;

namespace Novae.ECS
{
    public class MeshRendererComponent
    {
        public Mesh Mesh { get; set; }

        internal readonly VertexBuffer _vbo;
        internal readonly IndexBuffer _ibo;
        internal readonly VertexArray _vao;

        public MeshRendererComponent(Mesh mesh)
        {
            Mesh = mesh;

            _vbo = new(mesh.Vertices);
            _ibo = new(mesh.Indices);

            BufferLayout layout = new();
            layout.Push<Vector3D<float>>("aPos");
            layout.Push<Vector2D<float>>("aTexCoord");
            layout.Push<Vector3D<float>>("aNormal");
            layout.Push<Vector3D<float>>("aBitangent");
            layout.Push<Vector3D<float>>("aTangent");

            _vao = new VertexArray(new List<VertexBuffer> { _vbo }, _ibo, layout);
        }
    }
}
