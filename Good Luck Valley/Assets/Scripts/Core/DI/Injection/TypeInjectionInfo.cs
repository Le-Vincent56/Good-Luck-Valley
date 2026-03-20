using System;
using System.Reflection;

namespace GoodLuckValley.Core.DI.Injection
{
    /// <summary>
    /// Cached reflection data for [Inject] field and property injection on a specific type.
    /// Built once per type via <see cref="InjectionCache"/> and reused for all subsequent injections;
    /// zero runtime reflection after first access.
    /// </summary>
    internal sealed class TypeInjectionInfo
    {
        /// <summary>
        /// The type this injection info describes.
        /// </summary>
        public Type Type { get; }
        
        /// <summary>
        /// Fields marked with <see cref="InjectAttribute"/> on this type.
        /// </summary>
        public FieldInfo[] InjectableFields { get; }
        
        /// <summary>
        /// Writable properties marked with <see cref="InjectAttribute"/> on this type.
        /// </summary>
        public PropertyInfo[] InjectableProperties { get; }
        
        /// <summary>
        /// True if this type has any fields or properties requiring injection.
        /// </summary>
        public bool HasInjectables => InjectableFields.Length > 0 || InjectableProperties.Length > 0;

        public TypeInjectionInfo(Type type, FieldInfo[] injectableFields, PropertyInfo[] injectableProperties)
        {
            Type = type;
            InjectableFields = injectableFields;
            InjectableProperties = injectableProperties;
        }

        /// <summary>
        /// Sets all [Inject]-marked fields and properties on the target instance
        /// using the provided resolver function to obtain dependency values.
        /// </summary>
        /// <param name="target">The object instance to inject into.</param>
        /// <param name="resolver">
        /// A function that resolves a <see cref="Type"/> to its registered instance
        /// (typically <c>container.Resolve</c>)
        /// </param>
        public void InjectInto(object target, Func<Type, object> resolver)
        {
            foreach (FieldInfo field in InjectableFields)
            {
                field.SetValue(target, resolver(field.FieldType));
            }

            foreach (PropertyInfo property in InjectableProperties)
            {
                property.SetValue(target, resolver(property.PropertyType));
            }
        }
    }
}