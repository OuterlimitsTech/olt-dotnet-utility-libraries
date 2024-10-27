using Other.Assembly;
using System.Reflection;
using Test.AssemblyOne;
using Test.AssemblyTwo;

namespace OLT.Utility.AssemblyScanner.Tests5
{
    public class AssemblyScanBuilderTests5
    {

        [Fact]
        public void IncludeFilter_ShouldOnlyIncludeMatchingAssemblies_DeepScan()
        {
            // Arrange
            var builder = new OltAssemblyScanBuilder();

            // Act
            builder.IncludeFilter("Test.");

            var assemblies = new List<Assembly>
            {
                typeof(ClassOne).Assembly,
                typeof(OtherClass).Assembly,
                typeof(ClassTwo).Assembly,
            };

            var result = builder
                .IncludeAssembly(assemblies.ToArray())
                .DeepScan()
                .LoadAssemblies()
                .Build();

            Assert.Equal(4, result.Count());
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyTwo");
            Assert.Contains(result, a => a.GetName().Name == "Test.ExcludeAssembly");
            Assert.Contains(result, a => a.GetName().Name == "Test.IgnoreNameAssembly");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Other.Assembly");
        }
    }
}