using System.Reflection;
using Test.AssemblyOne;
using Test.AssemblyTwo;
using Test.IgnoreNameAssembly;

namespace OLT.Utility.AssemblyScanner.Tests3
{
    public class AssemblyScanBuilderTests3
    {
        [Fact]
        public void ExcludeFilter_ShouldIgnoreNameOfMatchingAssemblies()
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
                    .LoadAssemblies()
                    .Build();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyTwo");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Test.IgnoreNameAssembly");
        }
    }
}