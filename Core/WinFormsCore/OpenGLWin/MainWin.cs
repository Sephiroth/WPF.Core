//using OpenTK.Graphics.OpenGL;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.Desktop;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace WinFormsCore.OpenGLWin
//{
//    public class MainWin : GameWindow
//    {
//        private GameWindowSettings _gameWindowSettings;
//        private NativeWindowSettings _nativeWindowSettings;


//        public MainWin(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
//            : base(gameWindowSettings, nativeWindowSettings)
//        {
//            _gameWindowSettings = gameWindowSettings;
//            _nativeWindowSettings = nativeWindowSettings;
//        }

//        protected override void OnLoad()
//        {
//            base.OnLoad();
//        }

//        protected override void OnResize(ResizeEventArgs e)
//        {
//            base.OnResize(e);
//            //GL.Viewport(0,0,);
//            //GL.
//        }

//        protected override void OnRenderFrame(FrameEventArgs e)
//        {
//            base.OnRenderFrame(e);

//            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

//            SwapBuffers();
//        }

//    }
//}