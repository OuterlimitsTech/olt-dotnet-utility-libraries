using Other.Assembly;
using System.Reflection;
using Test.AssemblyOne;
using Test.AssemblyTwo;

namespace OLT.Utility.AssemblyScanner.Tests6
{
    public class AssemblyScanBuilderTests6
    {

        [Fact]
        public void IncludeFilter_ShouldOnlyIncludeMatchingAssemblies()
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

            var result = builder.IncludeAssembly(assemblies.ToArray()).LoadAssemblies().Build();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyTwo");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Other.Assembly");
        }

    }
}