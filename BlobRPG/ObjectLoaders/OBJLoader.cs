using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.ObjectLoaders.Models;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlobRPG.ObjectLoaders
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

            List<Vertex> vertices = new List<Vertex>();
            List<vec2> textures = new List<vec2>();
            List<vec3> normals = new List<vec3>();
            List<int> indices = new List<int>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] data = line.Split(' ');
                if (line.StartsWith("v "))
                {
                    vec3 vertexCoords = new vec3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                    Vertex vertex = new Vertex(vertices.Count, vertexCoords);
                    vertices.Add(vertex);
                }
                else if (line.StartsWith("vt "))
                {
                    vec2 textureCoord = new vec2(float.Parse(data[1]), float.Parse(data[2]));
                    textures.Add(textureCoord);
                }
                else if (line.StartsWith("vn "))
                {
                    vec3 normal = new vec3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                    normals.Add(normal);
                }
                else if (line.StartsWith("f "))
                {
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

                    Vertex v0 = ProcessVertex(vertex1, vertices, indices);
                    Vertex v1 = ProcessVertex(vertex2, vertices, indices);
                    Vertex v2 = ProcessVertex(vertex3, vertices, indices);
                    CalculateTangents(v0, v1, v2, textures);
                }

                line = reader.ReadLine();
            }

            reader.Close();

            RemoveUnusedVertices(vertices);

            float[] verticesArray = new float[vertices.Count * 3];
            float[] texturesArray = new float[vertices.Count * 2];
            float[] normalsArray = new float[vertices.Count * 3];
            float[] tangentsArray = new float[vertices.Count * 3];

            float furthest = ConvertDataToArrays(vertices, textures, normals, ref verticesArray, ref texturesArray, ref normalsArray, ref tangentsArray);
            int[] indicesArray = indices.ToArray();

            return Loader.LoadToVao(verticesArray, texturesArray, normalsArray, tangentsArray, indicesArray);
        }
        private static void CalculateTangents(Vertex v0, Vertex v1, Vertex v2, List<vec2> textures)
        {
            vec3 delatPos1 = v1.Position - v0.Position;
            vec3 delatPos2 = v2.Position - v0.Position;
            vec2 uv0 = textures[v0.TextureIndex];
            vec2 uv1 = textures[v1.TextureIndex];
            vec2 uv2 = textures[v2.TextureIndex];
            vec2 deltaUv1 = uv1 - uv0;
            vec2 deltaUv2 = uv2 - uv0;

            float r = 1.0f / (deltaUv1.x * deltaUv2.y - deltaUv1.y * deltaUv2.x);
            delatPos1 *= deltaUv2.y;
            delatPos2 *= deltaUv1.y;
            vec3 tangent = delatPos1 - delatPos2;
            tangent *= r;
            v0.AddTangent(tangent);
            v1.AddTangent(tangent);
            v2.AddTangent(tangent);
        }

        private static Vertex ProcessVertex(string[] vertex, List<Vertex> vertices, List<int> indices)
        {
            int index = int.Parse(vertex[0]) - 1;
            Vertex currentVertex = vertices[index];
            int textureIndex = int.Parse(vertex[1]) - 1;
            int normalIndex = int.Parse(vertex[2]) - 1;
            if (!currentVertex.IsSet)
            {
                currentVertex.TextureIndex = textureIndex;
                currentVertex.NormalIndex = normalIndex;
                indices.Add(index);
                return currentVertex;
            }
            else
            {
                return DealWithAlreadyProcessedVertex(currentVertex, textureIndex, normalIndex, indices, vertices);
            }
        }

        private static float ConvertDataToArrays(List<Vertex> vertices, List<vec2> textures, List<vec3> normals, ref float[] verticesArray, ref float[] texturesArray, ref float[] normalsArray, ref float[] tangentsArray)
        {
            float furthestPoint = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex currentVertex = vertices[i];
                if (currentVertex.Length > furthestPoint)
                {
                    furthestPoint = currentVertex.Length;
                }
                vec2 textureCoord = textures[currentVertex.TextureIndex];
                vec3 normalVector = normals[currentVertex.NormalIndex];
                vec3 tangent = currentVertex.AveragedTangent;
                verticesArray[i * 3] = currentVertex.Position.x;
                verticesArray[i * 3 + 1] = currentVertex.Position.y;
                verticesArray[i * 3 + 2] = currentVertex.Position.z;
                texturesArray[i * 2] = textureCoord.x;
                texturesArray[i * 2 + 1] = 1 - textureCoord.y;
                normalsArray[i * 3] = normalVector.x;
                normalsArray[i * 3 + 1] = normalVector.y;
                normalsArray[i * 3 + 2] = normalVector.z;
                tangentsArray[i * 3] = tangent.x;
                tangentsArray[i * 3 + 1] = tangent.y;
                tangentsArray[i * 3 + 2] = tangent.z;

            }
            return furthestPoint;
        }

        private static Vertex DealWithAlreadyProcessedVertex(Vertex previousVertex, int newTextureIndex,
                int newNormalIndex, List<int> indices, List<Vertex> vertices)
        {
            if (previousVertex.IsSameIndex(newTextureIndex, newNormalIndex))
            {
                indices.Add(previousVertex.Index);
                return previousVertex;
            }
            else
            {
                Vertex anotherVertex = previousVertex.DuplicateVertex;
                if (anotherVertex != null)
                {
                    return DealWithAlreadyProcessedVertex(anotherVertex, newTextureIndex, newNormalIndex, indices, vertices);
                }
                else
                {
                    Vertex duplicateVertex = previousVertex.Duplicate(vertices.Count);
                    duplicateVertex.TextureIndex = newTextureIndex;
                    duplicateVertex.NormalIndex = newNormalIndex;
                    previousVertex.DuplicateVertex = duplicateVertex;
                    vertices.Add(duplicateVertex);
                    indices.Add(duplicateVertex.Index);
                    return duplicateVertex;
                }
            }
        }

        private static void RemoveUnusedVertices(List<Vertex> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].AverageTangents();
                if (!vertices[i].IsSet)
                {
                    vertices[i].TextureIndex = 0;
                    vertices[i].NormalIndex = 0;
                }
            }
        }


        /* Old loader */
        public static RawModel LoadSimpleOBJ(string file)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            RawModel model = LoadSimpleOBJ(fs);
            fs.Close();
            return model;
        }
        public static RawModel LoadSimpleOBJ(Stream stream)
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

                    ProcessSimpleVertex(vertex1, textureCoords, normals, indices, ref textureCoordsArray, ref normalsArray);
                    ProcessSimpleVertex(vertex2, textureCoords, normals, indices, ref textureCoordsArray, ref normalsArray);
                    ProcessSimpleVertex(vertex3, textureCoords, normals, indices, ref textureCoordsArray, ref normalsArray);
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

        private static void ProcessSimpleVertex(string[] vertexData, List<vec2> textureCoords, List<vec3> normals, List<int> indices, ref float[] textureArray, ref float[] normalsArray)
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
