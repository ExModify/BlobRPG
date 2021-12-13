using BlobRPG.Font;
using BlobRPG.LoggerComponents;
using BlobRPG.Models;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace BlobRPG.MainComponents
{
    public class Loader : ILogger
    {
        private static new readonly LogModule Module = LogModule.Loader;

        private static Window Window;

        private static List<int> Vaos;
        private static List<int> Vbos;
        private static List<int> Textures;
        private static Dictionary<string, string> Shaders;

        public static Dictionary<string, FontType> Fonts { get; private set; }

        public static void Init()
        {
            Vaos = new List<int>();
            Vbos = new List<int>();
            Textures = new List<int>();

            Shaders = new Dictionary<string, string>();
            Fonts = new Dictionary<string, FontType>();
        }
        public static void Load(Window window)
        {
            Window = window;
            Log(Debug, "Loading shaders...");
            string[] shaders = Directory.EnumerateFiles("Shaders/GLSLs").ToArray();
            foreach (string shader in shaders)
            {
                string data = File.ReadAllText(shader);
                string key = Path.GetFileNameWithoutExtension(shader);
                Shaders.Add(key, data);
            }
            Log(Debug, "Loading fonts...");
            Fonts.Add("Meiryo", new FontType(LoadTexture("starter/font/meiryoTexture.png"), "starter/font/meiryo.fnt", window));
            Fonts.Add("Candara", new FontType(LoadTexture("starter/font/candara.png"), "starter/font/candara.fnt", window));
        }
        public static GUIText AddText(string text, vec2 position, string fontFamily = "Meiryo", int fontSize = 8, float maxLineLength = 1f, bool centered = false)
        {
            GUIText txt = new(text, fontSize, Fonts[fontFamily], position, maxLineLength, centered);
            Window.Renderer.AddText(txt);
            return txt;
        }
        public static void RemoveText(GUIText text)
        {
            Window.Renderer.RemoveText(text);
        }
        public static string GetShader(string name, ShaderType type)
        {
            return Shaders[name + (type == ShaderType.VertexShader ? "VS" : "FS")];
        }

        public static int LoadToVao(float[] positions, float[] textures, int dimension = 2)
        {
            int vao = CreateVao();
            StoreDataInAttributeList(0, dimension, positions);
            StoreDataInAttributeList(1, dimension, textures);
            UnbindVao();
            return vao;
        }
        public static RawModel LoadToVao(float[] positions, float[] textureCoords, float[] normals, int[] indices)
        {
            int vao = CreateVao();
            BindIndicesBuffer(indices);
            StoreDataInAttributeList(0, 3, positions);
            StoreDataInAttributeList(1, 2, textureCoords);
            StoreDataInAttributeList(2, 3, normals);
            UnbindVao();
            return new RawModel(vao, indices.Length);
        }
        public static RawModel LoadToVao(float[] positions, float[] textureCoords, float[] normals, float[] tangents, int[] indices)
        {
            int vao = CreateVao();
            BindIndicesBuffer(indices);
            StoreDataInAttributeList(0, 3, positions);
            StoreDataInAttributeList(1, 2, textureCoords);
            StoreDataInAttributeList(2, 3, normals);
            StoreDataInAttributeList(3, 3, tangents);
            UnbindVao();
            return new RawModel(vao, indices.Length);
        }
        public static RawModel LoadToVao(float[] positions, int dimension = 2)
        {
            int vao = CreateVao();
            StoreDataInAttributeList(0, dimension, positions);
            UnbindVao();
            return new RawModel(vao, positions.Length / dimension);
        }
        public static int LoadTexture(string file, bool lodBias = false)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            int id = LoadTexture(fs, lodBias);
            fs.Close();
            return id;
        }
        public static int LoadTexture(Stream texture, bool lodBias = false)
        {
            int textureId = GL.GenTexture();
            Textures.Add(textureId);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            using (Bitmap map = new Bitmap(Image.FromStream(texture)))
            {
                BitmapData data = map.LockBits(new Rectangle(0, 0, map.Width, map.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                if (lodBias)
                {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.4f);
                }
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                map.UnlockBits(data);
            }

            return textureId;
        }
        public static int LoadCubeMap(Stream[] textures)
        {
            int textureId = GL.GenTexture();
            Textures.Add(textureId);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, textureId);

            for (int i = 0; i < textures.Length; i++)
            {
                using Bitmap map = new Bitmap(Image.FromStream(textures[i]));
                BitmapData data = map.LockBits(new Rectangle(0, 0, map.Width, map.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                map.UnlockBits(data);
            }
            
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return textureId;
        }

        public static void CleanUp()
        {
            foreach (int vbo in Vbos)
            {
                GL.DeleteBuffer(vbo);
            }
            foreach (int vao in Vaos)
            {
                GL.DeleteVertexArray(vao);
            }
            foreach (int texture in Textures)
            {
                GL.DeleteTexture(texture);
            }
        }

        private static int CreateVao()
        {
            int vaoId = GL.GenVertexArray();
            Vaos.Add(vaoId);
            GL.BindVertexArray(vaoId);
            return vaoId;
        }
        private static void StoreDataInAttributeList(int attributeNumber, int size, float[] data)
        {
            int vboId = GL.GenBuffer();
            Vbos.Add(vboId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attributeNumber, size, VertexAttribPointerType.Float, false, size * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        private static void UnbindVao()
        {
            GL.BindVertexArray(0);
        }

        private static void BindIndicesBuffer(int[] buffer)
        {
            int vboId = GL.GenBuffer();
            Vbos.Add(vboId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, buffer.Length * sizeof(int), buffer, BufferUsageHint.StaticDraw);
        }

        public static Stream[] OpenStreams(string[] names)
        {
            List<Stream> streams = new List<Stream>();

            foreach (string name in names)
            {
                FileStream fs = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);

                streams.Add(fs);
            }

            return streams.ToArray();
        }

        public static void CloseStreams(Stream[] streams)
        {
            foreach (Stream s in streams)
            {
                (s as FileStream).Close();
            }
        }

        protected static new void Log(LogSeverity severity, string message)
        {
            Log(Module, severity, message);
        }
    }
}
