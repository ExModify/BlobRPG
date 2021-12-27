using BlobRPG.LoggerComponents;
using BlobRPG.MainComponents;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.PostProcessing
{
    public class Fbo : ILogger
    {
		public int ColorTexture { get; private set; }
		public int DepthTexture { get; private set; }

		public FboDepthType DepthBufferType { get; private set; }

		public bool Multisample { get; private set; }

		private int Width;
		private int Height;

		private int FrameBuffer;

		private int DepthBuffer;
		private int ColorBuffer;

		public Fbo(int width, int height, Window window, FboDepthType depthBufferType = FboDepthType.None, bool multisampled = false)
		{
			Width = width;
			Height = height;
			DepthBufferType = depthBufferType;
			Multisample = multisampled;

			if (!DepthBufferType.HasFlag(FboDepthType.DepthRenderBuffer) && multisampled)
            {
				Log(Warning, "Depth render buffer flag wasn't specified when creating the FBO. Automatically adding..");
				DepthBufferType |= FboDepthType.DepthRenderBuffer;
			}
			InitFrameBuffer(DepthBufferType);

			window.Resize += e =>
			{
				Width = e.Width;
				Height = e.Height;

				InitFrameBuffer(DepthBufferType);
			};
		}
		public Fbo(Window window, FboDepthType depthBufferType = FboDepthType.None, bool multisampled = false)
		{
			Width = window.ClientSize.X;
			Height = window.ClientSize.Y;
			DepthBufferType = depthBufferType;
			Multisample = multisampled;

			if (!DepthBufferType.HasFlag(FboDepthType.DepthRenderBuffer) && multisampled)
			{
				Log(Warning, "Depth render buffer flag wasn't specified when creating the FBO. Automatically adding..");
				DepthBufferType |= FboDepthType.DepthRenderBuffer;
			}
			InitFrameBuffer(DepthBufferType);

			window.Resize += e =>
			{
				Width = e.Width;
				Height = e.Height;

				InitFrameBuffer(DepthBufferType);
			};
		}
		public Fbo(Window window, ImageRenderer renderer, FboDepthType depthBufferType = FboDepthType.None, bool multisampled = false)
		{
			Width = (int)(window.ClientSize.X * renderer.Multiplier);
			Height = (int)(window.ClientSize.Y * renderer.Multiplier);
			DepthBufferType = depthBufferType;
			Multisample = multisampled;

			if (!DepthBufferType.HasFlag(FboDepthType.DepthRenderBuffer) && multisampled)
			{
				Log(Warning, "Depth render buffer flag wasn't specified when creating the FBO. Automatically adding..");
				DepthBufferType |= FboDepthType.DepthRenderBuffer;
			}
			InitFrameBuffer(DepthBufferType);

			window.Resize += e =>
			{
				Width = (int)(e.Width * renderer.Multiplier);
				Height = (int)(e.Height * renderer.Multiplier);

				InitFrameBuffer(DepthBufferType);
				renderer.Width = Width;
				renderer.Height = Height;
			};
		}
		public Fbo(int width, int height, FboDepthType depthBufferType = FboDepthType.None, bool multisampled = false)
		{
			Width = width;
			Height = height;
			DepthBufferType = depthBufferType;
			Multisample = multisampled;

			if (!DepthBufferType.HasFlag(FboDepthType.DepthRenderBuffer) && multisampled)
			{
				Log(Warning, "Depth render buffer flag wasn't specified when creating the FBO. Automatically adding..");
				DepthBufferType |= FboDepthType.DepthRenderBuffer;
			}
			InitFrameBuffer(DepthBufferType);
		}

		public void CleanUp()
		{
			GL.DeleteFramebuffer(FrameBuffer);
			GL.DeleteTexture(ColorTexture);
			GL.DeleteTexture(DepthTexture);
			GL.DeleteRenderbuffer(DepthBuffer);
			GL.DeleteRenderbuffer(ColorBuffer);
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

		public void ResolveToFbo(Fbo output)
        {
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, output.FrameBuffer);
			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, FrameBuffer);
			GL.BlitFramebuffer(0, 0, Width, Height, 0, 0, output.Width, output.Height, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
			UnbindFrameBuffer();
		}
		public void ResolveToScreen()
		{
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, FrameBuffer);
			GL.DrawBuffer(DrawBufferMode.Back);
			GL.BlitFramebuffer(0, 0, Width, Height, 0, 0, Settings.Width, Settings.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
			UnbindFrameBuffer();
		}

		private void InitFrameBuffer(FboDepthType type)
		{
			CreateFrameBuffer();
			if (Multisample)
            {
				CreateMultisampleColorAttachment();
			}
            else
			{
				CreateTextureAttachment();
			}
			if (type.HasFlag(FboDepthType.DepthRenderBuffer))
			{
				CreateDepthBufferAttachment();
			}
			if (type.HasFlag(FboDepthType.DepthTexture))
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
		private void CreateMultisampleColorAttachment()
        {
			ColorBuffer = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, ColorBuffer);
			GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, Settings.MSAA, RenderbufferStorage.Rgba8, Width, Height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, ColorBuffer);
		}
		private void CreateDepthBufferAttachment()
		{
			DepthBuffer = GL.GenRenderbuffer();
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBuffer);
			if (!Multisample)
				GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, Width, Height);
			else
				GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, Settings.MSAA, RenderbufferStorage.DepthComponent24, Width, Height);
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DepthBuffer);
		}
	}
}
