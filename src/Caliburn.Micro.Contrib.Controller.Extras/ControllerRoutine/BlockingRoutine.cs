﻿using System;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.Extras.Converters;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine
{
  public class BlockingRoutine : ControllerRoutineBase,
                                 IMixinControllerRoutine<BlockingRoutine.CanBeBlocked>
  {
    public override void OnViewReady(IScreen screen,
                                     object view)
    {
      base.OnViewReady(screen,
                       view);

      var dependencyObject = (DependencyObject) view;
      var binding = new Binding
                    {
                      Path = new PropertyPath(nameof(ICanBeBlocked.IsBlocked)),
                      Mode = BindingMode.OneWay,
                      Converter = new NegateBoolConverter()
                    };

      BindingOperations.SetBinding(dependencyObject,
                                   UIElement.IsEnabledProperty,
                                   binding);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [CanBeNull]
    public IDisposable Block([NotNull] IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      IDisposable result;

      var canBeBlocked = screen as ICanBeBlocked;
      if (canBeBlocked != null)
      {
        canBeBlocked.IsBlocked = true;
        screen.NotifyOfPropertyChange(nameof(ICanBeBlocked.IsBlocked));

        result = new DisposeAction(() =>
                                   {
                                     canBeBlocked.IsBlocked = false;
                                     screen.NotifyOfPropertyChange(nameof(ICanBeBlocked.IsBlocked));
                                   });
      }
      else
      {
        result = null;
      }

      return result;
    }

    public interface ICanBeBlocked
    {
      bool IsBlocked { get; set; }
    }

    public class CanBeBlocked : ICanBeBlocked
    {
      public virtual bool IsBlocked { get; set; }
    }

    public class DisposeAction : IDisposable
    {
      /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" /></exception>
      public DisposeAction([NotNull] Action action)
      {
        if (action == null)
        {
          throw new ArgumentNullException(nameof(action));
        }
        this.Action = action;
      }

      [CanBeNull]
      private Action Action { get; set; }

      public void Dispose()
      {
        this.Action?.Invoke();
        this.Action = null;
      }
    }
  }
}