using System;
using System.Collections.Generic;
using Autofac;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac.ControllerRoutine
{
  public class LifetimeDisposalRoutine : ControllerRoutineBase,
                                         IDisposable
  {
    [NotNull]
    private LinkedList<ScreenWithLifetimeScope> ScreenWithLifetimeScopes { get; } = new LinkedList<ScreenWithLifetimeScope>();

    public virtual void Dispose()
    {
      LinkedListNode<ScreenWithLifetimeScope> node;
      while ((node = this.ScreenWithLifetimeScopes.First) != null)
      {
        var screenWithLifetimeScope = node.Value;
        screenWithLifetimeScope.Dispose();

        this.ScreenWithLifetimeScopes.Remove(node);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public override void OnDeactivate(IScreen screen,
                                      bool close)
    {
      base.OnDeactivate(screen,
                        close);

      if (close)
      {
        var node = this.ScreenWithLifetimeScopes.First;
        while (node != null)
        {
          var previousNode = node;

          node = node.Next;

          var screenWithLifetimeScope = previousNode.Value;
          if (screenWithLifetimeScope.DisposeIfMatches(screen))
          {
            this.ScreenWithLifetimeScopes.Remove(previousNode);
            break;
          }
        }
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="lifetimeScope" /> is <see langword="null" /></exception>
    public virtual void RegisterLifetimeScope([NotNull] IScreen screen,
                                              [NotNull] ILifetimeScope lifetimeScope)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (lifetimeScope == null)
      {
        throw new ArgumentNullException(nameof(lifetimeScope));
      }

      var screenWithLifetimeScope = new ScreenWithLifetimeScope(screen,
                                                                lifetimeScope);

      this.ScreenWithLifetimeScopes.AddLast(screenWithLifetimeScope);
    }

    private class ScreenWithLifetimeScope : IDisposable
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="lifetimeScope" /> is <see langword="null" /></exception>
      public ScreenWithLifetimeScope([NotNull] IScreen screen,
                                     [NotNull] ILifetimeScope lifetimeScope)
      {
        if (screen == null)
        {
          throw new ArgumentNullException(nameof(screen));
        }
        if (lifetimeScope == null)
        {
          throw new ArgumentNullException(nameof(lifetimeScope));
        }
        this.WeakReference = new WeakReference(screen);
        this.LifetimeScope = lifetimeScope;
      }

      [NotNull]
      private WeakReference WeakReference { get; }

      [NotNull]
      private ILifetimeScope LifetimeScope { get; }

      public void Dispose()
      {
        this.LifetimeScope.Dispose();
      }

      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      public bool DisposeIfMatches([NotNull] IScreen screen)
      {
        if (screen == null)
        {
          throw new ArgumentNullException(nameof(screen));
        }

        if (this.WeakReference.Target == screen)
        {
          this.LifetimeScope.Dispose();
          return true;
        }

        return false;
      }
    }
  }
}
