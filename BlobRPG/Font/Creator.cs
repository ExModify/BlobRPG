using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Font
{
    class Creator
    {
		public const double LineHeight = 0.03f;
		public const int SpaceAscii = 32;

		private readonly MetaFile Metadata;

		public Creator(Stream file, Window window)
		{
			Metadata = new MetaFile(file, window);
		}

		public TextMeshData CreateTextMesh(GUIText text)
		{
			List<Line> lines = CreateStructure(text);
			TextMeshData data = CreateQuadVertices(text, lines);
			return data;
		}

		private List<Line> CreateStructure(GUIText text)
		{
			List<Line> lines = new List<Line>();
			Line currentLine = new Line(Metadata.SpaceWidth, text.FontSize, text.MaxLineLength);
			Word currentWord = new Word(text.FontSize);

			foreach (char c in text.TextString)
			{
				int ascii = c;
				if (ascii == SpaceAscii)
				{
					bool added = currentLine.TryAddWord(currentWord);
					if (!added)
					{
						lines.Add(currentLine);
						currentLine = new Line(Metadata.SpaceWidth, text.FontSize, text.MaxLineLength);
						currentLine.TryAddWord(currentWord);
					}
					currentWord = new Word(text.FontSize);
					continue;
				}
				Character character = Metadata.GetCharacter(ascii);
				currentWord.AddCharacter(character);
			}
			CompleteStructure(lines, currentLine, currentWord, text);
			return lines;
		}

		private void CompleteStructure(List<Line> lines, Line currentLine, Word currentWord, GUIText text)
		{
			bool added = currentLine.TryAddWord(currentWord);
			if (!added)
			{
				lines.Add(currentLine);
				currentLine = new Line(Metadata.SpaceWidth, text.FontSize, text.MaxLineLength);
				currentLine.TryAddWord(currentWord);
			}
			lines.Add(currentLine);
		}

		private TextMeshData CreateQuadVertices(GUIText text, List<Line> lines)
		{
			text.NumberOfLines = lines.Count;
			double curserX = 0f;
			double curserY = 0f;
			List<float> vertices = new List<float>();
			List<float> textureCoords = new List<float>();
			foreach (Line line in lines)
			{
				if (text.Centered)
				{
					curserX = (line.MaxLength - line.CurrentLineLength) / 2;
				}
				foreach (Word word in line.Words)
				{
					foreach (Character letter in word.Characters)
					{
						AddVerticesForCharacter(curserX, curserY, letter, text.FontSize, vertices);
						AddTexCoords(textureCoords, letter.XTextureCoord, letter.YTextureCoord,
								letter.XMaxTextureCoord, letter.YMaxTextureCoord);
						curserX += letter.XAdvance * text.FontSize;
					}
					curserX += Metadata.SpaceWidth * text.FontSize;
				}
				curserX = 0;
				curserY += LineHeight * text.FontSize;
			}
			return new TextMeshData(vertices.ToArray(), textureCoords.ToArray());
		}

		private void AddVerticesForCharacter(double curserX, double curserY, Character character, double fontSize, List<float> vertices)
		{
			double x = curserX + (character.XOffset * fontSize);
			double y = curserY + (character.YOffset * fontSize);
			double maxX = x + (character.SizeX * fontSize);
			double maxY = y + (character.SizeY * fontSize);
			double properX = (2 * x) - 1;
			double properY = (-2 * y) + 1;
			double properMaxX = (2 * maxX) - 1;
			double properMaxY = (-2 * maxY) + 1;

			AddVertices(vertices, properX, properY, properMaxX, properMaxY);
		}

		private static void AddVertices(List<float> vertices, double x, double y, double maxX, double maxY)
		{
			vertices.Add((float)x);
			vertices.Add((float)y);
			vertices.Add((float)x);
			vertices.Add((float)maxY);
			vertices.Add((float)maxX);
			vertices.Add((float)maxY);
			vertices.Add((float)maxX);
			vertices.Add((float)maxY);
			vertices.Add((float)maxX);
			vertices.Add((float)y);
			vertices.Add((float)x);
			vertices.Add((float)y);
		}

		private static void AddTexCoords(List<float> texCoords, double x, double y, double maxX, double maxY)
		{
			texCoords.Add((float)x);
			texCoords.Add((float)y);
			texCoords.Add((float)x);
			texCoords.Add((float)maxY);
			texCoords.Add((float)maxX);
			texCoords.Add((float)maxY);
			texCoords.Add((float)maxX);
			texCoords.Add((float)maxY);
			texCoords.Add((float)maxX);
			texCoords.Add((float)y);
			texCoords.Add((float)x);
			texCoords.Add((float)y);
		}
	}
}
