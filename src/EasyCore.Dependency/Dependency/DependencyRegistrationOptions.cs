using System.Reflection;

namespace EasyCore.Dependency
{
    /// <summary>
    /// Options that control which assemblies are scanned for dependency markers.
    /// </summary>
    public sealed class DependencyRegistrationOptions
    {
        /// <summary>
        /// Assemblies to scan. When empty, entry assembly and already-loaded
        /// non-framework assemblies are used.
        /// </summary>
        public IList<Assembly> Assemblies { get; } = new List<Assembly>();

        /// <summary>
        /// Optional assembly name prefixes to include (e.g. "MyApp").
        /// When set, only assemblies whose name starts with one of these prefixes are scanned.
        /// </summary>
        public IList<string> AssemblyNamePrefixes { get; } = new List<string>();

        /// <summary>
        /// Adds assemblies to scan.
        /// </summary>
        public DependencyRegistrationOptions AddAssemblies(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (assembly is not null && !Assemblies.Contains(assembly))
                {
                    Assemblies.Add(assembly);
                }
            }

            return this;
        }
    }
}
