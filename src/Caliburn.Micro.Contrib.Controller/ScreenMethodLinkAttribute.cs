using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class ScreenMethodLinkAttribute : Attribute
  {
    public bool CallBase { get; set; } = true;
    public bool CallAsync { get; set; }

    [CanBeNull]
    public string MethodName { get; set; }
  }
}
