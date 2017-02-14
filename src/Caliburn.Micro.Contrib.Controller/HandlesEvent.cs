using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class HandlesEvent : Attribute
  {
    public bool CallBase { get; set; } = true;

    [CanBeNull]
    public string MethodName { get; set; }

    [CanBeNull]
    public Type[] MethodParamterTypes { get; set; }
  }
}
