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

        private static Fbo MultisampledFbo;
        private static Fbo Fbo;
        private static Fbo BrightFbo;
        private static int[] ColorAttachments;

        public static int SceneTexture { get; private set; }
        public static int SceneBrightTexture { get; private set; }

        public static void Init(Window window)
        {
            Model = Loader.LoadToVao(Positions);
            Filters = new List<PostProcessFilter>();
            FiltersMap = new Dictionary<string, PostProcessFilter>();
            Window = window;
            ColorAttachments = new int[2];

            CreateInternalFBO();
        }

        public static void RegisterFilter(PostProcessFilter filter)
        {
            filter.UpdateVariables();
            filter.UniqueName = GenerateUniqueName();

            Filters.Add(filter);
            FiltersMap.Add(filter.UniqueName, filter);

            if (Filters.Count != 1)
            {
                Filters[^2].EnsureFBO();
            }
            else
            {
                CreateInternalFBO();
            }
        }
        public static void UnregisterFilter(PostProcessFilter filter)
        {
            Filters.Remove(filter);
            FiltersMap.Remove(filter.UniqueName);

            if (Filters.Count != 0)
            {
                for (int i = Filters.Count - 2; i > -1; i--)
                {
                    Filters[i].EnsureFBO();
                }
                Filters[^1].DestroyFBO();
            }
            else
            {
                DestroyInternalFBO();
            }
        }

        public static void StartSceneRendering()
        {
            if (MultisampledFbo != null)
            {
                MultisampledFbo.BindFrameBuffer();
            }
            else if (Fbo != null)
            {
                Fbo.BindFrameBuffer();
            }
        }
        public static void FinishSceneRendering()
        {
            if (MultisampledFbo != null)
            {
                MultisampledFbo.UnbindFrameBuffer();
                if (Fbo != null)
                {
                    MultisampledFbo.ResolveToFbo(Fbo, 0);
                    MultisampledFbo.ResolveToFbo(BrightFbo, 1);
                }
                else
                {
                    MultisampledFbo.ResolveToScreen();
                }
            }
            else if (Fbo != null)
            {
                Fbo.UnbindFrameBuffer();
            }
            PostProcess();
        }

        public static void PostProcess()
        {
            if (Filters.Count > 0)
            {
                Prepare();

                ColorAttachments[0] = Fbo.ColorTexture;
                ColorAttachments[1] = BrightFbo.ColorTexture;

                SceneTexture = Fbo.ColorTexture;
                SceneBrightTexture = BrightFbo.ColorTexture;

                int previousTexture = ColorAttachments[Filters[0].ColorAttachment];

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
        public static string GenerateUniqueName()
        {
            string guid;
            while (FiltersMap.ContainsKey(guid = Guid.NewGuid().ToString("N"))) ;
            return guid;
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
            if (Filters.Count != 0 && Fbo == null)
                Fbo = new Fbo(Window, FboDepthType.DepthRenderBuffer);

            if (Settings.MSAA != 0 && MultisampledFbo == null)
            {
                MultisampledFbo = new Fbo(Window, FboDepthType.DepthRenderBuffer, true, 2);
                BrightFbo = new Fbo(Window, FboDepthType.DepthRenderBuffer);
            }
        }
        private static void DestroyInternalFBO()
        {
            if (Filters.Count == 0 && Fbo != null)
            {
                Fbo.CleanUp();
                Fbo = null;
            }
            if (Settings.MSAA == 0 && MultisampledFbo != null)
            {
                MultisampledFbo.CleanUp();
                MultisampledFbo = null;

                BrightFbo.CleanUp();
                BrightFbo = null;
            }
        }
    }
}
