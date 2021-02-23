using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Textures;
using BlobRPG.Tools;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace BlobRPG.Entities
{
    public class Terrain
    {
        public const float MaxHeight = 40;

        private const float Size = 500;
        private const float MaxPixelColor = 256 * 256 * 256;

        public float X { get; private set; }
        public float Z { get; private set; }

        public vec4 Boundaries { get; private set; }
        public float[,] TerrainHeight { get; private set; }

        public RawModel Model { get; private set; }
        public TerrainTexturePack TexturePack { get; private set; }
        public TerrainTexture BlendMap { get; private set; }

        public mat4 TransformationMatrix { get; private set; }

        public Terrain(int gridX, int gridZ, TerrainTexturePack texturePack, TerrainTexture blendMap, Stream heightMap)
        {
            X = gridX * Size;
            Z = gridZ * Size;
            Boundaries = new vec4(X, Z, (gridX + 1) * Size, (gridZ + 1) * Size);
            TexturePack = texturePack;
            BlendMap = blendMap;
            GenerateTerrain(heightMap);
            UpdateTransformationMatrix();
        }
        

        public float GetHeightOfTerrain(float x, float z)
        {
            float tx = x - X;
            float tz = z - Z;

            int size = TerrainHeight.GetLength(0);

            float gridSquareSize = Size / (size - 1);
            int gridX = (int)Math.Floor(tx / gridSquareSize);
            int gridZ = (int)Math.Floor(tz / gridSquareSize);

            if (gridX >= size - 1 || gridZ >= size - 1 || gridX < 0 || gridZ < 0)
            {
                return 0;
            }

            float xCoord = (tx % gridSquareSize) / gridSquareSize;
            float zCoord = (tz % gridSquareSize) / gridSquareSize;
            float height;

            if (xCoord <= (1 - zCoord))
            {
                height = BaryCentric(new vec3(0, TerrainHeight[gridX, gridZ], 0), new vec3(1,
                            TerrainHeight[gridX + 1, gridZ], 0), new vec3(0,
                            TerrainHeight[gridX, gridZ + 1], 1), new vec2(xCoord, zCoord));
            }
            else
            {
                height = BaryCentric(new vec3(1, TerrainHeight[gridX + 1, gridZ], 0), new vec3(1,
                            TerrainHeight[gridX + 1, gridZ + 1], 1), new vec3(0,
                            TerrainHeight[gridX, gridZ + 1], 1), new vec2(xCoord, zCoord));
            }
            return height;
        }
        public bool OnTerrain(float x, float z)
        {
            float tx = x - X;
            float tz = z - Z;

            int size = TerrainHeight.GetLength(0);
            float gridSquareSize = Size / (size - 1);

            int gridX = (int)Math.Floor(tx / gridSquareSize);
            int gridZ = (int)Math.Floor(tz / gridSquareSize);

            if (gridX >= size - 1|| gridZ >= size - 1 || gridX < 0 || gridZ < 0)
            {
                return false;
            }
            return true;
        }

        private void GenerateTerrain(Stream heightMapData)
        {
            Bitmap heightMap = new Bitmap(Image.FromStream(heightMapData));

            int vertexCount = heightMap.Height;

            int count = vertexCount * vertexCount;

            int noVertices = count * 3;
            int noNormals = count * 3;
            int noTextureCoords = count * 2;
            int noIndices = 6 * (vertexCount - 1) * (vertexCount - 1);


            float[] vertices = new float[noVertices];
            float[] normals = new float[noNormals];
            float[] textureCoords = new float[noTextureCoords];
            int[] indices = new int[noIndices];

            int vertexPointer = 0;
            TerrainHeight = new float[vertexCount, vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    vertices[vertexPointer * 3] = j / ((float)vertexCount - 1) * Size;

                    float vertexHeight = GetHeight(j, i, heightMap);
                    TerrainHeight[i, j] = vertexHeight;
                    vertices[vertexPointer * 3 + 1] = vertexHeight;
                    vertices[vertexPointer * 3 + 2] = i / ((float)vertexCount - 1) * Size;
                    vec3 normal = CalculateNormal(j, i, heightMap);
                    normals[vertexPointer * 3] = normal.x;
                    normals[vertexPointer * 3 + 1] = normal.y;
                    normals[vertexPointer * 3 + 2] = normal.z;
                    textureCoords[vertexPointer * 2] = j / ((float)vertexCount - 1);
                    textureCoords[vertexPointer * 2 + 1] = i / ((float)vertexCount - 1);
                    vertexPointer++;
                }
            }
            int pointer = 0;
            for (int gz = 0; gz < vertexCount - 1; gz++)
            {
                for (int gx = 0; gx < vertexCount - 1; gx++)
                {
                    int topLeft = (gz * vertexCount) + gx;
                    int topRight = topLeft + 1;
                    int bottomLeft = ((gz + 1) * vertexCount) + gx;
                    int bottomRight = bottomLeft + 1;
                    indices[pointer++] = topLeft;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = topRight;
                    indices[pointer++] = topRight;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = bottomRight;
                }
            }

            heightMap.Dispose();

            Model = Loader.LoadToVao(vertices, textureCoords, normals, indices);
        }
        private void UpdateTransformationMatrix()
        {
            TransformationMatrix = MatrixMaths.CreateTransformationMatrix(new vec3(X, 0, Z), 0, 0, 0, 1);
        }
        private vec3 CalculateNormal(int x, int z, Bitmap image)
        {
            float heightL = GetHeight(x - 1, z, image);
            float heightR = GetHeight(x + 1, z, image);
            float heightD = GetHeight(x, z - 1, image);
            float heightU = GetHeight(x, z + 1, image);

            vec3 normal = new vec3(heightL - heightR, 2f, heightD - heightU);
            
            return normal.Normalized;
        }
        private float GetHeight(int x, int z, Bitmap bitmap)
        {
            if (x < 0 || x >= bitmap.Height || z < 0 || z >= bitmap.Width)
            {
                return 0;
            }
            Color pixel = bitmap.GetPixel(x, z);
            float height = 256f * 256 * pixel.R + 256f * pixel.G + pixel.B;
            height -= MaxPixelColor / 2f;
            height /= MaxPixelColor / 2f;
            height *= MaxHeight;
            return height;
        }

        private float BaryCentric(vec3 p1, vec3 p2, vec3 p3, vec2 pos)
        {
            float det = (p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z);
            float l1 = ((p2.z - p3.z) * (pos.x - p3.x) + (p3.x - p2.x) * (pos.y - p3.z)) / det;
            float l2 = ((p3.z - p1.z) * (pos.x - p3.x) + (p1.x - p3.x) * (pos.y - p3.z)) / det;
            float l3 = 1.0f - l1 - l2;
            return l1 * p1.y + l2 * p2.y + l3 * p3.y;
        }
    }
}
