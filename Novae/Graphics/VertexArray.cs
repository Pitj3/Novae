using Novae.Graphics;

namespace Novae.Graphics
{
    public class VertexArray : IDisposable
    {
        private bool _disposed;

        internal uint ID { get; private set; }

        internal List<VertexBuffer> VertexBuffers { get; private set; } = new List<VertexBuffer>();
        internal IndexBuffer? IndexBuffer { get; private set; }
        internal BufferLayout Layout { get; private set; }

        public VertexArray(List<VertexBuffer> vBuffers, IndexBuffer? iBuffer, BufferLayout layout)
        {
            VertexBuffers.AddRange(vBuffers);
            IndexBuffer = iBuffer;
            Layout = layout;

            Create();

            SetLayout(layout);
        }

        public void SetIndexBuffer(IndexBuffer iBuffer)
        {
            IndexBuffer = iBuffer;

            Graphics.GL.VertexArrayElementBuffer(ID, iBuffer.ID);
        }

        public void SetLayout(BufferLayout layout)
        {
            for (uint i = 0; i < layout.Elements.Count; i++)
            {
                BufferLayout.BufferElement element = layout.Elements[(int)i];
                Graphics.GL.EnableVertexArrayAttrib(ID, i);
                Graphics.GL.VertexArrayAttribFormat(ID, i, (int)element.Count, element.Type, element.Normalized, element.Offset);
                Graphics.GL.VertexArrayAttribBinding(ID, i, 0);
            }
        }

        public void Bind()
        {
            Graphics.GL.BindVertexArray(ID);
        }

        public static void Unbind()
        {
            Graphics.GL.BindVertexArray(0);
        }

        private void Create()
        {
            ID = Graphics.GL.CreateVertexArray();

            uint idx = 0;
            foreach (VertexBuffer vBuffer in VertexBuffers)
            {
                Graphics.GL.VertexArrayVertexBuffer(ID, idx++, vBuffer.ID, 0, Layout.Size);
            }

            if (IndexBuffer != null)
            {
                Graphics.GL.VertexArrayElementBuffer(ID, IndexBuffer.ID);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
