using BlobRPG.MainComponents;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.PostProcessing
{
    public class Fbo
    {
		public int ColorTexture { get; private set; }
		public int DepthTexture { get; private set; }

		private int Width;
		private int Height;

		private int FrameBuffer;


		private int DepthBuffer;
		/*private int ColorBuffer;*/

		public Fbo(int width, int height, Window window, FboDepthType depthBufferType = FboDepthType.None)
		{
			Width = width;
			Height = height;
			InitFrameBuffer(depthBufferType);

			window.Resize += e =>
			{
				Width = e.Width;
				Height = e.Height;

				InitFrameBuffer(depthBufferType);
			};
		}
		public Fbo(Window window, FboDepthType depthBufferType = FboDepthType.None)
		{
			Width = window.ClientSize.X;
			Height = window.ClientSize.Y;
			InitFrameBuffer(depthBufferType);

			window.Resize += e =>
			{
				Width = e.Width;
				Height = e.Height;

				InitFrameBuffer(depthBufferType);
			};
		}
		public Fbo(Window window, ImageRenderer renderer, FboDepthType depthBufferType = FboDepthType.None)
		{
			Width = window.ClientSize.X;
			Height = window.ClientSize.Y;
			InitFrameBuffer(depthBufferType);

			window.Resize += e =>
			{
				Width = e.Width;
				Height = e.Height;

				InitFrameBuffer(depthBufferType);
				renderer.Width = Width;
				renderer.Height = Height;
			};
		}
		public Fbo(int width, int height, FboDepthType depthBufferType = FboDepthType.None)
		{
			Width = width;
			Height = height;
			InitFrameBuffer(depthBufferType);
		}

		public void CleanUp()
		{
			GL.DeleteFramebuffer(FrameBuffer);
			GL.DeleteTexture(ColorTexture);
			GL.DeleteTexture(DepthTexture);
			GL.DeleteRenderbuffer(DepthBuffer);
			/*GL.DeleteRenderbuffer(ColorBuffer);*/
		}

		public void BindFrameBuffer()
		{
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, FrameBuffer);
			GL.Viewport(0, 0, Width, Height);
		}

		public void UnbindFrameBuffer()
		{
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.Viewport(0, 0, Settings.Width, Settings.Height);
		}

		public void BindToRead()
		{
			GL.BindTexture(TextureTarget.Texture2D, 0);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);
			GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
		}

		private void InitFrameBuffer(FboDepthType type)
		{
			CreateFrameBuffer();
			CreateTextureAttachment();
			if (type == FboDepthType.DepthRenderBuffer)
			{
				CreateDepthBufferAttachment();
			}
			else if (type == FboDepthType.DepthTexture)
			{
				CreateDepthTextureAttachment();
			}
			UnbindFrameBuffer();
		}

		private void CreateFrameBuffer()
		{
			FrameBuffer = GL.GenFramebuffer();
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBuffer);
			GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
		}

		private void CreateTextureAttachment()
		{
			ColorTexture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.FramebufferTexture2D( FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTexture, 0);
		}

		private void CreateDepthTextureAttachment()
		{
			DepthTexture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, Width, Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTexture, 0);
		}

		private void CreateDepthBufferAttachment()
		{
			DepthBuffer = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);
			GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, Width, Height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DepthBuffer);
		}
	}
}
