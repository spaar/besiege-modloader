using System;

namespace spaar.ModLoader
{
  /// <summary>
  /// Use this to mark a class extending <c>Mod</c> as a template for other
  /// people to extend. This will cause the class to not be loaded as a mod
  /// and allows you to have more than one implementation of Mod.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false,
    AllowMultiple = false)]
  public sealed class TemplateAttribute : Attribute
  {
  }
}
