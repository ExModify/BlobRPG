using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Render.PostProcessing.Filters;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.PostProcessing
{
    public class PostProcessor
    {
        private static readonly float[] Positions = new float[8] { -1, 1, -1, -1, 1, 1, 1, -1 };
        private static Dictionary<string, PostProcessFilter> FiltersMap;
        private static List<PostProcessFilter> Filters;
        private static RawModel Model;
        private static Window Window;
        private static Fbo Fbo;

        public static void Init(Window window)
        {
            Model = Loader.LoadToVao(Positions);
            Filters = new List<PostProcessFilter>();
            FiltersMap = new Dictionary<string, PostProcessFilter>();
            Window = window;
        }

        public static void RegisterFilter(PostProcessFilter filter)
        {
            if (Filters.Count != 0)
            {
                Filters[^1].CreateFBO();
            }
            else
            {
                CreateInternalFBO();
            }
            filter.UpdateVariables();
            Filters.Add(filter);
            FiltersMap.Add(filter.GetType().Name, filter);
        }
        public static void UnregisterFilter(PostProcessFilter filter)
        {
            Filters.Remove(filter);
            FiltersMap.Remove(filter.GetType().Name);
            if (Filters.Count != 0)
            {
                Filters[^1].DestroyFBO();
            }
            else
            {
                DestroyInternalFBO();
            }
        }

        public static void StartSceneRendering()
        {
            if (Fbo != null)
            {
                Fbo.BindFrameBuffer();
            }
        }
        public static void FinishSceneRendering()
        {
            if (Fbo != null)
            {
                Fbo.UnbindFrameBuffer();
            }
        }

        public static void PostProcess()
        {
            if (Filters.Count > 0)
            {
                Prepare();
                int previousTexture = Fbo.ColorTexture;

                for (int i = 0; i < Filters.Count; i++)
                {
                    Filters[i].Render(previousTexture);
                    previousTexture = Filters[i].OutputTexture;
                }

                Finish();
            }
        }
        public static void CleanUp()
        {
            DestroyInternalFBO();
            foreach (PostProcessFilter filter in Filters)
            {
                filter.CleanUp();
            }
        }

        private static void Prepare()
        {
            GL.BindVertexArray(Model.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.Disable(EnableCap.DepthTest);
        }
        private static void Finish()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private static void CreateInternalFBO()
        {
            Fbo = new Fbo(Window, FboDepthType.DepthRenderBuffer);
        }
        private static void DestroyInternalFBO()
        {
            Fbo.CleanUp();
            Fbo = null;
        }
    }
}
