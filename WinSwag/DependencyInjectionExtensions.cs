using System.Linq;
using System.Reflection;

namespace System
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Resolves and assigns service instances to properties having an <see cref="InjectAttribute"/>.
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="target">Target object with injectable properties</param>
        /// <exception cref="InvalidOperationException">Failed to resolve a specific service</exception>
        public static void Populate(this IServiceProvider serviceProvider, object target)
        {
            var targetType = target.GetType();

            var props = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(prop => prop.CanWrite && prop.GetCustomAttribute<InjectAttribute>() != null);

            var fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.GetCustomAttribute<InjectAttribute>() != null);

            foreach (var prop in props)
                ResolveAndAssignService(prop.PropertyType, prop, service => prop.SetValue(target, service));

            foreach (var field in fields)
                ResolveAndAssignService(field.FieldType, field, service => field.SetValue(target, service));

            // helper method
            void ResolveAndAssignService(Type serviceType, MemberInfo propOrField, Action<object> assign) => assign(
                serviceProvider.GetService(serviceType) ??
                throw new InvalidOperationException($"Could not resolve a service of type '{serviceType}' for property '{propOrField.DeclaringType.Name}.{propOrField.Name}'"));
        }
    }

    /// <summary>
    /// Indicates that a service instance should be injected into the annotated property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : Attribute
    {
    }
}
