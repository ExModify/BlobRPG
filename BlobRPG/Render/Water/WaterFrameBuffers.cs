using BlobRPG.MainComponents;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.Water
{
    public class WaterFrameBuffers
    {
		private int ReflectionWidth = 320;
		private int ReflectionHeight = 180;

		private int RefractionWidth = 1280;
		private int RefractionHeight = 720;

		public int ReflectionFrameBuffer { get; private set; }
		public int ReflectionTexture { get; private set; }
		public int ReflectionDepthBuffer { get; private set; }

		public int RefractionFrameBuffer { get; private set; }
		public int RefractionTexture { get; private set; }
		public int RefractionDepthTexture { get; private set; }

		public WaterFrameBuffers(Window window)
        {
			RefractionWidth = window.Size.X;
			RefractionHeight = window.Size.Y;

			ReflectionWidth = RefractionWidth / 4;
			ReflectionHeight = RefractionHeight / 4;

			window.Resize += (e) =>
			{
				RefractionWidth = e.Width;
				RefractionHeight = e.Height;

				ReflectionWidth = RefractionWidth / 4;
				ReflectionHeight = RefractionHeight / 4;

				InitReflectionFB();
				InitReferactionFB();
			};

			InitReflectionFB();
			InitReferactionFB();
		}
		public void CleanUp()
		{
			GL.DeleteFramebuffer(ReflectionFrameBuffer);
			GL.DeleteTexture(ReflectionTexture);
			GL.DeleteRenderbuffer(ReflectionDepthBuffer);
			GL.DeleteFramebuffer(RefractionFrameBuffer);
			GL.DeleteTexture(RefractionTexture);
			GL.DeleteTexture(RefractionDepthTexture);
		}
		public void BindReflectionFB()
		{
            BindFB(ReflectionFrameBuffer, ReflectionWidth, ReflectionHeight);
		}

		public void BindRefractionFB()
		{
            BindFB(RefractionFrameBuffer, RefractionWidth, RefractionHeight);
		}

		public void UnbindCurrentFB()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.Viewport(0, 0, Settings.Width, Settings.Height);
		}

		private void InitReflectionFB()
		{
			ReflectionFrameBuffer = CreateFrameBuffer();
			ReflectionTexture = CreateTextureAttachment(ReflectionWidth, ReflectionHeight);
			ReflectionDepthBuffer = CreateDepthBufferAttachment(ReflectionWidth, ReflectionHeight);
			UnbindCurrentFB();
		}

		private void InitReferactionFB()
		{
			RefractionFrameBuffer = CreateFrameBuffer();
			RefractionTexture = CreateTextureAttachment(RefractionWidth, RefractionHeight);
			RefractionDepthTexture = CreateDepthTextureAttachment(RefractionWidth, RefractionHeight);
			UnbindCurrentFB();
		}

		private static void BindFB(int frameBuffer, int width, int height)
		{
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
			GL.Viewport(0, 0, width, height);
		}

		private static int CreateFrameBuffer()
		{
			int frameBuffer = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
			GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
			return frameBuffer;
		}

		private static int CreateTextureAttachment(int width, int height)
		{
			int texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture, 0);
			return texture;
		}

		private static int CreateDepthTextureAttachment(int width, int height)
		{
			int texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, texture, 0);
			return texture;
		}

		private static int CreateDepthBufferAttachment(int width, int height)
		{
			int depthBuffer = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);
			return depthBuffer;
		}

	}
}
