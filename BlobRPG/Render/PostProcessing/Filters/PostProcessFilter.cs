using BlobRPG.MainComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.PostProcessing.Filters
{
    public abstract class PostProcessFilter
    {
        protected ImageRenderer Renderer { get; set; }
        public string UniqueName { get; set; }

        public int OutputTexture
        {
            get
            {
                return Renderer.OutputTexture;
            }
        }


        public PostProcessFilter(Window window, float multiplier = 1.0f)
        {
            Renderer = new ImageRenderer(window, multiplier);
        }
        public PostProcessFilter(int width, int height)
        {
            Renderer = new ImageRenderer(width, height);
        }

        public abstract void Render(int texture);
        public abstract void UpdateVariables();
        public abstract void CleanUp();

        public void EnsureFBO()
        {
            Renderer.CreateFBO();
        }
        public void DestroyFBO()
        {
            Renderer.DestroyFBO();
        }
    }
}
