using Silk.NET.Maths;

namespace Novae.Graphics
{
    public struct Vertex
    {
        public Vector3D<float> Position;
        public Vector3D<float> Normal;
        public Vector2D<float> UV;
        public Vector3D<float> Binormal;
        public Vector3D<float> Tangent;
    }
    
    public class Mesh
    {
        public Vertex[] Vertices { get; set; }
        public uint[] Indices { get; set; }
    }
}
