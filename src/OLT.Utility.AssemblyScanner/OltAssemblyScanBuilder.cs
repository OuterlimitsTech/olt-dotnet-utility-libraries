using System.Linq;
using System.Reflection;

namespace OLT.Utility
{
    /// <summary>
    /// Load Referenced Assembly Scanner
    /// </summary>
    public class OltAssemblyScanBuilder
    {
        protected List<string> includeFilters = new List<string>();
        protected List<string> excludeFilters = new List<string>();
        protected List<string> _ignoredNames = new List<string>();
        protected bool loadAssemblies;
        protected List<Assembly> scanAssemblies = new List<Assembly>();


        /// <summary>
        /// Add to include filters IncludeFilter("OLT.", "MyApp.")
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder IncludeFilter(params string[] filters)
        {
            includeFilters.AddRange(filters);
            return this;
        }

        /// <summary>
        /// Add to exclude filters ExcludeFilter("Microsoft.", "mscorlib", "netstandard", "Swashbuckle", "System.", "Windows.")
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder ExcludeFilter(params string[] filters)
        {
            excludeFilters.AddRange(filters);
            return this;
        }

        /// <summary>
        /// Adds "Microsoft.", "mscorlib", "netstandard", "Swashbuckle", "System.", "Windows."
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder ExcludeMicrosoft()
        {
            ExcludeFilter("Microsoft.", "mscorlib", "netstandard", "Swashbuckle", "System.", "Windows.");
            return this;
        }

        /// <summary>
        /// Adds "Automapper"
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder ExcludeAutomapper()
        {
            ExcludeFilter("Automapper");
            return this;
        }

        /// <summary>
        /// Ignore any assembly containing the name
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder IgnoreName(params string[] filters)
        {
            _ignoredNames.AddRange(filters);
            return this;
        }

        /// <summary>
        /// Insures the Assembly is included within the scanned assembly reference
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder IncludeAssembly(params Assembly[] assemblies)
        {
            scanAssemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Insures the Assembly is included within the scanned assembly reference
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder IncludeAssemblies(List<Assembly> assemblies)
        {
            scanAssemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Method to enable loading of assemblies
        /// </summary>
        /// <returns></returns>
        public OltAssemblyScanBuilder LoadAssemblies()
        {
            loadAssemblies = true;
            return this;
        }

        /// <summary>
        /// Build method to return filtered assemblies based on the current configuration
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> Build()
        {
            // Get all currently loaded assemblies in the application domain
            scanAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());

            // Apply include filters
            var filteredAssemblies = scanAssemblies
                .Where(a => includeFilters.Any(filter => a.GetName().Name.StartsWith(filter)))
                .ToList();

            // Apply exclude filters
            if (excludeFilters.Any())
            {
                filteredAssemblies = filteredAssemblies
                    .Where(a => !excludeFilters.Any(filter => a.GetName().Name.StartsWith(filter)))
                    .ToList();
            }

            // Optionally load assemblies
            if (loadAssemblies)
            {
                foreach (var assemblyName in filteredAssemblies.Select(a => a.GetName()))
                {
                    try
                    {
                        Assembly.Load(assemblyName);
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions, such as file not found or bad format
                        Console.WriteLine($"Error loading assembly: {assemblyName.Name} - {ex.Message}");
                    }
                }
            }

            return filteredAssemblies;
        }

        private Assembly[] LoadReferencedAssemblies(Assembly sourceAssembly, string filter)
        {
            if (sourceAssembly is null)
                return [];

            List<Assembly> loaded = [];

            foreach (AssemblyName asmName in sourceAssembly.GetReferencedAssemblies())
            {
                loaded.AddRange(LoadAssembly(asmName, filter));
            }

            return [.. loaded];
        }


        private Assembly[] LoadAssembly(AssemblyName asmName, string filter)
        {
            if (asmName is null || asmName.FullName.IsNullOrWhiteSpace() || IsIgnored(asmName) || IsAlreadyLoaded(asmName)) return Array.Empty<Assembly>();


            List<Assembly> loaded = [];


#if NET6_0_OR_GREATER
            if (filter.IsNullOrWhiteSpace() || asmName.FullName.Contains(filter, StringComparison.OrdinalIgnoreCase))
#else
            if (filter.IsNullOrWhiteSpace() || asmName.FullName.ToLower().Contains(filter.ToLower()))
#endif
            {
                try
                {
                    var assembly = Assembly.Load(asmName);

                    loaded.Add(assembly);

                    var childrenLoaded = LoadReferencedAssemblies(assembly, filter);

                    if (childrenLoaded.Length > 0)
                    {
                        loaded.AddRange(childrenLoaded);
                    }
                        
                }
                catch (FileNotFoundException) { }
            }

            return [.. loaded];
        }

        internal bool IsIgnored(AssemblyName asmName)
        {
            return asmName is null
                || asmName.FullName.IsNullOrWhiteSpace()
                || asmName.FullName.StartsWithAny(true, includeFilters.ToArray())
                || asmName.FullName.EqualsAny(true, _ignoredNames.ToArray());
        }

        private bool IsAlreadyLoaded(AssemblyName asmName)
        {
            var alreadyLoaded = AppDomain.CurrentDomain
                                         .GetAssemblies()
                                         .SelectDistinct(asm => asm.FullName ?? string.Empty)
                                         .ToList();

            // The DependencyContext doesn't provide the fully qualified assembly names,
            // so we have to check for StartsWith as well.

            return alreadyLoaded.Contains(asmName.FullName, StringComparer.OrdinalIgnoreCase) || alreadyLoaded.Any(asm => asm.StartsWith(asmName.FullName, StringComparison.OrdinalIgnoreCase));
        }


    }
}
