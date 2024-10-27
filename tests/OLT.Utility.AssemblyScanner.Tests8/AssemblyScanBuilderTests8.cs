namespace OLT.Utility.AssemblyScanner.Tests8
{
    public class AssemblyScanBuilderTests8
    {
        [Fact]
        public void IncludeOlt_ShouldOnlyOltAssemblies()
        {
            var result = new OltAssemblyScanBuilder()
                .IncludeFilter("OLT.")
                .ExcludeFilter("System.")
                .ExcludeFilter("Microsoft.")
                .DeepScan()
                .LoadAssemblies()
                .Build();


            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.GetName().Name == "OLT.Utility.AssemblyScanner");
            Assert.Contains(result, a => a.GetName().Name == "OLT.Utility.AssemblyScanner.Tests8");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Test.AssemblyOne");
            Assert.DoesNotContain(result, a => a.GetName().Name == "Test.AssemblyTwo");
        }

    }
}