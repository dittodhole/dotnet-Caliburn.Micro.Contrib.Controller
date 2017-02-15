using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IProvideScreenEventHandlers<TScreen>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    void OnClose([NotNull] TScreen screen,
                 bool? dialogResult = null);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    void OnInitialize([NotNull] TScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    void OnViewReady([NotNull] TScreen screen,
                     [NotNull] object view);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to attach events</remarks>
    void OnActivate([NotNull] TScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to detach events</remarks>
    void OnDeactivate([NotNull] TScreen screen,
                      bool close);
  }
}
