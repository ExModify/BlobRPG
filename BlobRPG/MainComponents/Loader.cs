using BlobRPG.Models;
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
    public static class Loader
    {
        private static List<int> Vaos;
        private static List<int> Vbos;
        private static List<int> Textures;
        private static Dictionary<string, string> Shaders;

        public static void Init()
        {
            Vaos = new List<int>();
            Vbos = new List<int>();
            Textures = new List<int>();

            Shaders = new Dictionary<string, string>();
        }
        public static void Load()
        {
            string[] shaders = Directory.EnumerateFiles("Shaders/GLSLs").ToArray();
            foreach (string shader in shaders)
            {
                string data = File.ReadAllText(shader);
                string key = Path.GetFileNameWithoutExtension(shader);
                Shaders.Add(key, data);
            }
        }
        public static string GetShader(string name, ShaderType type)
        {
            return Shaders[name + (type == ShaderType.VertexShader ? "VS" : "FS")];
        }

        public static RawModel LoadToVao(float[] positions, float[] textureCoords, int[] indices)
        {
            int vao = CreateVao();
            BindIndicesBuffer(indices);
            StoreDataInAttributeList(0, 3, positions);
            StoreDataInAttributeList(1, 2, textureCoords);
            UnbindVao();
            return new RawModel(vao, indices.Length);
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

    }
}
