using BlobRPG.Models;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlobRPG.MainComponents
{
    public static class Loader
    {
        private static List<int> Vaos;
        private static List<int> Vbos;
        private static Dictionary<string, string> Shaders;

        public static void Init()
        {
            Vaos = new List<int>();
            Vbos = new List<int>();

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

        public static RawModel LoadToVao(float[] positions, int[] indices)
        {
            int vao = CreateVao();
            BindIndicesBuffer(indices);
            StoreDataInAttributeList(0, positions);
            UnbindVao();
            return new RawModel(vao, indices.Length);
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
        }

        private static int CreateVao()
        {
            int vaoId = GL.GenVertexArray();
            Vaos.Add(vaoId);
            GL.BindVertexArray(vaoId);
            return vaoId;
        }
        private static void StoreDataInAttributeList(int attributeNumber, float[] data)
        {
            int vboId = GL.GenBuffer();
            Vbos.Add(vboId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attributeNumber, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
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
