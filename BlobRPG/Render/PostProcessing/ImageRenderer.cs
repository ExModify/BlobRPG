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
        public int Width { get; set; }
        public int Height { get; set; }

        private Fbo Fbo;
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
            Width = window.ClientSize.X;
            Height = window.ClientSize.Y;
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
                Fbo = new Fbo(Window, this, FboDepthType);
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
