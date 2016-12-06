﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  // TODO clarify correctness
  public interface IWeakCollection<T> : IDisposable
    where T : class
  {
    void Add([NotNull] T instance);

    /// <exception cref="ArgumentNullException"><paramref name="instance" /> is <see langword="null" /></exception>
    void Remove([NotNull] T instance);
  }

  public class WeakCollection<T> : IWeakCollection<T>
    where T : class
  {
    [NotNull]
    private LinkedList<WeakInstance> WeakInstances { get; } = new LinkedList<WeakInstance>();

    public virtual void Dispose()
    {
      LinkedListNode<WeakInstance> node;
      while ((node = this.WeakInstances.First) != null)
      {
        var weakInstance = node.Value;
        weakInstance.Dispose();

        this.WeakInstances.Remove(node);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="instance" /> is <see langword="null" /></exception>
    public void Add(T instance)
    {
      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      var weakInstance = new WeakInstance(instance);

      this.WeakInstances.AddLast(weakInstance);
    }

    /// <exception cref="ArgumentNullException"><paramref name="instance" /> is <see langword="null" /></exception>
    public void Remove(T instance)
    {
      if (instance == null)
      {
        throw new ArgumentNullException(nameof(instance));
      }

      var weakInstance = this.WeakInstances.Find(new WeakInstance(instance));
      if (weakInstance != null)
      {
        this.WeakInstances.Remove(weakInstance);
      }
    }

    private class WeakInstance : IDisposable,
                                 IEquatable<WeakInstance>
    {
      /// <exception cref="ArgumentNullException"><paramref name="instance" /> is <see langword="null" /></exception>
      public WeakInstance([NotNull] T instance)
      {
        if (instance == null)
        {
          throw new ArgumentNullException(nameof(instance));
        }
        this.Instance = new WeakReference(instance);
      }

      [NotNull]
      private WeakReference Instance { get; }

      public void Dispose()
      {
        var instance = this.Instance.Target as IDisposable;
        instance?.Dispose();

        this.Instance.Target = null;
      }

      public bool Equals(WeakInstance other)
      {
        return this.Instance.Target?.Equals(other?.Instance?.Target) ?? false;
      }

      public override bool Equals(object obj)
      {
        if (object.ReferenceEquals(null,
                                   obj))
        {
          return false;
        }
        if (object.ReferenceEquals(this,
                                   obj))
        {
          return true;
        }
        if (obj.GetType() != this.GetType())
        {
          return false;
        }
        return this.Equals((WeakInstance) obj);
      }

      public override int GetHashCode()
      {
        return this.Instance.Target?.GetHashCode() ?? this.Instance.GetHashCode();
      }
    }
  }
}
