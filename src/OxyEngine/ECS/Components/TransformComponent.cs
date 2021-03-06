﻿using Microsoft.Xna.Framework;
using OxyEngine.Dependency;
using OxyEngine.Ecs.Interfaces;
using OxyEngine.Graphics;

namespace OxyEngine.Ecs.Components
{
  /// <summary>
  ///   Component for manipulating transformation (position, rotation, scale) of a gamobject
  /// </summary>
  public class TransformComponent : GameComponent, ITransformable
  {
    private GraphicsManager _graphicsManager;
    
    public TransformComponent()
    {
      _transformation = new Transformation();
      _graphicsManager = Container.Instance.ResolveByName<GraphicsManager>(InstanceName.GraphicsManager);
    }

    #region Local transformation

    public Vector2 Position
    {
      get => _transformation.Position;
      set => _transformation.Translate(value - Position);
    }

    public float Rotation
    {
      get => _transformation.Rotation;
      set => _transformation.Rotate(value - Rotation);
    }

    public Vector2 Scale
    {
      get => _transformation.Scale;
      set => _transformation.Zoom(value / Scale);
    }

    public Matrix Matrix => _transformation.Matrix;

    #endregion

    #region Global transformation

    public Vector2 GlobalPosition
    {
      get => Entity.Parent != null
        ? Vector2.Transform(Position, Entity.Parent.GetComponent<TransformComponent>().Matrix)
        : Position;
      set => Position = Entity.Parent != null
        ? value - GlobalPosition
        : value;
    }

    public float GlobalRotation     
    {
      get => Entity.Parent != null
        ? Entity.Parent.GetComponent<TransformComponent>().Rotation + Rotation
        : Rotation;
      set => Rotation = Entity.Parent != null
        ? value - GlobalRotation
        : value;
    }

    public Vector2 GlobalScale
    {
      get => Entity.Parent != null
        ? Entity.Parent.GetComponent<TransformComponent>().Scale * Scale
        : Scale;
      set => Scale = Entity.Parent != null
        ? value / GlobalScale
        : value;
    }

    public Matrix GlobalMatrix => Entity.Parent != null
      ? Entity.Parent.GetComponent<TransformComponent>().Matrix * Matrix
      : Matrix;

    #endregion

    private Transformation _transformation;

    public void Translate(float x, float y)
    {
      Translate(new Vector2(x, y));
    }

    public void Translate(Vector2 vector)
    {
      _transformation.Translate(vector);
    }
  
    public void Rotate(float angle)
    {
      _transformation.Rotate(angle);
    }
  
    public void Zoom(float x, float y)
    {
      Zoom(new Vector2(x, y));
    }

    public void Zoom(Vector2 vector)
    {
      _transformation.Zoom(vector);
    }

    /// <summary>
    ///   Adds transformation of an object to transformation stack
    /// </summary>
    public void AttachTransformation()
    {
      _graphicsManager.PushMatrix();
      _graphicsManager.Translate(Position.X, Position.Y);
      _graphicsManager.Rotate(Rotation);
      _graphicsManager.Scale(Scale.X, Scale.Y);
    }
    
    /// <summary>
    ///   Removes transformation of an object to transformation stack
    /// </summary>
    public void DetachTransformation()
    {
      _graphicsManager.PopMatrix();
    }
  }
}