using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface ILocator<T>
  {
    /// <exception cref="Exception" />
    [NotNull]
    T Locate();
  }
}
