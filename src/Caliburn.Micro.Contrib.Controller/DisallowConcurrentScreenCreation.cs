using System;

namespace Caliburn.Micro.Contrib.Controller
{
  /// <remarks>
  ///   When <see cref="IScreenFactoryAdapter" /> classes are annotated
  ///   with this attribute, <see cref="ScreenManager" /> will enforce
  ///   singleton display of linked <see cref="Screen" /> instances -
  ///   as implemented in <see cref="IScreenFactoryAdapter.GetScreenType" /> -
  ///   and return the original <see cref="IScreenFactoryAdapter" /> instance,
  ///   which was used to create the currently displayed <see cref="Screen" />
  ///   instance.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class)]
  public class DisallowConcurrentScreenCreation : Attribute { }
}
