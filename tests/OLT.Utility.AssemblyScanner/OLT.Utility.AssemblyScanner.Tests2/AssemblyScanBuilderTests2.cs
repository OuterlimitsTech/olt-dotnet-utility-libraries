using System.Reflection;
using Test.AssemblyOne;
using Test.AssemblyTwo;
using Test.ExcludeAssembly;

namespace OLT.Utility.AssemblyScanner.Tests2
{
    public class AssemblyScanBuilderTests2
    {
        [Fact]
        public void ExcludeFilter_ShouldExcludeMatchingAssemblies_DeepScan()
        {
            // Arrange
            var builder = new OltAssemblyScanBuilder();
            builder.IncludeFilter("Test."); // Include all Test.* assemblies
            builder.ExcludeFilter("Test.Exclude");


            var assemblies = new List<Assembly>
            {
                typeof(ClassOne).Assembly,
                typeof(ClassExclude).Assembly,
                typeof(ClassTwo).Assembly,
            };

            var result = builder
                .IncludeAssembly(assemblies)
                .DeepScan()
                .LoadAssemblies()
                .Build();

            Assert.Equal(3, result.Count());
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyTwo");
            Assert.Contains(result, a => a.GetName().Name == "Test.IgnoreNameAssembly");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Test.ExcludeAssembly");
        }
    }
}