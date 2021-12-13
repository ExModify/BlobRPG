using BlobRPG.MainComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Font
{
    class MetaFile
    {
		private const int PadTop = 0;
		private const int PadLeft = 1;
		private const int PadBottom = 2;
		private const int PadRight = 3;
		private const int DesiredPadding = 8;
		private const char Separator = ' ';
		private const char NumberSeparator = ',';


		private double AspectRatio { get; set; }
		

		public double VerticalPerPixelSize { get; private set; }
		public double HorizontalPerPixelSize { get; private set; }
		public double SpaceWidth { get; set; }
		public int[] Padding { get; private set; }
		public int PaddingWidth { get; private set; }
		public int PaddingHeight { get; private set; }

		public Dictionary<int, Character> Metadata { get; }

		private StreamReader Reader { get; }
		private Dictionary<string, string> Values { get; }

		public MetaFile(Stream file, Window window)
		{
			AspectRatio = window.ClientSize.X / (double)window.ClientSize.Y;
			Metadata = new Dictionary<int, Character>();
			Values = new Dictionary<string, string>();

			Reader = new StreamReader(file);

			LoadPaddingData();
			LoadLineSizes();
			int imageWidth = GetValue("scaleW").Value;
			LoadCharacterData(imageWidth);
			Reader.Close();
		}


		public Character GetCharacter(int ascii)
		{
			return Metadata[ascii];
		}
		private bool ProcessNextLine()
		{
			Values.Clear();
			string line = Reader.ReadLine();
			if (line == null) return false;
			string[] split = line.Split(Separator);
			foreach (string part in split)
			{
				string[] valuePairs = part.Split('=');
				if (valuePairs.Length == 2)
				{
					Values.Add(valuePairs[0], valuePairs[1]);
				}
			}
			return true;
		}

		private int? GetValue(string key)
		{
			if (!Values.ContainsKey(key))
				return null;
			return int.Parse(Values[key]);
		}

		private int[] GetValues(string variable)
		{
			if (!Values.ContainsKey(variable))
				return null;

			string[] numbers = Values[variable].Split(NumberSeparator);
			int[] actualValues = new int[numbers.Length];
			for (int i = 0; i < actualValues.Length; i++)
			{
				actualValues[i] = int.Parse(numbers[i]);
			}
			return actualValues;
		}

		private void LoadPaddingData()
		{
			ProcessNextLine();
			Padding = GetValues("padding");
			PaddingWidth = Padding[PadLeft] + Padding[PadRight];
			PaddingHeight = Padding[PadTop] + Padding[PadBottom];
		}

		private void LoadLineSizes()
		{
			ProcessNextLine();
			int lineHeightPixels = GetValue("lineHeight").Value - PaddingHeight;
			VerticalPerPixelSize = Creator.LineHeight / (double)lineHeightPixels;
			HorizontalPerPixelSize = VerticalPerPixelSize / AspectRatio;
		}

		private void LoadCharacterData(int width)
		{
			ProcessNextLine();
			ProcessNextLine();
			while (ProcessNextLine())
			{
				Character c = LoadCharacter(width);
				if (c != null)
				{
					Metadata.Add(c.Id, c);
				}
			}
		}

		private Character LoadCharacter(int size)
		{
			int? id = GetValue("id");
			if (!id.HasValue)
				return null;
			if (id == Creator.SpaceAscii)
			{
				SpaceWidth = (GetValue("xadvance").Value - PaddingWidth) * HorizontalPerPixelSize;
				return null;
			}
			double xTex = ((double)GetValue("x").Value + (Padding[PadLeft] - DesiredPadding)) / size;
			double yTex = ((double)GetValue("y").Value + (Padding[PadTop] - DesiredPadding)) / size;
			int width = GetValue("width").Value - (PaddingWidth - (2 * DesiredPadding));
			int height = GetValue("height").Value - ((PaddingHeight) - (2 * DesiredPadding));
			double quadWidth = width * HorizontalPerPixelSize;
			double quadHeight = height * VerticalPerPixelSize;
			double xTexSize = (double)width / size;
			double yTexSize = (double)height / size;
			double xOff = (GetValue("xoffset").Value + Padding[PadLeft] - DesiredPadding) * HorizontalPerPixelSize;
			double yOff = (GetValue("yoffset").Value + (Padding[PadTop] - DesiredPadding)) * VerticalPerPixelSize;
			double xAdvance = (GetValue("xadvance").Value - PaddingWidth) * HorizontalPerPixelSize;
			return new Character(id.Value, xTex, yTex, xTexSize, yTexSize, xOff, yOff, quadWidth, quadHeight, xAdvance);
		}
	}
}
