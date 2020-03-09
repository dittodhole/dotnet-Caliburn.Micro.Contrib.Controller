using System;

namespace Caliburn.Micro.Contrib.Controller
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class HandlesViewModelMethodAttribute : Attribute
  {
    public bool CallBase { get; set; } = true;

    public string? MethodName { get; set; }
  }
}
