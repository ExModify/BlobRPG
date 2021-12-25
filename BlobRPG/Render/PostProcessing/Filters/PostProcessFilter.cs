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
        public int OutputTexture
        {
            get
            {
                return Renderer.OutputTexture;
            }
        }


        public PostProcessFilter(Window window)
        {
            Renderer = new ImageRenderer(window);
        }
        public PostProcessFilter(int width, int height)
        {
            Renderer = new ImageRenderer(width, height);
        }

        public abstract void Render(int texture);
        public abstract void UpdateVariables();
        public abstract void CleanUp();

        public void CreateFBO()
        {
            Renderer.CreateFBO();
        }
        public void DestroyFBO()
        {
            Renderer.DestroyFBO();
        }
    }
}
