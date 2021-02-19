using BlobRPG.MainComponents;
using BlobRPG.Models;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlobRPG.ObjectManager
{
    public static class OBJLoader
    {
        public static RawModel LoadOBJ(string file)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            RawModel model = LoadOBJ(fs);
            fs.Close();
            return model;
        }
        public static RawModel LoadOBJ(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

            List<vec3> vertices = new List<vec3>();
            List<vec2> textureCoords = new List<vec2>();
            List<vec3> normals = new List<vec3>();
            List<int> indices = new List<int>();

            float[] textureCoordsArray = null;
            float[] normalsArray = null;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] data = line.Split(' ');
                if (line.StartsWith("v "))
                {
                    vec3 vertex = new vec3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                    vertices.Add(vertex);
                }
                else if (line.StartsWith("vt "))
                {
                    vec2 textureCoord = new vec2(float.Parse(data[1]), float.Parse(data[2]));
                    textureCoords.Add(textureCoord);
                }
                else if (line.StartsWith("vn "))
                {
                    vec3 normal = new vec3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                    normals.Add(normal);
                }
                else if (line.StartsWith("f "))
                {
                    textureCoordsArray = new float[vertices.Count * 2];
                    normalsArray = new float[vertices.Count * 3];
                    break;
                }
            }

            while (line != null)
            {
                if (line.StartsWith("f "))
                {
                    string[] data = line.Split(' ');
                    string[] vertex1 = data[1].Split('/');
                    string[] vertex2 = data[2].Split('/');
                    string[] vertex3 = data[3].Split('/');

                    ProcessVertex(vertex1, textureCoords, normals, indices, ref textureCoordsArray, ref normalsArray);
                    ProcessVertex(vertex2, textureCoords, normals, indices, ref textureCoordsArray, ref normalsArray);
                    ProcessVertex(vertex3, textureCoords, normals, indices, ref textureCoordsArray, ref normalsArray);
                }

                line = reader.ReadLine();
            }

            reader.Close();

            float[] verticesArray = new float[vertices.Count * 3];
            for (int i = 0; i < vertices.Count; i++)
            {
                verticesArray[i * 3] = vertices[i].x;
                verticesArray[i * 3 + 1] = vertices[i].y;
                verticesArray[i * 3 + 2] = vertices[i].z;
            }
            int[] indicesArray = indices.ToArray();

            return Loader.LoadToVao(verticesArray, textureCoordsArray, normalsArray, indicesArray);
        }

        private static void ProcessVertex(string[] vertexData, List<vec2> textureCoords, List<vec3> normals, List<int> indices, ref float[] textureArray, ref float[] normalsArray)
        {
            int vertexPointer = int.Parse(vertexData[0]) - 1;
            indices.Add(vertexPointer);

            vec2 currentTextureCoord = textureCoords[int.Parse(vertexData[1]) - 1];
            textureArray[vertexPointer * 2] = currentTextureCoord.x;
            textureArray[vertexPointer * 2 + 1] = 1 - currentTextureCoord.y;

            vec3 currentNormal = normals[int.Parse(vertexData[2]) - 1];
            normalsArray[vertexPointer * 3] = currentNormal.x;
            normalsArray[vertexPointer * 3 + 1] = currentNormal.y;
            normalsArray[vertexPointer * 3 + 2] = currentNormal.z;
        }
    }
}
