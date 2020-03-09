using System;
using System.Windows;

namespace Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine
{
  public sealed class BlockingRoutine : IControllerRoutine
  {
    /// <inheritdoc/>
    public void OnInitialize(IScreen screen) { }

    /// <inheritdoc/>
    public void OnActivate(IScreen screen) { }

    public void OnViewReady(IScreen screen,
                            object view) { }

    /// <inheritdoc/>
    public void OnClose(IScreen screen,
                        bool? dialogResult = null) { }

    /// <inheritdoc/>
    public void OnDeactivate(IScreen screen,
                             bool close) { }

    /// <exception cref="ArgumentNullException"/>
    public IDisposable Block(IViewAware viewAware)
    {
      if (viewAware == null)
      {
        throw new ArgumentNullException(nameof(viewAware));
      }

      var uiElement = (UIElement) viewAware.GetView();

      Execute.OnUIThread(() => uiElement.SetValue(UIElement.IsEnabledProperty,
                                                  false));

      DisposeAction? result = null;

      var canBeBlocked = viewAware as ICanBeBlocked;
      if (canBeBlocked != null)
      {
        canBeBlocked.IsBlocked = true;
        viewAware.NotifyOfPropertyChange(nameof(ICanBeBlocked.IsBlocked));

        result = new DisposeAction(instance =>
                                   {
                                     canBeBlocked.IsBlocked = false;
                                     viewAware.NotifyOfPropertyChange(nameof(ICanBeBlocked.IsBlocked));

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
  }
}
