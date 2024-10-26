using Moq;
using System.Reflection;

namespace OLT.Utility.AssemblyScanner.Tests
{
    public class AssemblyScanBuilderTests
    {
        [Fact]
        public void NoMok_ShouldOnlyContainOltAssemblies()
        {
            var assemblies = new OltAssemblyScanBuilder()
                .IncludeFilter("OLT.")
                .ExcludeFilter("System.")
                .ExcludeFilter("Microsoft.")
                .DeepScan()
                .LoadAssemblies()
                .Build();

            Assert.NotEmpty(assemblies);


            assemblies = new OltAssemblyScanBuilder()
                .IncludeFilter("OLT.")
                .DeepScan()
                .LoadAssemblies()
                .Build();

            Assert.NotEmpty(assemblies);

            assemblies = new OltAssemblyScanBuilder()
                .DeepScan()
                .LoadAssemblies()
                .Build();

            Assert.Empty(assemblies);
        }      

        [Fact]
        public void IncludeFilter_ShouldOnlyIncludeMatchingAssemblies()
        {
            // Arrange
            var builder = new OltAssemblyScanBuilder();

            // Act
            builder.IncludeFilter("Test.");

            // Mock a list of assemblies for testing
            var mockAssemblies = new List<Assembly>
            {
                CreateMockAssembly("Test.AssemblyOne"),
                CreateMockAssembly("Test.AssemblyTwo"),
                CreateMockAssembly("Other.Assembly")
            };

            // Inject the mock assemblies for testing purposes
            var result = builder.IncludeAssemblies(mockAssemblies).LoadAssemblies().Build();

            // Assert - only assemblies starting with "Test." should be included
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyTwo");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Other.Assembly");
        }

        [Fact]
        public void ExcludeFilter_ShouldExcludeMatchingAssemblies()
        {
            // Arrange
            var builder = new OltAssemblyScanBuilder();
            builder.IncludeFilter("Test."); // Include all Test.* assemblies
            builder.ExcludeFilter("Test.Exclude");

            // Mock a list of assemblies for testing
            var mockAssemblies = new List<Assembly>
        {
            CreateMockAssembly("Test.AssemblyOne"),
            CreateMockAssembly("Test.ExcludeAssembly"),
            CreateMockAssembly("Test.AssemblyTwo")
        };

            // Inject the mock assemblies for testing purposes
            var result = builder.IncludeAssemblies(mockAssemblies).LoadAssemblies().Build();

            // Assert - assemblies starting with "Test.Exclude" should be excluded
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.Contains(result, a => a.GetName().Name == "Test.AssemblyTwo");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Test.ExcludeAssembly");
        }


        // Helper method to create a mock Assembly
        private Assembly CreateMockAssembly(string assemblyName)
        {
            var mockAssembly = new Mock<Assembly>();
            var mockAssemblyName = new AssemblyName(assemblyName);

            mockAssembly.Setup(a => a.GetName()).Returns(mockAssemblyName);
            return mockAssembly.Object;
        }


    }
}