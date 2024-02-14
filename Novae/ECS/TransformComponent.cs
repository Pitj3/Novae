using Silk.NET.Maths;

namespace Novae.ECS
{
    public class TransformComponent
    {
        internal Matrix4X4<float> Matrix { get; set; } = Matrix4X4<float>.Identity;

        private Vector3D<float> _position = Vector3D<float>.Zero;
        public Vector3D<float> Position { 
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                UpdateMatrix();
            }
        }

        private Vector3D<float> _scale = Vector3D<float>.One;
        public Vector3D<float> Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                UpdateMatrix();
            }
        }

        private Vector3D<float> _euler = Vector3D<float>.Zero;
        public Vector3D<float> Euler
        {
            get
            {
                return _euler;
            }
            set
            {
                _euler = value;
                UpdateMatrix();
            }
        }

        private void UpdateMatrix()
        {
            Matrix = Matrix4X4.CreateTranslation(_position);
            Matrix *= Matrix4X4.CreateFromYawPitchRoll(_euler.Y, _euler.X, _euler.Z);
            Matrix *= Matrix4X4.CreateScale(_scale);
        }
    }
}
