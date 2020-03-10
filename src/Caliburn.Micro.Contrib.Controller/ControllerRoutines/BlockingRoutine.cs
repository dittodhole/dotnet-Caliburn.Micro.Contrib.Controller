using System;
using System.Windows;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutines
{
  public sealed class BlockingRoutine : IControllerRoutine
  {
    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnInitialize(IScreen screen) { }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnActivate(IScreen screen) { }

    void IHandleScreenEvents<IScreen>.OnViewReady(IScreen screen,
                                                  object view) { }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnClose(IScreen screen,
                                              bool? dialogResult = null) { }

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
  }
}
