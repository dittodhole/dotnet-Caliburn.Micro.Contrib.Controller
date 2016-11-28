using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class ScreenMethodLinkAttribute : Attribute
  {
    public bool SkipInvocation { get; set; }

    [CanBeNull]
    public string MethodName { get; set; }
  }
}
