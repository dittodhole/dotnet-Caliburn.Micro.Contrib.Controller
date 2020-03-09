using System;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro.Contrib.Controller.Extras.Converters;

namespace Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine
{
  public class BlockingRoutine : IControllerRoutine,
                                 IMixinInstance<BlockingRoutine.ICanBeBlocked>,
                                 IMixinInterface<BlockingRoutine.ICanBeBlocked>
  {
    public virtual ICanBeBlocked CreateMixinInstance()
    {
      var instance = new CanBeBlocked();

      return instance;
    }

    /// <inheritdoc/>
    public void OnViewReady(IScreen screen,
                            object view)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (view == null)
      {
        throw new ArgumentNullException(nameof(view));
      }

      var binding = new Binding
                    {
                      Path = new PropertyPath(nameof(ICanBeBlocked.IsBlocked)),
                      Mode = BindingMode.OneWay,
                      Converter = new NegateBoolConverter()
                    };

      Execute.OnUIThread(() =>
                         { // TODO verify, if execution on UI thread is needed
                           var dependencyObject = (DependencyObject) view;

                           BindingOperations.SetBinding(dependencyObject,
                                                        UIElement.IsEnabledProperty,
                                                        binding);
                         });
    }

    /// <exception cref="ArgumentNullException"/>
    public virtual IDisposable? Block(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      DisposeAction? result = null;

      var canBeBlocked = screen as ICanBeBlocked;
      if (canBeBlocked != null)
      {
        canBeBlocked.IsBlocked = true;
        screen.NotifyOfPropertyChange(nameof(ICanBeBlocked.IsBlocked));

        result = new DisposeAction(instance =>
                                   {
                                     canBeBlocked.IsBlocked = false;
                                     screen.NotifyOfPropertyChange(nameof(ICanBeBlocked.IsBlocked));

                                     this.DisposeActions.Remove(result);
                                   });

        this.DisposeActions.Add(result);
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
      public bool IsBlocked { get; set; }
    }
  }
}
