using System;

namespace Helpers.Dependency_Injection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute { }
}