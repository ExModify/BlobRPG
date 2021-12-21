using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.Shadows
{
	public class ShadowFrameBuffer
	{
		public int ShadowMap { get; private set; }

		private readonly int Width;
		private readonly int Height;
		private int FBO;


		public ShadowFrameBuffer(int width, int height)
		{
			Width = width;
			Height = height;
			InitializeFrameBuffer();
		}

		public void CleanUp()
		{
			GL.DeleteFramebuffer(FBO);
			GL.DeleteTexture(ShadowMap);
		}

		public void BindFrameBuffer()
		{
			BindFrameBuffer(FBO, Width, Height);
		}

		public void UnbindFrameBuffer()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.Viewport(0, 0, Settings.Width, Settings.Height);
		}

		private void InitializeFrameBuffer()
		{
			FBO = CreateFrameBuffer();
			ShadowMap = CreateDepthBufferAttachment(Width, Height);
			UnbindFrameBuffer();
		}

		private static void BindFrameBuffer(int frameBuffer, int width, int height)
		{
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, frameBuffer);
			GL.Viewport(0, 0, width, height);
		}
		private static int CreateFrameBuffer()
		{
			int frameBuffer = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
			GL.DrawBuffer(DrawBufferMode.None);
			GL.ReadBuffer(ReadBufferMode.None);
			return frameBuffer;
		}
		private static int CreateDepthBufferAttachment(int width, int height)
		{
			int texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent16, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, texture, 0);
			return texture;
		}
	}
}
