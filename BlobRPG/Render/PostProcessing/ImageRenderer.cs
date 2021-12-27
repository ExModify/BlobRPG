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

        public bool Multisampled { get; private set; }
        public float Multiplier { get; private set; } = 1;

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

        public ImageRenderer(Window window, float multiplier = 1, FboDepthType fboDepthType = FboDepthType.None, bool multisampled = false)
        {
            Window = window;
            Width = (int)(window.ClientSize.X * multiplier);
            Height = (int)(window.ClientSize.Y * multiplier);
            Multiplier = multiplier;
            FboDepthType = fboDepthType;
            Multisampled = multisampled;
        }
        public ImageRenderer(int width, int height, FboDepthType fboDepthType = FboDepthType.None, bool multisampled = false)
        {
            Width = width;
            Height = height;
            FboDepthType = fboDepthType;
            Multisampled = multisampled;
        }

        public void CreateFBO()
        {
            if (Fbo == null)
            {
                if (Window == null)
                {
                    Fbo = new Fbo(Width, Height, FboDepthType, Multisampled);
                }
                else
                {
                    Fbo = new Fbo(Window, this, FboDepthType, Multisampled);
                }
            }
        }
        public void DestroyFBO()
        {
            if (Fbo != null)
            {
                Fbo.CleanUp();
                Fbo = null;
            }
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
