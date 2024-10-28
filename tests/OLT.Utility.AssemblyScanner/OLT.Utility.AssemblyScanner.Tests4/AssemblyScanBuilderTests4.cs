using System.Reflection;
using Test.AssemblyOne;
using Test.AssemblyTwo;
using Test.IgnoreNameAssembly;

namespace OLT.Utility.AssemblyScanner.Tests4
{
    public class AssemblyScanBuilderTests4
    {

        [Fact]
        public void ExcludeFilter_ShouldIgnoreNameOfMatchingAssemblies_DeepScan()
        {
            // Arrange
            var builder = new OltAssemblyScanBuilder();
            builder.IncludeFilter("Test."); // Include all Test.* assemblies
            builder.IgnoreName("IgnoreName"); //Ignore a name containing Exclude

            var assemblies = new List<Assembly>
            {
                typeof(IgnoreName).Assembly,
            };

            var result = builder
                    .IncludeAssembly(assemblies)
                    .IncludeAssembly(typeof(ClassOne).Assembly, typeof(ClassTwo).Assembly)
                    .DeepScan()
                    .LoadAssemblies()
                    .Build();

            Assert.Equal(3, result.Count());
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyTwo");
            Assert.Contains(result, a => a.GetName().Name == "Test.ExcludeAssembly");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Test.IgnoreNameAssembly");
        }

    }
}