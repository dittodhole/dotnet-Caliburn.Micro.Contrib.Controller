﻿using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class ScreenMethodLinkAttribute : Attribute
  {
    public bool SkipInvocation { get; set; }

    [CanBeNull]
    public string MethodName { get; set; }

    public Type InjectInterfaceDefinition { get; set; }
  }
}
