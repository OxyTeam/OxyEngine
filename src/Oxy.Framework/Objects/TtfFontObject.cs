﻿using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Oxy.Framework.Events;
using Oxy.Framework.Rendering;
using Color = System.Drawing.Color;
using PointF = System.Drawing.PointF;

namespace Oxy.Framework.Objects
{
  /// <summary>
  ///   Represents font with given name and size
  /// </summary>
  public class TtfFontObject : FontObject, IDisposable
  {
    private TextRenderer _textRenderer;
    
    public TtfFontObject(Font font)
    {
      Font = font;
    }

    public void Dispose()
    {
      Font?.Dispose();
    }

    public Font Font { get; private set; }

    /// <summary>
    ///   Set new size for font
    /// </summary>
    /// <param name="newSize">New size in pixels</param>
    public void SetSize(float newSize)
    {
      Font = new Font(Font.FontFamily, newSize);
    }

    /// <summary>
    ///   Returns size of the font
    /// </summary>
    /// <returns></returns>
    public float GetSize()
    {
      return Font.Size;
    }

    internal override void Print(SolidBrush brush, System.Drawing.SizeF size, string text, float x = 0, float y = 0, float r = 0, float sx = 1, float sy = 1)
    {
      RedrawRenderer(size, brush, text);
      
      GL.PushMatrix();

      GL.Translate(x, y, 0);
      GL.Rotate(r, Vector3.UnitZ);
      GL.Scale(sx, sy, 1);

      GL.BindTexture(TextureTarget.Texture2D, _textRenderer.Texture);
      GL.Begin(PrimitiveType.Quads);

      GL.TexCoord2(1.0f, 1.0f);
      GL.Vertex3(size.Width, size.Height, 0.1);
      GL.TexCoord2(1.0f, 0.0f);
      GL.Vertex3(size.Width, 0, 0.1);
      GL.TexCoord2(0.0f, 0.0f);
      GL.Vertex3(0, 0, 0.1);
      GL.TexCoord2(0.0f, 1.0f);
      GL.Vertex3(0, size.Height, 0.1);

      GL.End();
      GL.BindTexture(TextureTarget.Texture2D, 0);

      GL.PopMatrix();
    }
    
    private void RedrawRenderer(System.Drawing.SizeF newSize, SolidBrush brush, string text)
    {
      _textRenderer?.Dispose();
      _textRenderer = new TextRenderer((int) newSize.Width, (int) newSize.Height);
      _textRenderer.Clear(Color.Transparent);
      _textRenderer.DrawString(text, Font, brush, new PointF(0, 0));
    }
  }
}