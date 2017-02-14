using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IProvideScreenEventHandlers
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    void OnViewReady([NotNull] IScreen screen,
                     [NotNull] object view);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    void OnActivate([NotNull] IScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    void OnDeactivate([NotNull] IScreen screen,
                      bool close);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    void OnInitialize([NotNull] IScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    void OnClose([NotNull] IScreen screen,
                 bool? dialogResult = null);
  }

  public interface IProvideScreenEventHandlers<TScreen> : IProvideScreenEventHandlers
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    void OnClose([NotNull] TScreen screen,
                 bool? dialogResult = null);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    void OnInitialize([NotNull] TScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="sceen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    void OnViewReady([NotNull] TScreen sceen,
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
