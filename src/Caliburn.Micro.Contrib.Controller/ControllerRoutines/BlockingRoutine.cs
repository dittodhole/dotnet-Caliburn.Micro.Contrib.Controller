using Caliburn.Micro.Contrib.Controller.Converters;
using System;
using System.Windows;
using System.Windows.Data;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutines
{
  public sealed class BlockingRoutine : IControllerRoutine
  {
    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnInitialize(IScreen screen) { }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnActivate(IScreen screen) { }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnViewReady(IScreen screen,
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

      if (screen is ICanBeBlocked canBeBlocked)
      if (view is DependencyObject dependencyObject)
      {
        var binding = new Binding
                      {
                        Path = new PropertyPath(nameof(canBeBlocked.IsBlocked)),
                        Mode = BindingMode.OneWay,
                        Converter = new NegateBoolConverter()
                      };

        Execute.OnUIThread(() =>
                           {
                             BindingOperations.SetBinding(dependencyObject,
                                                          UIElement.IsEnabledProperty,
                                                          binding);
                           });
      }
    }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnClose(IScreen screen,
                                              bool? dialogResult) { }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnDeactivate(IScreen screen,
                                                   bool close) { }

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public IDisposable Block(IViewAware viewAware)
    {
      if (viewAware == null)
      {
        throw new ArgumentNullException(nameof(viewAware));
      }

      var uiElement = (UIElement) viewAware.GetView();

      Execute.OnUIThread(() => uiElement.SetValue(UIElement.IsEnabledProperty,
                                                  false));

      var result = new DisposeAction(() => Execute.OnUIThread(() => uiElement.SetValue(UIElement.IsEnabledProperty,
                                                                                       true)));

      return result;
    }

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public IDisposable Block(ICanBeBlocked canBeBlocked)
    {
      if (canBeBlocked == null)
      {
        throw new ArgumentNullException(nameof(canBeBlocked));
      }

      canBeBlocked.IsBlocked = true;

      var result = new DisposeAction(() => canBeBlocked.IsBlocked = false);

      return result;
    }

    public interface ICanBeBlocked
    {
      bool IsBlocked { get; set; }
    }
  }
}
