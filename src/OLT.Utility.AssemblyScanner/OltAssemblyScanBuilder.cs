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
        protected bool _loadAssemblies;
        protected bool _deepScan;
        protected List<Assembly> _scanAssemblies = new List<Assembly>();
        
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
            _scanAssemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Insures the Assembly is included within the scanned assembly reference
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder IncludeAssemblies(List<Assembly> assemblies)
        {
            _scanAssemblies.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Deep Scan will get the dependent assemblies
        /// </summary>
        /// <remarks>
        /// NOTE: This does require loading dependent assemblies
        /// </remarks>
        /// <returns></returns>
        public OltAssemblyScanBuilder DeepScan()
        {
            _deepScan = true;
            return this;
        }

        /// <summary>
        /// Load assemblies into <seealso cref="AppDomain.CurrentDomain"/>
        /// </summary>
        /// <returns></returns>
        public OltAssemblyScanBuilder LoadAssemblies()
        {
            _loadAssemblies = true;
            return this;
        }

        /// <summary>
        /// Build method to return filtered assemblies based on the current configuration
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> Build()
        {
            // Get all currently loaded assemblies in the application domain
            _scanAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            _scanAssemblies.Add(Assembly.GetCallingAssembly());
            Assembly? entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly is not null && !_scanAssemblies.Any(asm => asm.Equals(entryAssembly)))
            {
                _scanAssemblies.Add(entryAssembly);
            }


            // Apply include filters
            var filteredAssemblies = _scanAssemblies
                .Where(a => includeFilters.Any(filter => a.GetName().FullName.StartsWith(filter)))
                .ToList();
                

            if (_deepScan)
            {
                foreach (var asm in filteredAssemblies)
                {
                     filteredAssemblies.AddRange(LoadReferencedAssemblies(asm));
                }

                filteredAssemblies = filteredAssemblies
                    .Where(a => includeFilters.Any(filter => a.GetName().FullName.StartsWith(filter)))
                    .ToList();
            }


            // Apply exclude filters
            if (excludeFilters.Any())
            {
                filteredAssemblies = filteredAssemblies
                    .Where(a => !excludeFilters.Any(filter => a.GetName().FullName.StartsWith(filter)))
                    .ToList();
            }

                        
            if (_loadAssemblies)
            {
                foreach (var assemblyName in filteredAssemblies.Select(a => a.GetName()))
                {
                    try
                    {
                        Assembly.Load(assemblyName);
                    }
                    catch (FileNotFoundException) { }
                }
            }

#if NET6_0_OR_GREATER
            return filteredAssemblies.DistinctBy(a => a.GetName().FullName).ToList();
#else
            return filteredAssemblies.DistinctBy(a => a.GetName().FullName).ToList(); 
#endif


        }

        private IEnumerable<Assembly> LoadReferencedAssemblies(Assembly sourceAssembly)
        {
            if (sourceAssembly is null) return Array.Empty<Assembly>();

            List<Assembly> loaded = [];

            foreach (AssemblyName asmName in sourceAssembly.GetReferencedAssemblies())
            {
                loaded.AddRange(LoadAssembly(asmName));
            }

            return loaded;
        }


        private IEnumerable<Assembly> LoadAssembly(AssemblyName asmName)
        {
            if (asmName is null || asmName.FullName.IsNullOrWhiteSpace() || IsIgnored(asmName) || IsAlreadyLoaded(asmName)) return Array.Empty<Assembly>();

            List<Assembly> loaded = [];
            var addAssembly = includeFilters.Count > 0 ? asmName.FullName.StartsWithAny(true, includeFilters.ToArray()) : true;
            if (addAssembly)
            {
                try
                {
                    var assembly = Assembly.Load(asmName);  //Load Assembly into AppDomain
                    loaded.Add(assembly);
                    var children = LoadReferencedAssemblies(assembly);
                    loaded.AddRange(children);
                }
                catch (FileNotFoundException) { }
            }

            return loaded;
        }

        private bool IsIgnored(AssemblyName asmName)
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

#if NET6_0_OR_GREATER
            return alreadyLoaded.Contains(asmName.FullName, StringComparer.OrdinalIgnoreCase) || alreadyLoaded.Any(asm => asm.StartsWith(asmName.FullName, StringComparison.OrdinalIgnoreCase));
#else
            return alreadyLoaded.Contains(asmName.FullName) || alreadyLoaded.Any(asm => asm.StartsWith(asmName.FullName));
#endif
        }


    }
}
