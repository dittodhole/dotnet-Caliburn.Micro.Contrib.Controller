﻿using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public interface IMixinAttributes : IMixinProvider
  {
    [Pure]
    [NotNull]
    [ItemNotNull]
    CustomAttributeInfo[] GetCustomAttributeInfos();
  }
}