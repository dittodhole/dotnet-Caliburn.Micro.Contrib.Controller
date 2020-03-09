using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IProvideScreenEventHandlers<TScreen>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    void OnClose(TScreen screen,
                 bool? dialogResult = null);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    void OnInitialize(TScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    void OnViewReady(TScreen screen,
                     object view);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    /// <remarks>Should be used to attach events</remarks>
    void OnActivate(TScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    /// <remarks>Should be used to detach events</remarks>
    void OnDeactivate(TScreen screen,
                      bool close);
  }
}
