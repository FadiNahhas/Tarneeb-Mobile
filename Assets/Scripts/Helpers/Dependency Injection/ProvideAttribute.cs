using System;

namespace Helpers.Dependency_Injection
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute { }
}