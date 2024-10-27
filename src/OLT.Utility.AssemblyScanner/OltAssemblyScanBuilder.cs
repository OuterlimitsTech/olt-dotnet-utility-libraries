using System.Reflection;

namespace OLT.Utility.AssemblyScanner
{
    /// <summary>
    /// Scans <see cref="AppDomain.CurrentDomain"/> and <see cref="IncludeAssembly(Assembly[])"/> Assemblies and returns a list of referenced <see cref="Assembly"/>
    /// </summary>
    /// <remarks>
    /// <see cref="IEnumerable{T}"/> Assembiles that can be passed into
    /// <list type="bullet">
    /// <item><a href="https://github.com/khellang/Scrutor/">Scutor</a></item>
    /// <item><a href="https://automapper.org/">Automapper</a></item>
    /// </list>
    /// </remarks>
    public class OltAssemblyScanBuilder
    {
        private List<string> _includeFilters = new List<string>();
        private List<string> _excludeFilters = new List<string>();
        private List<string> _ignoredNames = new List<string>();
        private bool _loadAssemblies;
        private bool _deepScan;
        private List<Assembly> _scanAssemblies = new List<Assembly>();

        /// <summary>
        /// Add to include filters IncludeFilter("OLT.", "MyApp.")
        /// </summary>
        /// <remarks>
        /// Includes <see cref="AssemblyName"/> that start with the <paramref name="filters"/>
        /// </remarks>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder IncludeFilter(params string[] filters)
        {
            _includeFilters.AddRange(filters);
            return this;
        }

        /// <summary>
        /// Add to exclude filters ExcludeFilter("Microsoft.", "mscorlib", "netstandard", "Swashbuckle", "System.", "Windows.")
        /// </summary>
        /// <remarks>
        /// Excludes <see cref="AssemblyName"/> that start with the <paramref name="filters"/>
        /// </remarks>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual OltAssemblyScanBuilder ExcludeFilter(params string[] filters)
        {
            _excludeFilters.AddRange(filters);
            return this;
        }

        /// <summary>
        /// Ignore any assembly containing the <see cref="string"/>
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
        public virtual OltAssemblyScanBuilder IncludeAssembly(IEnumerable<Assembly> assemblies)
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
        /// Build method to return filtered assemblies based on the current <see cref="includeFilters"/> (and <see cref="excludeFilters"/> if provided)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> Build()
        {
            var allAssemblies = new HashSet<Assembly>(new OltAssemblyFullNameComparer());
            var toProcess = new Queue<Assembly>(_scanAssemblies);

            // Add the calling assembly if it's not part of the assemblies to scan and it's not already added
            var callingAssembly = Assembly.GetCallingAssembly();
            if (!_scanAssemblies.Any(a => a.FullName == callingAssembly.FullName))
            {
                toProcess.Enqueue(callingAssembly);
            }

            //Include assemblies already loaded in the AppDomain that match the filters
            foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                toProcess.Enqueue(loadedAssembly);
            }

            while (toProcess.Count > 0)
            {
                var assembly = toProcess.Dequeue();
                if (allAssemblies.Add(assembly))
                {
                    ProcessReferencedAssemblies(assembly, toProcess.Enqueue);
                }
            }

            // Apply include filters
            var filteredAssemblies = allAssemblies
                .Where(a => _includeFilters.Any(filter => a.GetName().FullName.StartsWith(filter)))
                .ToList();

            // Apply exclude filters
            if (_excludeFilters.Any())
            {
                filteredAssemblies = filteredAssemblies
                    .Where(a => !_excludeFilters.Any(filter => a.GetName().FullName.StartsWith(filter)))
                    .ToList();
            }

            if (_ignoredNames.Any())
            {
                filteredAssemblies = filteredAssemblies
                    .Where(a => !_ignoredNames.Any(filter => a.GetName().FullName.Contains(filter)))
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

            return filteredAssemblies.DistinctBy(a => a.GetName().FullName).ToList();
        }

        private void ProcessReferencedAssemblies(Assembly assembly, Action<Assembly> addToQueue)
        {
            if (!_deepScan)
            {
                return;
            }

            var referencedAssemblies = assembly.GetReferencedAssemblies()
                    .Where(a => _includeFilters.Any(filter => a.FullName.StartsWith(filter)))
                    .ToList();

            // Apply exclude filters
            if (_excludeFilters.Any())
            {
                referencedAssemblies = referencedAssemblies
                    .Where(a => !_excludeFilters.Any(filter => a.FullName.StartsWith(filter)))
                    .ToList();
            }

            if (_ignoredNames.Any())
            {
                referencedAssemblies = referencedAssemblies
                    .Where(a => !_ignoredNames.Any(filter => a.FullName.Contains(filter)))
                    .ToList();
            }

            // Load and queue referenced assemblies
            foreach (var reference in referencedAssemblies)
            {
                try
                {
                    var loadedAssembly = Assembly.Load(reference);
                    addToQueue(loadedAssembly);
                }
                catch (Exception)
                {

                }
            }

        }

    }
}
