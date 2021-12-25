using BlobRPG.MainComponents;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.PostProcessing
{
    public class ImageRenderer
    {
        private Fbo Fbo;
        private readonly int Width;
        private readonly int Height;
        private readonly Window Window;
        private readonly FboDepthType FboDepthType;

        public int OutputTexture
        {
            get
            {
                return Fbo == null ? 0 : Fbo.ColorTexture;
            }
        }

        public ImageRenderer(Window window, FboDepthType fboDepthType = FboDepthType.None)
        {
            Window = window;
            FboDepthType = fboDepthType;
        }
        public ImageRenderer(int width, int height, FboDepthType fboDepthType = FboDepthType.None)
        {
            Width = width;
            Height = height;
            FboDepthType = fboDepthType;
        }

        public void CreateFBO()
        {
            if (Window == null)
            {
                Fbo = new Fbo(Width, Height, FboDepthType);
            }
            else
            {
                Fbo = new Fbo(Window, FboDepthType);
            }
        }
        public void DestroyFBO()
        {
            Fbo.CleanUp();
            Fbo = null;
        }

        public void Render()
        {
            if (Fbo != null)
            {
                Fbo.BindFrameBuffer();
            }
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            if (Fbo != null)
            {
                Fbo.UnbindFrameBuffer();
            }
        }
        public void CleanUp()
        {
            if (Fbo != null)
            {
                Fbo.CleanUp();
            }
        }
    }
}
