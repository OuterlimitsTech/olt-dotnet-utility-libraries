using Serilog;
using Test.IgnoreNameAssembly;

namespace Test.AssemblyTwo
{
    public class ClassTwo
    {
        private RegularPoco ignoreNameAssembly = new RegularPoco();

        public ClassTwo(HttpClient httpClient)
        {
            
        }

        public void Method1()
        {
            Log.Information("Test {Number}", 1);
        }

        public void Method2()
        {
            Log.Information("Test {Number}", 2);
        }

    }
}
