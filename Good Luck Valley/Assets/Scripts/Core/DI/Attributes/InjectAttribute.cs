using System;

namespace GoodLuckValley.Core.DI.Attributes
{
    /// <summary>
    /// Marks a field or property on a MonoBehaviour adapter for dependency injection.
    /// Injection occurs after Awake and before Start via the PlayerLoop injection phase.
    /// Only used on MonoBehaviour adapters - pure C# classes use constructor injection instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute { }
}