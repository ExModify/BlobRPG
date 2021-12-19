using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.WavefrontOBJ.Models
{
    public class Vertex
    {
        private const int NO_INDEX = -1;

        public vec3 Position { get; private set; }
        public int TextureIndex { get; set; } = NO_INDEX;
        public int NormalIndex { get; set; } = NO_INDEX;
        public Vertex DuplicateVertex { get; set; }
        public int Index { get; private set; }
        public float Length { get; private set; }
        public List<vec3> Tangents { get; private set; }
        public vec3 AveragedTangent { get; set; } = new vec3();

        public bool IsSet { get => TextureIndex != NO_INDEX && NormalIndex != NO_INDEX; }

        public Vertex(int index, vec3 position)
        {
            Tangents = new List<vec3>();

            Index = index;
            Position = position;
            Length = position.Length;
        }

        public void AddTangent(vec3 tangent)
        {
            Tangents.Add(tangent);
        }

        public Vertex Duplicate(int newIndex)
        {
            Vertex vertex = new Vertex(newIndex, Position)
            {
                Tangents = Tangents
            };
            return vertex;
        }

        public vec3 AverageTangents()
        {
            if (Tangents.Count == 0) return AveragedTangent;

            for (int i = 0; i < Tangents.Count; i++)
            {
                AveragedTangent += Tangents[i];
            }

            AveragedTangent = AveragedTangent.Normalized;
            return AveragedTangent;
        }

        public bool IsSameIndex(int texture, int normal)
            => TextureIndex == texture && NormalIndex == normal;


    }
}
