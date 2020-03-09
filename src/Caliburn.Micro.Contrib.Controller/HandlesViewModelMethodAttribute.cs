using System;

namespace Caliburn.Micro.Contrib.Controller
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class HandlesViewModelMethodAttribute : Attribute
  {
    public string? MethodName { get; set; }
  }
}
