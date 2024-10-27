namespace OLT.Utility.AssemblyScanner.Tests7
{
    public class AssemblyScanBuilderTests7
    {
        [Fact]
        public void IncludeOlt_ShouldOnlyOltAssemblies_DeepScan()
        {
            var result = new OltAssemblyScanBuilder()
                .IncludeFilter("OLT.")
                .DeepScan()
                .LoadAssemblies()
                .Build();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.GetName().Name == "OLT.Utility.AssemblyScanner");
            Assert.Contains(result, a => a.GetName().Name == "OLT.Utility.AssemblyScanner.Tests7");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Test.AssemblyTwo");
        }
    }
}