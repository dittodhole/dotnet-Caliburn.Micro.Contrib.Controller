using System;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.Extras.Converters;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine
{
  public class BlockingRoutine : ControllerRoutineBase,
                                 IControllerRoutineMixin<BlockingRoutine.CanBeBlocked>,
                                 IDisposable
  {
    [NotNull]
    private IWeakCollection<DisposeAction> DisposeActions { get; } = new WeakCollection<DisposeAction>();

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
    public virtual IDisposable Block([NotNull] IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      DisposeAction result;

      var canBeBlocked = screen as ICanBeBlocked;
      if (canBeBlocked != null)
      {
        canBeBlocked.IsBlocked = true;
        screen.NotifyOfPropertyChange(nameof(ICanBeBlocked.IsBlocked));

        result = new DisposeAction(instance =>
                                   {
                                     canBeBlocked.IsBlocked = false;
                                     screen.NotifyOfPropertyChange(nameof(ICanBeBlocked.IsBlocked));
                                   });

        this.DisposeActions.Add(result);
      }
      else
      {
        result = null;
      }

      return result;
    }

    internal interface ICanBeBlocked
    {
      bool IsBlocked { get; set; }
    }

    internal class CanBeBlocked : ICanBeBlocked,
                                  IMixin
    {
      public bool IsBlocked { get; set; }

      public CustomAttributeBuilder[] GetCustomAttributeBuilders()
      {
        return new CustomAttributeBuilder[0];
      }
    }

    public override void Dispose()
    {
      this.DisposeActions.Dispose();
    }
  }
}
