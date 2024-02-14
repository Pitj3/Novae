using Silk.NET.OpenGL;
using System.Drawing;

namespace Novae.Graphics.Commands.OGL
{
    public class ClearColorCommand : Command
    {
        public Color Color { get; set; }

        public ClearColorCommand(Color color)
        {
            Color = color;
        }

        public override void Execute()
        {
            Graphics.GL.ClearColor(Color);
        }
    }

    public class ClearCommand : Command
    {
        public bool ClearColor { get; set; }
        public bool ClearDepth { get; set; }
        public bool ClearStencil { get; set; }

        public ClearCommand(bool clearColor = false, bool clearDepth = false, bool clearStencil = false)
        {
            ClearColor = clearColor;
            ClearDepth = clearDepth;
            ClearStencil = clearStencil;
        }

        public override void Execute()
        {
            ClearBufferMask mask = ClearBufferMask.None;
            if (ClearColor)
                mask |= ClearBufferMask.ColorBufferBit;
            if (ClearDepth)
                mask |= ClearBufferMask.DepthBufferBit;
            if (ClearStencil)
                mask |= ClearBufferMask.StencilBufferBit;

            Graphics.GL.Clear(mask);
        }
    }

    public class BindGraphicsPipeline : Command
    {
        public ShaderProgram? Pipeline { get; set; }

        public BindGraphicsPipeline(ShaderProgram? pipeline)
        {
            Pipeline = pipeline;
        }

        public override void Execute()
        {
            if (Pipeline != null)
                Graphics.GL.UseProgram(Pipeline.ID);
            else
                Graphics.GL.UseProgram(0);
        }
    }

    public class BindFramebufferCommand : Command
    {
        public FramebufferTarget Target { get; set; }
        public FrameBuffer FrameBuffer { get; set; }

        public BindFramebufferCommand(FrameBuffer frameBuffer)
        {
            FrameBuffer = frameBuffer;
        }

        public override void Execute()
        {
            Graphics.GL.BindFramebuffer(Target, FrameBuffer.ID);
        }
    }

    public class BindSwapChainCommand : Command
    {
        public override void Execute()
        {
            Graphics.GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }

    public class BindVertexArrayCommand : Command
    {
        public VertexArray VAO { get; set; }
        public BindVertexArrayCommand(VertexArray VAO)
        {
            this.VAO = VAO;
        }

        public override void Execute()
        {
            Graphics.GL.BindVertexArray(VAO.ID);
        }
    }

    public class DrawElementsCommand : Command
    {
        public GLEnum Mode { get; set; }
        public uint Count { get; set; }
        public GLEnum Type { get; set; }

        public DrawElementsCommand(GLEnum mode, uint count, GLEnum type)
        {
            Mode = mode;
            Count = count;
            Type = type;
        }

        public override void Execute()
        {
            unsafe
            {
                Graphics.GL.DrawElements(Mode, Count, Type, null);
            }
        }
    }

    public class BindBufferCommand : Command
    {
        public Buffer Buffer { get; set; }
        public BufferTargetARB Target { get; set; }

        public BindBufferCommand(BufferTargetARB target, Buffer buffer)
        {
            Buffer = buffer;
            Target = target;
        }

        public override void Execute()
        {
            Graphics.GL.BindBuffer(Target, Buffer.ID);
        }
    }

    public class BindBufferBaseCommand : Command
    {
        public Buffer Buffer { get; set; }
        public BufferTargetARB Target { get; set; }
        public uint Binding { get; set; }

        public BindBufferBaseCommand(BufferTargetARB target, uint binding, Buffer buffer)
        {
            Buffer = buffer;
            Target = target;
            Binding = binding;
        }

        public override void Execute()
        {
            Graphics.GL.BindBufferBase(Target, Binding, Buffer.ID);
        }
    }

    public class BufferSubDataCommand<T> : Command where T : struct
    {
        public GLEnum Target { get; set; }
        public nint Offset { get; set; }
        public nuint Size { get; set; }

        public T Data;

        public unsafe BufferSubDataCommand(GLEnum target, nint offset, nuint size, T data)
        {
            Target = target;
            Offset = offset;
            Size = size;
            Data = data;
        }

        public override void Execute()
        {
            unsafe
            {
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
                fixed (T* d = &Data)
                {
                    Graphics.GL.BufferSubData(Target, Offset, Size, d);
                }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            }
        }
    }

    public class MakeTextureHandleResident : Command
    {
        public ulong Handle { get; set; }

        public MakeTextureHandleResident(ulong handle)
        {
            Handle = handle;
        }

        public override void Execute()
        {
            Silk.NET.OpenGL.Extensions.ARB.ArbBindlessTexture bt = new(Graphics.GL.Context);
            bt.MakeTextureHandleResident(Handle);
        }
    }

    public class MakeTextureHandleNonResident : Command
    {
        public ulong Handle { get; set; }

        public MakeTextureHandleNonResident(ulong handle)
        {
            Handle = handle;
        }

        public override void Execute()
        {
            Silk.NET.OpenGL.Extensions.ARB.ArbBindlessTexture bt = new(Graphics.GL.Context);
            bt.MakeTextureHandleNonResident(Handle);
        }
    }
}
