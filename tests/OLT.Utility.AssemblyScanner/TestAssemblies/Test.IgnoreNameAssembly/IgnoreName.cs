using Microsoft.Extensions.Caching.Memory;

namespace Test.IgnoreNameAssembly
{
    public class IgnoreName
    {
        private readonly IMemoryCache _memoryCache;

        public IgnoreName(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
    }

    public class RegularPoco
    {
        public string Testing { get; set; } = "Testing";
    }
}
