using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public interface IRoutine
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    void OnInitialize([NotNull] IScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    /// <remarks>Should be used to attach events</remarks>
    void OnActivate([NotNull] IScreen screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    /// <remarks>Should be used to detach events</remarks>
    void OnDeactivate([NotNull] IScreen screen,
                      bool close);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    /// <remarks>Should be used for funky UI stuff (like initial validation, initial focus, ... stuff ... :beers:)</remarks>
    [NotNull]
    Task OnViewReadyAsync([NotNull] IScreen screen,
                          [NotNull] object view);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    void OnClose([NotNull] IScreen screen,
                 bool? dialogResult = null);
  }
}
