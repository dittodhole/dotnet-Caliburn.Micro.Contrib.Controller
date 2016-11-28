using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IInterceptScreenEvents
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    void OnInitialize([NotNull] object screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    /// <remarks>Should be used to attach events</remarks>
    void OnActivate([NotNull] object screen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    /// <remarks>Should be used to detach events</remarks>
    void OnDeactivate([NotNull] object screen,
                      bool close);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    /// <remarks>Should be used for funky UI stuff (like initial validation, initial focus, ... stuff ... :beers:)</remarks>
    void OnViewReady([NotNull] object screen,
                     [NotNull] object view);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    void OnClose([NotNull] object screen,
                 bool? dialogResult = null);
  }
}
