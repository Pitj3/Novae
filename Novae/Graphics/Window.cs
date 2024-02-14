using Novae.Core;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;
using Monitor = Silk.NET.GLFW.Monitor;

namespace Novae.Graphics
{
    public class Window
    {
        #region Internal Data
        internal unsafe WindowHandle* _handle;
        internal static readonly log4net.ILog log = log4net.LogManager.GetLogger("Novae.Graphics.Window");
        #endregion

        #region Structs
        public class WindowCreationInfo
        {
            public string Name = string.Empty;
            public uint Width;
            public uint Height;

            public bool Fullscreen;
            public bool VSync;
        }
        #endregion

        #region Private Data
        private uint _width;
        private uint _height;
        private string _title;
        #endregion

        #region Public Data
        public uint Width { 
            get 
            { 
                return _width; 
            } 

            set
            {
                SetSize(value, _height);
            }
        }

        public uint Height
        {
            get
            {
                return _height;
            }

            set
            {
                SetSize(_width, value);
            }
        }

        public string Title
        {
            get
            {
                unsafe
                {
                    return _title;
                }
            }
            set
            {
                unsafe
                {
                    _title = value;
                    Graphics.Glfw.SetWindowTitle(_handle, value);
                }
            }
        }

        public Vector2D<int> Position
        {
            get
            {
                unsafe
                {
                    Graphics.Glfw.GetWindowPos(_handle, out int x, out int y);
                    return new Vector2D<int>(x, y);
                }
            }
            set
            {
                unsafe
                {
                    Graphics.Glfw.SetWindowPos(_handle, value.X, value.Y);
                }
            }
        }

        public bool Minimized
        {
            get
            {
                unsafe
                {
                    return Graphics.Glfw.GetWindowAttrib(_handle, WindowAttributeGetter.Iconified);
                }
            }

            set
            {
                SetMinimized(value);
            }
        }

        public bool Maximized
        {
            get
            {
                unsafe
                {
                    return Graphics.Glfw.GetWindowAttrib(_handle, WindowAttributeGetter.Maximized);
                }
            }

            set
            {
                SetMaximized(value);
            }
        }

        public bool Focused
        {
            get
            {
                unsafe
                {
                    return Graphics.Glfw.GetWindowAttrib(_handle, WindowAttributeGetter.Focused);
                }
            }

            set
            {
                SetFocused(value);
            }
        }

        public bool Resizable
        {
            get
            {
                unsafe
                {
                    return Graphics.Glfw.GetWindowAttrib(_handle, WindowAttributeGetter.Resizable);
                }
            }

            set
            {
                unsafe
                {
                    Graphics.Glfw.SetWindowAttrib(_handle, WindowAttributeSetter.Resizable, value);
                }
            }
        }

        public bool Visible
        {
            set
            {
                unsafe
                {
                    if (value)
                    {
                        Graphics.Glfw.ShowWindow(_handle);
                    }
                    else
                    {
                        Graphics.Glfw.HideWindow(_handle);
                    }
                }
            }
            get
            {
                unsafe
                {
                    return Graphics.Glfw.GetWindowAttrib(_handle, WindowAttributeGetter.Visible);
                }
            }
        }

        public bool ShouldClose { get; internal set; }
        #endregion

        #region Static API
        public static Window Create(WindowCreationInfo createInfo)
        {
            unsafe
            {
                Graphics.Glfw.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGL);
                Graphics.Glfw.WindowHint(WindowHintInt.ContextVersionMajor, 4);
                Graphics.Glfw.WindowHint(WindowHintInt.ContextVersionMinor, 6);
                Graphics.Glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

                Monitor* primaryMonitor = null;
                if (createInfo.Fullscreen)
                    primaryMonitor = Graphics.Glfw.GetPrimaryMonitor();

                Window w = new()
                {
                    _handle = Graphics.Glfw.CreateWindow((int)createInfo.Width, (int)createInfo.Height, createInfo.Name, primaryMonitor, null)
                };

                Graphics.Glfw.MakeContextCurrent(w._handle);
                w.Visible = true;

                log.Info("Created window with OpenGL Context version " +
                    Graphics.GL.GetInteger(Silk.NET.OpenGL.GLEnum.MajorVersion) + "." + Graphics.GL.GetInteger(Silk.NET.OpenGL.GLEnum.MinorVersion));

                if (createInfo.VSync)
                    Graphics.Glfw.SwapInterval(1);

                Graphics.Glfw.SetWindowSizeCallback(w._handle, (handle, width, height) =>
                {
                    w._width = (uint)width;
                    w._height = (uint)height;
                });

                Graphics.Glfw.SetKeyCallback(w._handle, (handle, key, scancode, action, mods) =>
                {
                    Input.SetKeyState(key, action);
                });

                Graphics.Glfw.SetMouseButtonCallback(w._handle, (handle, button, action, mods) =>
                {
                    Input.SetMouseState(button, action);
                });

                Graphics.Glfw.SetCursorPosCallback(w._handle, (handle, xpos, ypos) =>
                {
                    Input.MousePosition = new Silk.NET.Maths.Vector2D<int>((int)xpos, (int)ypos);
                });

                Graphics.Glfw.SetScrollCallback(w._handle, (handle, xscroll, yscroll) =>
                {
                    Input.MouseScroll = new Silk.NET.Maths.Vector2D<int>((int)xscroll, (int)yscroll);
                });

                Graphics.Glfw.SetWindowCloseCallback(w._handle, (handle) =>
                {
                    w.ShouldClose = true;
                });

                Graphics.Glfw.SetErrorCallback((error, description) =>
                {
                    log.Error("GLFW Error with code " + error + " and message " + description);
                    throw new ApplicationException();
                });

                unsafe
                {
                    Graphics.GL.Enable(EnableCap.DebugOutput);
                    Graphics.GL.DebugMessageCallback(OpenGLMessageCallback, null);
                }

                return w;
            }
        }
        #endregion

        public static void OpenGLMessageCallback(GLEnum source, GLEnum type, int id, GLEnum severity, int length, nint message, nint userParam)
        {
            if(severity == GLEnum.DebugSeverityHigh)
                Console.WriteLine("Source: " + source.ToString() + " Type: " + type.ToString() + " Message: " + Marshal.PtrToStringUTF8(message));
        }

        #region Public API
        public static void PollEvents()
        {
            Graphics.Glfw.PollEvents();
        }

        public void SwapBuffers()
        {
            unsafe
            {
                Graphics.Glfw.SwapBuffers(_handle);
            }
        }
        #endregion

        #region Set Events
        public void SetSize(uint width, uint height)
        {
            _width = width;
            _height = height;

            unsafe
            {
                Graphics.Glfw.SetWindowSize(_handle, (int)_width, (int)_height);
            }
        }

        public void SetMinimized(bool value)
        {
            unsafe
            {
                if (value)
                {
                    Graphics.Glfw.IconifyWindow(_handle);
                }
                else
                {
                    Graphics.Glfw.RestoreWindow(_handle);
                }
            }
        }

        public void SetMaximized(bool value)
        {
            unsafe
            {
                if (value)
                {
                    Graphics.Glfw.MaximizeWindow(_handle);
                }
                else
                {
                    Graphics.Glfw.RestoreWindow(_handle);
                }
            }
        }

        public void SetFocused(bool value)
        {
            unsafe
            {
                if (value)
                {
                    Graphics.Glfw.FocusWindow(_handle);
                }
            }
        }
        #endregion
    }

}
