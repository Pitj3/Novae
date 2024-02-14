using Silk.NET.Maths;

namespace Novae.Graphics
{
    public enum ShaderType
    {
        FragmentShader = 35632,
        FragmentShaderArb = 35632,
        VertexShader = 35633,
        VertexShaderArb = 35633,
        GeometryShader = 36313,
        TessEvaluationShader = 36487,
        TessControlShader = 36488,
        ComputeShader = 37305
    }

    public class ShaderModule
    {
        public ShaderType Type { get; internal set; }
        internal uint ID { get; private set; }

        public ShaderModule(ShaderType type, string source)
        {
            Type = type;

            ID = Graphics.GL.CreateShader((Silk.NET.OpenGL.ShaderType)type);
            Graphics.GL.ShaderSource(ID, source);
            Graphics.GL.CompileShader(ID);

            int isCompiled = Graphics.GL.GetShader(ID, Silk.NET.OpenGL.GLEnum.CompileStatus);
            if (isCompiled == 0)
            {
                // Shader failed to compile
                string log = Graphics.GL.GetShaderInfoLog(ID);
                if (!string.IsNullOrEmpty(log))
                {
                    /// TODO Replace with proper logger
                    Console.WriteLine(log);
                }

                Graphics.GL.DeleteShader(ID);

                throw new Exception("Shader failed to compile with message: " + log);
            }
        }
    }

    public class ShaderProgram
    {
        public uint ID { get; private set; }
        private Dictionary<string, int> _uniformLocations = new();

        public static ShaderProgram? Current { get; private set; }

        public static readonly uint ObjectDataBufferBinding = 3;
        public static readonly uint ObjectMaterialDataBufferBinding = 4;
        

        public ShaderProgram(params ShaderModule[] modules)
        {
            ID = Graphics.GL.CreateProgram();

            foreach (ShaderModule module in modules)
            {
                Graphics.GL.AttachShader(ID, module.ID);
            }

            Graphics.GL.LinkProgram(ID);

            int isLinked = Graphics.GL.GetProgram(ID, Silk.NET.OpenGL.GLEnum.LinkStatus);
            if (isLinked == 0)
            {
                // Program failed to link
                string log = Graphics.GL.GetProgramInfoLog(ID);
                if (!string.IsNullOrEmpty(log))
                {
                    /// TODO Replace with proper logger
                    Console.WriteLine(log);
                }

                Graphics.GL.DeleteProgram(ID);

                foreach (ShaderModule module in modules)
                {
                    Graphics.GL.DeleteShader(module.ID);
                }

                throw new Exception("Program failed to link with message: " + log);
            }

            // Detach shaders after succesful link
            foreach (ShaderModule module in modules)
            {
                Graphics.GL.DetachShader(ID, module.ID);
            }
        }

        public void Bind()
        {
            Graphics.GL.UseProgram(ID);
            Current = this;
        }

        public static void Unbind()
        {
            Graphics.GL.UseProgram(0);
            Current = null;
        }

        public void Set<T>(T value, string uniform)
        {
            if (value is int i)
            {
                Graphics.GL.Uniform1(GetUniformLocation(uniform), i);
            }

            if (value is float f)
            {
                Graphics.GL.Uniform1(GetUniformLocation(uniform), f);
            }

            if (value is uint u)
            {
                Graphics.GL.Uniform1(GetUniformLocation(uniform), u);
            }

            if (value is short s)
            {
                Graphics.GL.Uniform1(GetUniformLocation(uniform), s);
            }

            if (value is ushort us)
            {
                Graphics.GL.Uniform1(GetUniformLocation(uniform), us);
            }

            if (value is double d)
            {
                Graphics.GL.Uniform1(GetUniformLocation(uniform), d);
            }

            if (value is Vector2D<int> vi)
            {
                Graphics.GL.Uniform2(GetUniformLocation(uniform), vi.X, vi.Y);
            }

            if (value is Vector2D<uint> vui)
            {
                Graphics.GL.Uniform2(GetUniformLocation(uniform), vui.X, vui.Y);
            }

            if (value is Vector2D<float> vfi)
            {
                Graphics.GL.Uniform2(GetUniformLocation(uniform), vfi.X, vfi.Y);
            }

            if (value is Vector3D<int> v3i)
            {
                Graphics.GL.Uniform3(GetUniformLocation(uniform), v3i.X, v3i.Y, v3i.Z);
            }

            if (value is Vector3D<uint> v3ui)
            {
                Graphics.GL.Uniform3(GetUniformLocation(uniform), v3ui.X, v3ui.Y, v3ui.Z);
            }

            if (value is Vector3D<float> v3fi)
            {
                Graphics.GL.Uniform3(GetUniformLocation(uniform), v3fi.X, v3fi.Y, v3fi.Z);
            }

            if (value is Vector4D<int> v4i)
            {
                Graphics.GL.Uniform4(GetUniformLocation(uniform), v4i.X, v4i.Y, v4i.Z, v4i.W);
            }

            if (value is Vector4D<uint> v4ui)
            {
                Graphics.GL.Uniform4(GetUniformLocation(uniform), v4ui.X, v4ui.Y, v4ui.Z, v4ui.W);
            }

            if (value is Vector4D<float> v4fi)
            {
                Graphics.GL.Uniform4(GetUniformLocation(uniform), v4fi.X, v4fi.Y, v4fi.Z, v4fi.W);
            }

            if (value is Matrix2X2<float> m2x2)
            {
                unsafe
                {
                    Graphics.GL.UniformMatrix2(GetUniformLocation(uniform), 1, false, (float*)&m2x2);
                }
            }

            if (value is Matrix3X3<float> m3x3)
            {
                unsafe
                {
                    Graphics.GL.UniformMatrix3(GetUniformLocation(uniform), 1, false, (float*)&m3x3);
                }
            }

            if (value is Matrix4X4<float> m4x4)
            {
                unsafe
                {
                    Graphics.GL.UniformMatrix4(GetUniformLocation(uniform), 1, false, (float*)&m4x4);
                }
            }
        }

        public int GetUniformLocation(string uniform)
        {
            if (_uniformLocations.ContainsKey(uniform))
            {
                return _uniformLocations[uniform];
            }
            else
            {
                int location = Graphics.GL.GetUniformLocation(ID, uniform);
                _uniformLocations[uniform] = location;
                return location;
            }
        }
    }
}
